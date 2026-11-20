using Elements.Core;

using FrooxEngine;
using FrooxEngine.ProtoFlux;

using ProtoFlux.Core;

namespace FlightDataRecorder.ProtoFluxHelpers;
internal class NodeFormatter(ProtoFluxNodeGroup group) {
    private readonly ProtoFluxNodeGroup group = group;

    //first is left of second
    private readonly Dictionary<ProtoFluxNode, HashSet<ProtoFluxNode>> rightNeighbors = [];
    private readonly Dictionary<ProtoFluxNode, int> leftNeighborCounts = [];
    private readonly List<ProtoFluxNode> topologicalOrder = [];
    private readonly Dictionary<ProtoFluxNode, int> topologicalIndices = [];
    private readonly Dictionary<ProtoFluxNode, int> columnIndices = [];
    private readonly Dictionary<int, List<ProtoFluxNode>> nodesGroupedByColumn = [];
    private readonly Dictionary<ProtoFluxNode, float> verticalPositions = [];
    private readonly Dictionary<ProtoFluxNode, float> verticalSizes = [];

    private void ComputeNeighbors() {
        foreach (var node in group.Nodes) {
            var impulseTargets = node.AllTargetOperations
                .Select(target => target.FindNearestParent<ProtoFluxNode>())
                .ToList();
            var inputExpressions = node.AllSourceOutputs
                .Select(target => target.FindNearestParent<ProtoFluxNode>())
                .ToList();
            FlightDataRecorder.Debug($"node={node}, impulseTargets.Count={impulseTargets.Count}, inputExpressions.Count={inputExpressions.Count}");
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
        FlightDataRecorder.Debug($"topologicalIndex={topologicalIndex}, leftNeighborlessNodes.Count={leftNeighborlessNodes.Count}, leftUnvisitedNeighborCounts.Count={leftUnvisitedNeighborCounts.Count}");
        while (leftNeighborlessNodes.Count > 0) {
            foreach (var node in leftNeighborlessNodes) {
                FlightDataRecorder.Debug($"node={node.ReferenceID}/{node.Name}");
                topologicalIndices[node] = topologicalIndex;
                topologicalOrder.Add(node);
                if (rightNeighbors.TryGetValue(node, out var neighbors)) {
                    FlightDataRecorder.Debug($"neighbors.Count={neighbors.Count}");
                    foreach (var neighbor in neighbors) {
                        int remaining = leftUnvisitedNeighborCounts[neighbor];
                        remaining--;
                        FlightDataRecorder.Debug($"neighbor={neighbor.ReferenceID}/{neighbor.Name}, remaining={remaining}");
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
            FlightDataRecorder.Debug($"topologicalIndex={topologicalIndex}, leftNeighborlessNodes.Count={leftNeighborlessNodes.Count}, leftUnvisitedNeighborCounts.Count={leftUnvisitedNeighborCounts.Count}");
        }

        if (leftUnvisitedNeighborCounts.Count > 0) {
            FlightDataRecorder.Error($"Cycle(s) detected:\n{string.Join("\n", leftUnvisitedNeighborCounts.Keys)}");
            throw new InvalidOperationException("Cannot compute topological order with cyclic node graphs!");
        }
    }

    private void ComputeColumnIndices() {
        List<ProtoFluxNode> reversedOrder = [.. topologicalOrder];
        reversedOrder.Reverse();
        FlightDataRecorder.Debug($"reversedOrder.Count={reversedOrder.Count}, (group.NodeCount={group.NodeCount})");
        foreach (var node in reversedOrder) {
            int columnIndex;
            if (rightNeighbors.TryGetValue(node, out var neighbors)) {
                //neighbors.Count > 0 due to how AddNeighbor is implemented.
                columnIndex = neighbors.Select((neighbor) => columnIndices[neighbor]).Min() - 1;
                FlightDataRecorder.Debug($"node={node.ReferenceID}/{node.Name} has neighbors. (columnIndex={columnIndex})");
            } else {
                columnIndex = topologicalIndices[node];
                FlightDataRecorder.Debug($"node={node.ReferenceID}/{node.Name} has no neighbors. (columnIndex={columnIndex})");
            }
            columnIndices[node] = columnIndex;
            if (nodesGroupedByColumn.TryGetValue(columnIndex, out var nodes)) {
                nodes.Add(node);
            } else {
                nodesGroupedByColumn[columnIndex] = [node];
            }
        }
    }

    private void ComputeVerticalPositions() {
        var random = Random.Shared;
        var allNodes = group.Nodes.ToArray();
        foreach (var node in allNodes) {
            FlightDataRecorder.Debug($"node={node.ReferenceID}/{node.Name}");
            var metadata = NodeMetadataHelper.GetMetadata(node.NodeType);
            var leftConnectors = node.NodeOperationCount + node.NodeOperationLists.Sum(it => it.Count) + node.NodeInputCount + node.NodeInputLists.Sum(it => it.Count);
            var rightConnectors = node.NodeImpulseCount + node.NodeImpulseLists.Sum(it => it.Count) + node.NodeOutputCount + node.NodeOutputLists.Sum(it => it.Count);
            var references = node.NodeReferenceCount + node.NodeGlobalRefCount + node.NodeGlobalRefLists.Sum(it => it.Count);
            var connectorHeight = Math.Max(leftConnectors, rightConnectors) * ProtoFluxNodeVisual.ELEMENT_HEIGHT;
            var referenceHeight = references * ProtoFluxNodeVisual.ELEMENT_HEIGHT;
            var fixedHeight = ProtoFluxNodeVisual.LABEL_HEIGHT + ProtoFluxNodeVisual.FLOW_ELEMENT_HEIGHT + ProtoFluxNodeVisual.FOOTER_HEIGHT;
            var buffer = 0.05f;
            var nodeHeight = (connectorHeight + referenceHeight + fixedHeight) / 1000f + buffer;
            verticalSizes[node] = nodeHeight;
            verticalPositions[node] = random.NextSingle();
            FlightDataRecorder.Debug($"node={node.ReferenceID}/{node.Name}, node.position={verticalPositions[node]}, nodeHeight={nodeHeight}");
        }
        float wireSize = ProtoFluxNodeVisual.ELEMENT_HEIGHT / 1000f;

        var appliedForces = new Dictionary<ProtoFluxNode, float>(allNodes.Length);
        for (int i = 0; i < 1000; i++) {
            appliedForces.Clear();
            var connectionPullCoefficient = MathF.Exp(-i * 0.01f);
            var columnSpacingPushCoefficient = MathF.Min(MathF.Exp(-i * 0.007f), MathF.Exp(i * 0.1f-10f));
            var wireSpacingPushCoefficient = MathF.Min(MathF.Exp(-i * 0.007f), MathF.Exp(i * 0.1f-10f));
            FlightDataRecorder.Debug($"columnSpacingPushCoefficient={columnSpacingPushCoefficient}, connectionPullCoefficient={connectionPullCoefficient}");
            foreach (var node in allNodes) {
                int nodeColumn = columnIndices[node];
                float nodePosition = verticalPositions[node];
                float nodeSize = verticalSizes[node];
                if (nodesGroupedByColumn.TryGetValue(nodeColumn, out var nodes)) {
                    foreach(var otherNode in nodes) {
                        if (node != otherNode) {
                            //TODO: push both if too close
                            float otherPosition = verticalPositions[otherNode];
                            float otherSize = verticalSizes[otherNode];
                            float nodeToOtherDiff = otherPosition - nodePosition;
//                            FlightDataRecorder.Warn($"node={node.ReferenceID}/{node.Name}, node={otherNode.ReferenceID}/{otherNode.Name}, nodeToOtherDiff={nodeToOtherDiff}");
                            if (MathF.Abs(nodeToOtherDiff) < 0.001f) {
                                //avoid very very rare case of both positions being equal
                                nodeToOtherDiff = 0.001f;
                            }
                            float overlap = (nodeSize + otherSize) * 0.5f - MathF.Abs(nodeToOtherDiff);
//                            FlightDataRecorder.Warn($"overlap={overlap}");
                            if (overlap > 0f) {
                                float forceAtOther = overlap * MathF.Sign(nodeToOtherDiff) * columnSpacingPushCoefficient;
//                                FlightDataRecorder.Warn($"forceAtOther={forceAtOther}");
                                appliedForces[node] = appliedForces.GetValueOrDefault(node, 0f) - forceAtOther;
                                appliedForces[otherNode] = appliedForces.GetValueOrDefault(otherNode, 0f) + forceAtOther;
                            }
                        }
                    }
                }
                if (rightNeighbors.TryGetValue(node, out var neighbors)){
                    foreach (var neighborNode in neighbors) {
                        int neighborColumn = columnIndices[neighborNode];
                        float neighborPosition = verticalPositions[neighborNode];
                        float neighborSize = verticalSizes[neighborNode];
                        float nodeToNeighborDiff = neighborPosition - nodePosition;
//                        FlightDataRecorder.Warn($"node={node.ReferenceID}/{node.Name}, node={neighborNode.ReferenceID}/{neighborNode.Name}, nodeToNeighborDiff={nodeToNeighborDiff}");
                        float distance = MathF.Abs(nodeToNeighborDiff);
                        if (distance > 0.0001f) {
                            float forceToNeighbor = MathF.Sqrt(distance) * MathF.Sign(nodeToNeighborDiff) * connectionPullCoefficient;
//                            FlightDataRecorder.Warn($"forceToNeighbor={forceToNeighbor}");
                            appliedForces[node] = appliedForces.GetValueOrDefault(node, 0f) + forceToNeighbor;
                            appliedForces[neighborNode] = appliedForces.GetValueOrDefault(neighborNode, 0f) - forceToNeighbor;
                        }
                        //push intermediate nodes away from wire
                        int columnDiff = neighborColumn - nodeColumn;
                        for (int betweenColumn = nodeColumn + 1; betweenColumn < neighborColumn; betweenColumn++) {
                            float lerp = ((float)(betweenColumn - nodeColumn)) / columnDiff;
                            float blendedPosition = MathX.Lerp(nodePosition, neighborPosition, lerp);
                            if (nodesGroupedByColumn.TryGetValue(betweenColumn, out var column)) {
                                foreach (var betweenNode in column) {
                                    float betweenPosition = verticalPositions[betweenNode];
                                    float betweenSize = verticalSizes[betweenNode];
                                    float blendToBetweenDiff = betweenPosition - blendedPosition;
                                    if (MathF.Abs(blendToBetweenDiff) < 0.001f) {
                                        //avoid very very rare case of both positions being equal
                                        blendToBetweenDiff = 0.001f;
                                    }
                                    float overlap = (wireSize + betweenSize) * 0.5f - MathF.Abs(blendToBetweenDiff);
                                    //                            FlightDataRecorder.Warn($"overlap={overlap}");
                                    if (overlap > 0f) {
                                        float forceAtBetween = overlap * MathF.Sign(blendToBetweenDiff) * wireSpacingPushCoefficient;
                                        //                                FlightDataRecorder.Warn($"forceAtBetween={forceAtBetween}");
                                        appliedForces[node] = appliedForces.GetValueOrDefault(node, 0f) - forceAtBetween* (1f - lerp);
                                        appliedForces[neighborNode] = appliedForces.GetValueOrDefault(neighborNode, 0f) - forceAtBetween * lerp;
                                        appliedForces[betweenNode] = appliedForces.GetValueOrDefault(betweenNode, 0f) + forceAtBetween;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var (node, force) in appliedForces) {
                verticalPositions[node] += force;
            }
        }
        //TODO: recenter
    }

    private void ApplyPositions() {
        var random = Random.Shared;
        foreach(var node in group.Nodes) {
            var slot = node.Slot;
            if (columnIndices.TryGetValue(node, out var column)) {
                var verticalPosition = verticalPositions[node];
                FlightDataRecorder.Debug($"Moving:{node} to {column},{verticalPosition}");
                slot.LocalPosition += float3.Right * column * 0.4f;
                slot.LocalPosition += float3.Up * verticalPosition;
            }
            //random shifts to avoid z-fighting:
            slot.LocalPosition += float3.Forward * random.NextSingle() * 0.01f;
        }
    }

    internal void Apply() {
        FlightDataRecorder.Debug($"Formatting:\n{group}");
        ComputeNeighbors();
        ComputeTopologicalIndices();
        ComputeColumnIndices();
        ComputeVerticalPositions();
        ApplyPositions();
    }

    private void AddNeighbor(ProtoFluxNode left, ProtoFluxNode right) {
        FlightDataRecorder.Debug($"Neighbor:{left.ReferenceID}/{left.Name}->{right.ReferenceID}/{right.Name}");
        if (!rightNeighbors.TryGetValue(left, out var relations)) {
            relations = [];
            rightNeighbors[left] = relations;
        }
        if (relations.Add(right)) {
            leftNeighborCounts[right] = leftNeighborCounts.GetValueOrDefault(right, 0) + 1;
        }
    }
}
