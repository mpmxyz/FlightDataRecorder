# `Slot`

These objects extend [`Record`](Record.md) with dictionaries of a specific type.
They can be created on demand but should not be created twice.

The check for existence should be focused on checking for their variables as some implementations may include them within the `Record` template.

### Properties

|Name|Type|Access|Description|
|-|-|-|-|
|Name|`string`||reserved|
|Active|`bool`||reserved|
|Tag|`string`||reserved|
|OrderOffset|`long`||reserved|

## Variant: "T" (single type of value)

### Space: inherited from parent [`Record`](Record.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncFieldDictionary<string,T>`|Read only, Collection: Read/Write|[`Key`](Key.md) -> Value|

## Variant: "Type" (type values)

### Space: inherited from parent [`Record`](Record.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncTypeDictionary<string>`|Read only, Collection: Read/Write|[`Key`](Key.md) -> Value|

## Variant: "IWorldElement" (used for all values of IWorldElement)

### Space: inherited from parent [`Record`](Record.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncRefDictionary<string,IWorldElement>`|Read only, Collection: Read/Write|[`Key`](Key.md) -> Value|

## Variant: "ObjectContainer" (ProtoFlux-Store based storage)

### Space: inherited from parent [`Record`](Record.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Containers|`Slot`|Read only|parent of instances of [`ObjectContainer`](ObjectContainer.md)|
|Containers|`SyncRefDictionary<string,Slot>`|Read only, Collection: Read/Write|[`Key`](Key.md) -> [`ObjectContainer`](ObjectContainer.md), a lookup table to get children of `Containers:Slot` by key|
