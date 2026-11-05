`Slot`:
|Name|Type|Description|
|-|-|-|
|Name|`string`|???|
|Active|`bool`|???|
|Tag|`string`|???|
|OrderOffset|`long`|???|

TODO: id generated from User

Children: instances of [`ColumnData`](ColumnData.md)

Space: "UserPartition"
|Name|Type|Description|
|-|-|-|
|Data|`SyncRefDictionary<string,Slot>`|"{[`ColumnKey`](ColumnKey.md)}.{[`ColumnData.Variant`](ColumnData.Variant.md)}" -> [`ColumnData`](ColumnData.md)|


