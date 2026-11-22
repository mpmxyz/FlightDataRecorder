# API Specification

This part of the API 
TODO: "Logging" shares its dynamic variable space with "Records"
TODO: specify a dynamic variable under `World` space to easily access the API
TODO: implement a "manager" setup

### Space: ???

|Name|Type|Access|Description|
|-|-|-|-|
|AddLogRecordForLink|`Slot`|Read only|reference to the dynamic impulse receiver `sync "AddLogRecordForLink"(Slot)`|

### Dynamic Impulses

|Tag|Type|Description|
|-|-|-|
|AddLogRecordForLink|`Slot` (sync)|The given slot is expected to be an instance of [`EventSourceLink`](Templates/EventSourceLink.md). The call will create a [`Record`](../Records/Templates/Record.md) which is pre-filled with metadata of the event.*|

*) TODO: specify the details of AddLogRecordForLink

