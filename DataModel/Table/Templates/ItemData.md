# `Slot`

These objects extend [`Item`](Item.md) with dictionaries of a specific type.
They can be created on demand but should not be created twice.

The check for existence should be focused on checking for their variables as some implementations may include them within the `Item` template.

### Properties

|Name|Type|Access|Description|
|-|-|-|-|
|Name|`string`||reserved|
|Active|`bool`||reserved|
|Tag|`string`||reserved|
|OrderOffset|`long`||reserved|

## Variant: "T" (single type of value)

### Space: inherited from parent [`Item`](Item.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncFieldDictionary<string,T>`|Read only, Collection: Read/Write|[`ColumnKey`](ColumnKey.md) -> Value|

## Variant: "Type" (type values)

### Space: inherited from parent [`Item`](Item.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncTypeDictionary<string>`|Read only, Collection: Read/Write|[`ColumnKey`](ColumnKey.md) -> Value|

## Variant: "Reference" (used for all values of IWorldElement)

### Space: inherited from parent [`Item`](Item.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncRefDictionary<string,IWorldElement>`|Read only, Collection: Read/Write|[`ColumnKey`](ColumnKey.md) -> Value|

## Variant: "ObjectContainer" (ProtoFlux-Store based storage)

### Space: inherited from parent [`Item`](Item.md) (no `DynamicVariableSpace`)

|Name|Type|Access|Description|
|-|-|-|-|
|Containers|`Slot`|Read only|parent of instances of [`ObjectContainer`](ObjectContainer.md)|
|Containers|`SyncRefDictionary<string,Slot>`|Read only, Collection: Read/Write|[`ColumnKey`](ColumnKey.md) -> [`ObjectContainer`](ObjectContainer.md), a lookup table to get children of `Containers:Slot` by key|
