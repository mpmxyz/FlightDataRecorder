# `Slot`

### Properties

|Name|Type|Description|
|-|-|-|
|Name|`string`|[`ItemKey`](ItemKey.md), initialized with a unique value on duplication|
|Active|`bool`|TODO: used for locking/highlight/selection/garbage collection?|
|OrderOffset|`long`|bit-converted `Time:double` to ensure time-consistent order|


### Space: "Item"

|Name|Type|Description|
|-|-|-|
|Data|`SyncRefDictionary<string,Slot>`|[`ColumnKey`](ColumnKey.md) -> [`ColumnData`](ColumnData.md)|


