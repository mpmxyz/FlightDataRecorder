# `Slot`

|Name|Type|Description|
|-|-|-|
|Name|`string`|[`ItemKey`](ItemKey.md) -> easily visible in inspectors or when grabbing references|
|Active|`bool`|TODO: used for locking/highlight/selection/garbage collection?|
|OrderOffset|`long`|bit-converted `Time:double` to ensure time-consistent order|


Space: "Item"
|Name|Type|Description|
|-|-|-|
|Columns|`SyncDictionary<string,Slot>`|[`ColumnKey`](ColumnKey.md) -> [`ColumnData`](ColumnData.md)|


