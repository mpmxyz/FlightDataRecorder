using Elements.Core;

using FrooxEngine;

namespace FlightDataRecorder.Generators;
internal abstract class BaseHandler<T> {
    public static string Name => typeof(T).GetNiceName();


    public Slot Build(Slot parent) {
        var handlerSlot = parent.AddSlot(Name);
        handlerSlot.Tag = Name;
        handlerSlot.AttachComponent<DynamicVariableSpace>(beforeAttach: (space) => {
            space.SpaceName.Value = "Handler";
            space.OnlyDirectBinding.Value = true;
        });

        var templates = handlerSlot.AddSlot("Templates");
        var template = BuildTemplate(templates);
        template.Name = Name;
        template.Tag = Name;
        templates.AttachComponent<DynamicReferenceVariable<Slot>>(beforeAttach: (variable) => {
            variable.VariableName.Value = "Handler/Template";
            variable.Reference.Target = template;
        });

        var protoFlux = handlerSlot.AddSlot("ProtoFlux");
        var impulseReceivers = BuildProtoFlux(protoFlux);
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
    protected abstract Slot BuildTemplate(Slot templates);
}
