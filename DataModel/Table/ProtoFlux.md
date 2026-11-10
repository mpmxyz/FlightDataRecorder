# API-Specification

### Space: ???

|Name|Type|Access|Description|
|-|-|-|-|
|Item|`Slot`|Write only|[`Item`](Templates/Item.md) to be manipulated|
|ColumnKey|`string`|Write only|[`ColumnKey`](Templates/ColumnKey.md), column to be interacted with|
|Table|`Slot`|Read/Write|[`Table`](Templates/Table.md), TODO: check feasability|
|ItemKey|`string`|Write only|[`ItemKey`](Templates/ItemKey.md), TODO: check feasability|
|ClearInputsOnCall|`bool`|Write only|`true` will reset inputs when calling a dynamic impulse. (helps detect errors) `false` will keep the inputs. (simplifies batch processing) |
|OnError|`Slot`|Write only|hierarchy containing a dynamic impulse receiver `sync OnError(string)`, called when errors (optional)|
|WriteValue|`Slot`|Read only|reference to dynamic impulse receiver `sync "WriteValue"(object)`|
|ReadValue|`Slot`|Read only|reference to dynamic impulse receiver `sync "ReadValue"(object)`|

### Dynamic Impulses

|Tag|Type|Description|
|-|-|-|
|WriteValue|`object` (sync)|stores the given value for the currently set `Item:Slot`and `ColumnKey:string`|
|ReadValue|`Slot` (sync)|responds with `sync "ReadValue.OnRead"(object)` targeted at the given `Slot` with the stored value for the currently set `Item:Slot`and `ColumnKey:string`|
