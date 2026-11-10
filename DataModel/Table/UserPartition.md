# `Slot`

contains all data under a given user's authority

Only this user is permitted to add/remove entries.
Modifying existing items is permitted for every user.

### Properties

|Name|Type|Access|Description|
|-|-|-|-|
|Name|`string`||reserved|
|Active|`bool`||reserved|
|Tag|`string`||reserved|
|OrderOffset|`long`||reserved|

### Space: "UserPartition"

|Name|Type|Access|Description|
|-|-|-|-|
|ColumnData|`Slot`|Read only|contains instances of [`ColumnData`](ColumnData.md)|
|ColumnData|`SyncRefDictionary<string,Slot>`|Read only, Collection: Read/Write|"{[`ColumnKey`](ColumnKey.md)}.{[`ColumnData.Variant`](ColumnData.Variant.md)}" -> [`ColumnData`](ColumnData.md), index of `ColumnData:Slot`|
|Owner|`User`|Read only|owner of the partition who has authority over it (`null`/not existing implies host), initialized with `LocalUser` on duplication|

TODO: index of items owned by user