using Elements.Core;

using FrooxEngine;
using FrooxEngine.ProtoFlux;

namespace FlightDataRecorder.ProtoFluxHelpers;
internal class NodeFormatter(ProtoFluxNodeGroup group) {
    private readonly ProtoFluxNodeGroup group = group;

    //first is left of second
    private readonly Dictionary<ProtoFluxNode, HashSet<ProtoFluxNode>> rightNeighbors = [];
    private readonly Dictionary<ProtoFluxNode, int> leftNeighborCounts = [];
    private readonly List<ProtoFluxNode> topologicalOrder = [];
    private readonly Dictionary<ProtoFluxNode, int> topologicalIndices = [];
    private readonly Dictionary<ProtoFluxNode, int> columnIndices = [];
    //private readonly Dictionary<Slot, Dictionary<Slot, float>> verticalConstraints = [];

    private void ComputeNeighbors() {
        foreach (var node in group.Nodes) {
            var impulseTargets = node.AllTargetOperations
                .Select(target => target.FindNearestParent<ProtoFluxNode>())
                .ToList();
            var inputExpressions = node.AllSourceOutputs
                .Select(target => target.FindNearestParent<ProtoFluxNode>())
                .ToList();
            FlightDataRecorder.Warn($"node={node}, impulseTargets.Count={impulseTargets.Count}, inputExpressions.Count={inputExpressions.Count}");
            foreach (var target in impulseTargets) {
                AddNeighbor(node, target);
            }
            foreach (var source in inputExpressions) {
                AddNeighbor(source, node);
            }
        }
    }

    /// <summary>
    /// This computation assumes the node graph is acyclic.
    /// </summary>
    private void ComputeTopologicalIndices() {
        HashSet<ProtoFluxNode> leftNeighborlessNodes = [.. group.Nodes];
        foreach (var item in leftNeighborCounts.Keys) {
            leftNeighborlessNodes.Remove(item);
        }
        HashSet<ProtoFluxNode> nextLeftNeighborlessNodes = [];
        Dictionary<ProtoFluxNode, int> leftUnvisitedNeighborCounts = new(leftNeighborCounts);

        int topologicalIndex = 0;
        FlightDataRecorder.Warn($"topologicalIndex={topologicalIndex}, leftNeighborlessNodes.Count={leftNeighborlessNodes.Count}, leftUnvisitedNeighborCounts.Count={leftUnvisitedNeighborCounts.Count}");
        while (leftNeighborlessNodes.Count > 0) {
            foreach (var node in leftNeighborlessNodes) {
                FlightDataRecorder.Warn($"node={node.ReferenceID}/{node.Name}");
                topologicalIndices[node] = topologicalIndex;
                topologicalOrder.Add(node);
                if (rightNeighbors.TryGetValue(node, out var neighbors)) {
                    FlightDataRecorder.Warn($"neighbors.Count={neighbors.Count}");
                    foreach (var neighbor in neighbors) {
                        int remaining = leftUnvisitedNeighborCounts[neighbor];
                        remaining--;
                        FlightDataRecorder.Warn($"neighbor={neighbor.ReferenceID}/{neighbor.Name}, remaining={remaining}");
                        if (remaining == 0) {
                            nextLeftNeighborlessNodes.Add(neighbor);
                            leftUnvisitedNeighborCounts.Remove(neighbor);
                        } else {
                            leftUnvisitedNeighborCounts[neighbor] = remaining;
                        }
                    }
                }
            }
            topologicalIndex++;
            (leftNeighborlessNodes, nextLeftNeighborlessNodes) = (nextLeftNeighborlessNodes, leftNeighborlessNodes);
            nextLeftNeighborlessNodes.Clear();
            FlightDataRecorder.Warn($"topologicalIndex={topologicalIndex}, leftNeighborlessNodes.Count={leftNeighborlessNodes.Count}, leftUnvisitedNeighborCounts.Count={leftUnvisitedNeighborCounts.Count}");
        }

        if (leftUnvisitedNeighborCounts.Count > 0) {
            FlightDataRecorder.Error($"Cycle(s) detected:\n{string.Join("\n", leftUnvisitedNeighborCounts.Keys)}");
            throw new InvalidOperationException("Cannot compute topological order with cyclic node graphs!");
        }
    }

    private void ComputeColumnIndices() {
        List<ProtoFluxNode> reversedOrder = [.. topologicalOrder];
        reversedOrder.Reverse();

        foreach(var node in reversedOrder) {
            if (rightNeighbors.TryGetValue(node, out var neighbors)) {
                //neighbors.Count > 0 due to how AddNeighbor is implemented.
                columnIndices[node] = neighbors.Select((neighbor) => columnIndices[neighbor]).Min() - 1;
            } else {
                columnIndices[node] = topologicalIndices[node];
            }
            //TODO: group by column for vertical positioning
        }
    }

    private void ApplyPositions() {
        foreach(var node in group.Nodes) {
            var slot = node.Slot;
            if (columnIndices.TryGetValue(node, out var column)) {
                FlightDataRecorder.Warn($"Moving:{node} to {column}");
                slot.LocalPosition += float3.Right * column * 0.4f;
                //TODO: add random miniscule increments in float3.Forward to avoid z-fighting
            }
        }
    }

    internal void Apply() {
        FlightDataRecorder.Warn($"Formatting:\n{group}");
        ComputeNeighbors();
        ComputeTopologicalIndices();
        ComputeColumnIndices();
        ApplyPositions();
    }

    private void AddNeighbor(ProtoFluxNode left, ProtoFluxNode right) {
        FlightDataRecorder.Warn($"Neighbor:{left.ReferenceID}/{left.Name}->{right.ReferenceID}/{right.Name}");
        if (!rightNeighbors.TryGetValue(left, out var relations)) {
            relations = [];
            rightNeighbors[left] = relations;
        }
        if (relations.Add(right)) {
            leftNeighborCounts[right] = leftNeighborCounts.GetValueOrDefault(right, 0) + 1;
        }
    }
}
