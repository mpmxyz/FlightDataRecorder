using Elements.Core;

using FlightDataRecorder.ProtoFluxHelpers;

using FrooxEngine;
using FrooxEngine.ProtoFlux;

namespace FlightDataRecorder.Generators;
internal abstract class BaseHandler<T> : IHandler {
    public virtual string Name => typeof(T).GetNiceName();

    public Slot Build(Slot parent) {
        var handlerSlot = parent.AddSlot(Name);
        handlerSlot.Tag = Name;
        handlerSlot.AttachComponent<DynamicVariableSpace>(beforeAttach: (space) => {
            space.SpaceName.Value = "Handler";
            space.OnlyDirectBinding.Value = true;
        });

        var templates = handlerSlot.AddSlot("Templates");
        var template = BuildTemplate(templates);
        if (template != null) {
            template.Name = Name;
            template.Tag = Name;
            templates.AttachComponent<DynamicReferenceVariable<Slot>>(beforeAttach: (variable) => {
                variable.VariableName.Value = "Handler/Template";
                variable.Reference.Target = template;
            });
        }

        var protoFlux = handlerSlot.AddSlot("ProtoFlux");
        var impulseReceivers = BuildProtoFlux(protoFlux);
        FormatProtoFlux(protoFlux);
        protoFlux.AttachComponent<DynamicReferenceVariable<Slot>>(beforeAttach: (variable) => {
            variable.VariableName.Value = "Handler/ReadValue";
            variable.Reference.Target = impulseReceivers.ReadValue;
        });
        protoFlux.AttachComponent<DynamicReferenceVariable<Slot>>(beforeAttach: (variable) => {
            variable.VariableName.Value = "Handler/WriteValue";
            variable.Reference.Target = impulseReceivers.WriteValue;
        });

        return handlerSlot;
    }

    protected abstract ImpulseReceivers BuildProtoFlux(Slot protoFlux);
    protected abstract Slot? BuildTemplate(Slot templates);

    private void FormatProtoFlux(Slot protoFlux) {
        protoFlux.World.RunInUpdates(3, () => {
            var groups = protoFlux.Children
                .Select((child) => child.GetComponent<ProtoFluxNode>()?.Group)
                .OfType<ProtoFluxNodeGroup>()
                .ToHashSet();
            FlightDataRecorder.Warn($"groups.Count={groups.Count}");
            foreach (var group in groups) {
                new NodeFormatter(group).Apply();
            }
        });
    }
}
