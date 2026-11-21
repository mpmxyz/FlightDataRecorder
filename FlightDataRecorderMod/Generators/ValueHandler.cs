using Elements.Core;

using FlightDataRecorder.ProtoFluxHelpers;

using FrooxEngine;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.Actions;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;

namespace FlightDataRecorder.Generators;
internal class ValueHandler<T> : BaseHandler<T> where T : unmanaged, IEquatable<T> {
    internal ValueHandler(){
        if (!Coder<T>.IsEnginePrimitive) {
            throw new ArgumentException($"Invalid type argument: {typeof(T)}");
        }
    }

    protected override ImpulseReceivers BuildProtoFlux(Slot protoFlux) {
        var builder = new NodeBuilder(protoFlux);

        var record = builder.DynamicObjectInput<Slot>("Record");
        var values = builder.ReadDynamicObject<SyncFieldDictionary<string, T>>(record.Value, "Values");
        var key = builder.DynamicObjectInput<string>("Key");
        var writeValue = builder.Node<DynamicImpulseReceiverWithObject<object>>((node) => {
            builder.SetGlobalValue(node.Tag, "WriteValue");
            var value = builder.Unbox<T>(node.Value);
            //TODO: check type first
            node.OnTriggered.Target = builder.SetValueWithObjectKey(
                values.Value,
                key.Value,
                value,
                onFail: builder.If(
                    values.FoundValue,
                    onFalse: builder.If(
                        builder.ElementExists(record.Value),
                        onTrue: builder.Node<DuplicateSlot>((node) => {
                            node.Template.Target = builder.DynamicObjectInput<Slot>("Handler/Template").Value;
                            node.OverrideParent.Target = record.Value;
                            node.Next.Target = builder.SetValueWithObjectKey(
                                values.Value,
                                key.Value,
                                value,
                                onFail: builder.OnError("Failed to write")
                            );
                        }),
                        onFalse: builder.OnError("Handler.WriteValue: invalid/missing Record")
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
                    var value = builder.GetValueWithObjectKey(values.Value, key.Value);
                    trigger.Value.Target = builder.Box(value);
                }));
        });

        return new ImpulseReceivers(readValue.Slot, writeValue.Slot);
    }

    protected override Slot BuildTemplate(Slot templates) {
        var template = templates.AddSlot();
        var dictionary = template.AttachComponent<ValueDictionary<string, T>>();
        template.AttachComponent<DynamicReferenceVariable<SyncFieldDictionary<string, T>>>(beforeAttach: (variable) => {
            variable.VariableName.Value = "Values";
            variable.Reference.Target = dictionary.Values;
        });
        return template;
    }
}
