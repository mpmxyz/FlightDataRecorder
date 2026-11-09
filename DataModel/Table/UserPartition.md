# `Slot`

contains all data under a given user's authority

Only this user is permitted to add/remove entries.
Modifying existing items is permitted for every user.

### Properties

|Name|Type|Description|
|-|-|-|
|Name|`string`|reserved|
|Active|`bool`|reserved|
|Tag|`string`|reserved|
|OrderOffset|`long`|reserved|

### Space: "UserPartition"

|Name|Type|Description|
|-|-|-|
|ColumnData|`Slot`|contains instances of [`ColumnData`](ColumnData.md)|
|ColumnData|`SyncRefDictionary<string,Slot>`|"{[`ColumnKey`](ColumnKey.md)}.{[`ColumnData.Variant`](ColumnData.Variant.md)}" -> [`ColumnData`](ColumnData.md), index of `ColumnData:Slot`|
|Owner|`User`|owner of the partition who has authority over it (`null`/not existing implies host)|

