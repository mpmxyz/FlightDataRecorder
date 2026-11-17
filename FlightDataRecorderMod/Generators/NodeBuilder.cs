using FrooxEngine;
using FrooxEngine.ProtoFlux;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.Actions;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.Casts;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.Collections;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Elements;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Variables;

namespace FlightDataRecorder.Generators;
internal class NodeBuilder(Slot parent) {
    private readonly Slot parent = parent;

    internal T Node<T>(Action<T>? init) where T : FrooxEngine.ProtoFlux.ProtoFluxNode, new() {
        var nodeSlot = parent.AddSlot(typeof(T).Name);
        //TODO: positioning
#pragma warning disable CS8604 // Possible null reference argument.
        return nodeSlot.AttachComponent(beforeAttach: init);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    internal ISyncNodeOperation If(INodeValueOutput<bool> condition, INodeOperation? onTrue = null, INodeOperation? onFalse = null) {
        return Node<If>((node) => {
            node.Condition.Target = condition;
#pragma warning disable CS8601 // Possible null reference assignment.
            node.OnTrue.Target = onTrue;
            node.OnFalse.Target = onFalse;
#pragma warning restore CS8601 // Possible null reference assignment.
        });
    }

    internal INodeObjectOutput<T> ObjectInput<T>(T value) {
        return Node<ValueObjectInput<T>>((node) => {
            node.Value.Value = value;
        });
    }

    internal DynamicVariableObjectInput<T> DynamicObjectInput<T>(string value) {
        return Node<DynamicVariableObjectInput<T>>((node) => {
            SetGlobalValue(node.VariableName, "OnError");
        });
    }

    internal ReadDynamicObjectVariable<T> ReadDynamicObject<T>(INodeObjectOutput<Slot> hierarchy, string variableName) {
        var input = ObjectInput(variableName);
        return Node<ReadDynamicObjectVariable<T>>((node) => {
            node.Source.Target = hierarchy;
            node.Path.Target = input;
        });
    }

    internal INodeValueOutput<bool> ElementExists<T>(INodeObjectOutput<T> element) where T : class {
        var input = ObjectCast<T, IWorldElement>(element);
        return Node<ElementExists>((node) => {
            node.Element.Target = input;
        });
    }

    internal INodeObjectOutput<O> ObjectCast<I, O>(INodeObjectOutput<I> input) where I : class where O : class {
        return Node<ObjectCast<I,O>>((node) => {
            node.Input.Target = input;
        });
    }

    internal INodeValueOutput<bool> ContainsObjectKey<K, V>(NodeObjectOutput<SyncFieldDictionary<K, V>> dictionary, NodeObjectOutput<K> key) where V : IEquatable<V> {
        return Node<ContainsObjectKey<SyncFieldDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
        });
    }

    internal SetValueWithObjectKey<SyncFieldDictionary<K, V>, K, V> SetValueWithObjectKey<K, V>(
        INodeObjectOutput<SyncFieldDictionary<K, V>> dictionary,
        INodeObjectOutput<K> key,
        INodeValueOutput<V> value,
        INodeOperation? onSet = null,
        INodeOperation? onFail = null
    ) where V : unmanaged, IEquatable<V> {
        return Node<SetValueWithObjectKey<SyncFieldDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
            node.Value.Target = value;
#pragma warning disable CS8601 // Possible null reference assignment.
            node.OnSet.Target = onSet;
            node.OnFail.Target = onFail;
#pragma warning restore CS8601 // Possible null reference assignment.
        });
    }

    internal GetValueWithObjectKey<SyncFieldDictionary<K, V>, K, V> GetValueWithObjectKey<K, V>(
        INodeObjectOutput<SyncFieldDictionary<K, V>> dictionary,
        INodeObjectOutput<K> key
    ) where V : unmanaged, IEquatable<V> {
        return Node<GetValueWithObjectKey<SyncFieldDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
        });
    }

    internal SetObjectWithObjectKey<SyncFieldDictionary<K, V>, K, V> SetObjectWithObjectKey<K, V>(
        INodeObjectOutput<SyncFieldDictionary<K, V>> dictionary,
        INodeObjectOutput<K> key,
        INodeObjectOutput<V> value,
        INodeOperation? onSet = null,
        INodeOperation? onFail = null
    ) where V : class, IEquatable<V> {
        return Node<SetObjectWithObjectKey<SyncFieldDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
            node.Value.Target = value;
#pragma warning disable CS8601 // Possible null reference assignment.
            node.OnSet.Target = onSet;
            node.OnFail.Target = onFail;
#pragma warning restore CS8601 // Possible null reference assignment.
        });
    }

    internal GetObjectWithObjectKey<SyncFieldDictionary<K, V>, K, V> GetObjectWithObjectKey<K, V>(
        INodeObjectOutput<SyncFieldDictionary<K, V>> dictionary,
        INodeObjectOutput<K> key
    ) where V : class, IEquatable<V> {
        return Node<GetObjectWithObjectKey<SyncFieldDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
        });
    }

    internal INodeValueOutput<bool> ContainsObjectKey<K>(NodeObjectOutput<SyncTypeDictionary<K>> dictionary, NodeObjectOutput<K> key) {
        return Node<ContainsObjectKey<SyncTypeDictionary<K>, K, Type>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
        });
    }

    internal SetObjectWithObjectKey<SyncTypeDictionary<K>, K, Type> SetTypeWithObjectKey<K>(
        INodeObjectOutput<SyncTypeDictionary<K>> dictionary,
        INodeObjectOutput<K> key,
        INodeObjectOutput<Type> value,
        INodeOperation? onSet = null,
        INodeOperation? onFail = null
    ) {
        return Node<SetObjectWithObjectKey<SyncTypeDictionary<K>, K, Type>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
            node.Value.Target = value;
#pragma warning disable CS8601 // Possible null reference assignment.
            node.OnSet.Target = onSet;
            node.OnFail.Target = onFail;
#pragma warning restore CS8601 // Possible null reference assignment.
        });
    }

    internal GetObjectWithObjectKey<SyncTypeDictionary<K>, K, Type> GetTypeWithObjectKey<K>(
        INodeObjectOutput<SyncTypeDictionary<K>> dictionary,
        INodeObjectOutput<K> key
    ) {
        return Node<GetObjectWithObjectKey<SyncTypeDictionary<K>, K, Type>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
        });
    }



    internal INodeValueOutput<bool> ContainsObjectKey<K, V>(NodeObjectOutput<SyncRefDictionary<K, V>> dictionary, NodeObjectOutput<K> key) where V : class, IWorldElement {
        return Node<ContainsObjectKey<SyncRefDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
        });
    }

    internal SetObjectWithObjectKey<SyncRefDictionary<K, V>, K, V> SetReferenceWithObjectKey<K, V>(
        INodeObjectOutput<SyncRefDictionary<K, V>> dictionary,
        INodeObjectOutput<K> key,
        INodeObjectOutput<V> value,
        INodeOperation? onSet = null,
        INodeOperation? onFail = null
    ) where V : class, IWorldElement {
        return Node<SetObjectWithObjectKey<SyncRefDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
            node.Value.Target = value;
#pragma warning disable CS8601 // Possible null reference assignment.
            node.OnSet.Target = onSet;
            node.OnFail.Target = onFail;
#pragma warning restore CS8601 // Possible null reference assignment.
        });
    }

    internal GetObjectWithObjectKey<SyncRefDictionary<K, V>, K, V> GetReferenceWithObjectKey<K, V>(
        INodeObjectOutput<SyncRefDictionary<K, V>> dictionary,
        INodeObjectOutput<K> key
    ) where V : class, IWorldElement {
        return Node<GetObjectWithObjectKey<SyncRefDictionary<K, V>, K, V>>((node) => {
            node.Dictionary.Target = dictionary;
            node.Key.Target = key;
        });
    }

    internal INodeValueOutput<T> Unbox<T>(INodeObjectOutput<object> input) where T : unmanaged {
        return Node<Unbox<T>>((node) => {
            node.Input.Target = input;
        });
    }

    internal INodeObjectOutput<object> Box<T>(INodeValueOutput<T> input) where T : unmanaged {
        return Node<Box<T>>((node) => {
            node.Input.Target = input;
        });
    }

    internal INodeOperation OnError(string messageText) {
        var tag = ObjectInput("OnError");
        var message = ObjectInput(messageText);
        var hierarchy = DynamicObjectInput<Slot>("OnError").Value;
        return Node<DynamicImpulseTriggerWithObject<string>>((node) => {
            node.Tag.Target = tag;
            node.TargetHierarchy.Target = hierarchy;
            node.Value.Target = message;
        });
    }

    internal void SetGlobalValue<T>(SyncRef<IGlobalValueProxy<T>> proxyRef, T value) {
        if (proxyRef.Target == null) {
            proxyRef.Target = proxyRef.Slot.AttachComponent<GlobalValue<T>>();
        }
#pragma warning disable CS8604 // Possible null reference argument.
        if (!proxyRef.Target.TrySetValue(value)) {
            throw new InvalidOperationException($"Failed to assign value {value} to proxy of type {typeof(T)}!");
        }
#pragma warning restore CS8604 // Possible null reference argument.
    }

    internal void SetGlobalReference<T>(SyncRef<IGlobalValueProxy<T>> proxyRef, T value) where T : class, IWorldElement{
        if (proxyRef.Target == null) {
            proxyRef.Target = proxyRef.Slot.AttachComponent<GlobalReference<T>>();
        }
#pragma warning disable CS8604 // Possible null reference argument.
        if (!proxyRef.Target.TrySetValue(value)) {
            throw new InvalidOperationException($"Failed to assign value {value} to proxy of type {typeof(T)}!");
        }
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
