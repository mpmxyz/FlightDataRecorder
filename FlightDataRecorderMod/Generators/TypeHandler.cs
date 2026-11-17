using FrooxEngine;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.Actions;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;

namespace FlightDataRecorder.Generators;
internal class TypeHandler : BaseHandler<Type> {

    protected override ImpulseReceivers BuildProtoFlux(Slot protoFlux) {
        var builder = new NodeBuilder(protoFlux);

        var item = builder.DynamicObjectInput<Slot>("Item");
        var values = builder.ReadDynamicObject<SyncTypeDictionary<string>>(item.Value, "Values");
        var key = builder.DynamicObjectInput<string>("ColumnKey");

        var writeValue = builder.Node<DynamicImpulseReceiverWithObject<object>>((node) => {
            builder.SetGlobalValue(node.Tag, "WriteValue");
            var value = builder.ObjectCast<object, Type>(node.Value);
            //TODO: check type first
            node.OnTriggered.Target = builder.SetTypeWithObjectKey(
                values.Value,
                key.Value,
                value,
                onFail: builder.If(
                    values.FoundValue,
                    onFalse: builder.If(
                        builder.ElementExists(item.Value),
                        onTrue: builder.Node<DuplicateSlot>((node) => {
                            node.Template.Target = builder.DynamicObjectInput<Slot>("Handler/Template").Value;
                            node.OverrideParent.Target = item.Value;
                            node.Next.Target = builder.SetTypeWithObjectKey(
                                values.Value,
                                key.Value,
                                value,
                                onFail: builder.OnError("Failed to write")
                            );
                        }),
                        onFalse: builder.OnError("Handler.WriteValue: invalid/missing Item")
                    )
                )
            );
        });

        var readValue = builder.Node<DynamicImpulseReceiverWithObject<Slot>>((receiver) => {
            builder.SetGlobalValue(receiver.Tag, "ReadValue");
            receiver.OnTriggered.Target = builder.If(
                builder.ContainsObjectKey(values.Value, key.Value),
                onTrue: builder.Node<DynamicImpulseTriggerWithObject<object>>((trigger) => {
                    trigger.Tag.Target = builder.ObjectInput("ReadValue.OnRead");
                    trigger.TargetHierarchy.Target = receiver.Value;
                    var value = builder.GetTypeWithObjectKey(values.Value, key.Value);
                    trigger.Value.Target = builder.ObjectCast<Type, object>(value);
                    //TODO: OnError when there is no receiver
                }));
        });

        return new ImpulseReceivers(readValue.Slot, writeValue.Slot);
    }

    protected override Slot BuildTemplate(Slot templates) {
        var template = templates.AddSlot();
        var dictionary = template.AttachComponent<TypeDictionary<string>>();
        template.AttachComponent<DynamicReferenceVariable<SyncTypeDictionary<string>>>(beforeAttach: (variable) => {
            variable.VariableName.Value = "Values";
            variable.Reference.Target = dictionary.Types;
        });
        return template;
    }
}
