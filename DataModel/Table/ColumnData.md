# `Slot`

|Name|Type|Description|
|-|-|-|
|Name|`string`|[`ColumnKey`](ColumnKey.md)|
|Active|`bool`|???|
|Tag|`string`|[`ColumnData.Variant`](ColumnData.Variant.md)|
|OrderOffset|`long`|???|

## Variant: "Value" (single type of value)

Space: "ColumnData"
|Name|Type|Description|
|-|-|-|
|Data|`SyncFieldDictionary<string,T>` or `SyncTypeDictionary<string>`|[`ItemKey`](ItemKey.md) -> Value|
|Text|`SyncFieldDictionary<string,string>`|[`ItemKey`](ItemKey.md) -> Value.ToString()|
|Type|`Type`|type of the data|

## Variant: "Reference" (values implementing `IWorldElement`)

Space: "ColumnData"
|Name|Type|Description|
|-|-|-|
|Data|`SyncRefDictionary<string,IWorldElement>`|[`ItemKey`](ItemKey.md) -> Value|
|Text|`SyncFieldDictionary<string,string>`|[`ItemKey`](ItemKey.md) -> "{Value.RefID}:{Value.ToString()}"|
|Type|`SyncTypeDictionary<string>`|[`ItemKey`](ItemKey.md) -> Value.GetType()|

## Variant: "Object" (ProtoFlux-Store based storage)

Children: instances of [`ObjectContainer`](ObjectContainer.md)

Space: "ColumnData"
|Name|Type|Description|
|-|-|-|
|Containers|`SyncRefDictionary<string,Slot>`|[`ItemKey`](ItemKey.md) -> [`ObjectContainer`](ObjectContainer.md), a lookup table to get instances within by key|
|Text|`SyncFieldDictionary<string,string>`|[`ItemKey`](ItemKey.md) -> Value.ToString()|
|Type|`SyncTypeDictionary<string>`|[`ItemKey`](ItemKey.md) -> Value.GetType()|
