# API Specification

### Space: ???

|Name|Type|Access|Description|
|-|-|-|-|
|Record|`Slot`|Write only|[`Record`](Templates/Record.md) to be manipulated|
|Key|`string`|Write only|[`Key`](Values/Key.md), key of the value to be interacted with|
|ClearInputsOnCall|`bool`|Write only|`true` will reset inputs when calling a dynamic impulse. (helps detect errors) `false` will keep the inputs. (simplifies batch processing) |
|OnError|`Slot`|Write only|hierarchy containing a dynamic impulse receiver `sync OnError(string)`, called when errors (optional)|
|WriteValue|`Slot`|Read only|reference to the dynamic impulse receiver `sync "WriteValue"(object)`|
|ReadValue|`Slot`|Read only|reference to the dynamic impulse receiver `sync "ReadValue"(Slot)`|
|AddRecordTo|`Slot`|Read only|reference to the dynamic impulse receiver `sync "AddRecordTo"(Slot)`|

### Dynamic Impulses

|Tag|Type|Description|
|-|-|-|
|WriteValue|`object` (sync)|stores the given value for the currently set `Record:Slot`and `Key:string`|
|ReadValue|`Slot` (sync)|responds with `sync "ReadValue.OnRead"(object)` targeted at the given `Slot` with the stored value for the currently set `Record:Slot`and `Key:string`|
|AddRecordTo|`Slot` (sync)|creates a new [`Record`](Templates/Record.md) as a child of the given slot, also assigns `Record:Slot` with the new instance|