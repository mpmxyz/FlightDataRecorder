# `Slot`

An `EventSourceLink` represents a connection from an [`EventSource`](EventSource.md) to a [`Log`](Log.md).

The Flight Data Recorder's implementation is bi-directionally connected to a [`TODO: link from Logger to EventSource`](TODO.md) including:
- `DestroyProxy` (both ways)
- `TODO` (other components and behaviors affecting saving, deletion etc.)

### Properties

|Name|Type|Access|Description|
|-|-|-|-|
|Name|`string`||reserved|
|Active|`bool`||reserved*|
|Tag|`string`||reserved|
|OrderOffset|`long`|Read/Write|user-local and usually defined by logger-specific logic, must be `<=0` for links that are _enabled_ (can trigger events) and `>0` for links that are _disabled_ (no events)*|

### Space: "EventSourceLink"

|Name|Type|Access|Description|
|-|-|-|-|
|Log|`Slot`|TODO|the [`Log`](Log.md) the records will be appended to|
|EventSource|`Slot`|TODO|the [`EventSource`](EventSource) this link is part of**|
|DataSources.Target|`SyncRefDictionary<string,Slot>`|TODO, but can be shared by multiple links|[`Key`](../../Records/Values/Key.md) -> `Slot`|
|DataSources.Tag|`SyncRefDictionary<string,Slot>`|TODO, but can be shared by multiple links|[`Key`](../../Records/Values/Key.md) -> `string`|
|DataSources.Payload|`SyncRefDictionary<string,Slot>`|TODO, but can be shared by multiple links|[`Key`](../../Records/Values/Key.md) -> `Slot`|

TODO: Integrate `EventType` in EventSource or here?

*) It was originally intended to use the `Active` property to signal if a link is active. Encoding it in `OrderOffset` allows checking `LinkParent.GetChild(0).OrderOffset <= 0` to confirm if _any_ links are enabled. The practive of using both `Active` and `OrderOffset` like in the original Flight Data Recorder has been discontinued because it introduces redundancy that needs to be handled whenever a value is changed.

**) The original Flight Data Recorder assumed that a link is a child of a source. This specification is more explicit in case there is a use-case to not do that.