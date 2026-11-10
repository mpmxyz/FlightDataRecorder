# `Slot`

each instance contains a column of values of a certain (base) type

### Properties

|Name|Type|Access|Description|
|-|-|-|-|
|Name|`string`|Read only|[`ColumnKey`](ColumnKey.md), initialized with a unique value on duplication|
|Active|`bool`||reserved|
|Tag|`string`|Read only|[`ColumnData.Variant`](ColumnData.Variant.md)|
|OrderOffset|`long`||reserved|

## Variant: "T" (single type of value)

### Space: "ColumnData"

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncFieldDictionary<string,T>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value|
|Texts|`SyncFieldDictionary<string,string>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value.ToString()|
|Type|`Type`|Read only|type of the value|

(T=`string` allows `Data` and `Text` to point to the same dictionary.)

## Variant: "Type" (type values)

### Space: "ColumnData"

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncTypeDictionary<string>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value|
|Texts|`SyncFieldDictionary<string,string>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value.ToString()|
|Type|`Type`|Read only|the base class `Type`|

## Variant: "Reference" (used for all values of IWorldElement)

### Space: "ColumnData"

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncRefDictionary<string,IWorldElement>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value|
|Texts|`SyncFieldDictionary<string,string>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> "{Value.RefID}:{Value.ToString()}"|
|Types|`SyncTypeDictionary<string>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value.GetType()|

## Variant: "ObjectContainer" (ProtoFlux-Store based storage)

### Space: "ColumnData"

|Name|Type|Access|Description|
|-|-|-|-|
|Containers|`Slot`|Read only|parent of instances of [`ObjectContainer`](ObjectContainer.md)|
|Containers|`SyncRefDictionary<string,Slot>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> [`ObjectContainer`](ObjectContainer.md), a lookup table to get children of `Containers:Slot` by key|
|Texts|`SyncFieldDictionary<string,string>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value.ToString()|
|Types|`SyncTypeDictionary<string>`|Read only, Collection: Read/Write|[`ItemKey`](ItemKey.md) -> Value.GetType()|
