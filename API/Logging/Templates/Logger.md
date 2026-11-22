# `Slot`

This a template that can be instanced as an `EventSource` when a user-made object is discovered by a hierarchy-based linking operation.
It acts as a low-overhead interface to user's ProtoFlux code:
- only a single dynamic impulse trigger with a single argument per event
- no message evaluation unless linked and active
- integrated by adding a small module (TBD) with a dynamic impulse receiver listening for the hiearchy-query calling auxiliary code within the API

TODO: This is a distraction from figuring out what the opposite side of `EventSourceLink` points to. -> Maybe something within `Log`? Another entity?


### Space: Logger

|Name|Type|Access|Description|
|-|-|-|-|
|Log|`Slot`|Read/Write|reference to the [`Log`](Log.md) this logger appends to|
|Level|`string`|Read/Write|filters which events are actually logged: Only events at the given level or higher are logged. TODO: make it a dictionary?|
|LogError|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogError"(string)`|
|LogError.object|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogError"(object)`|
|LogWarn|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogWarn"(string)`|
|LogWarn.object|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogWarn"(object)`|
|LogInfo|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogInfo"(string)`|
|LogInfo.object|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogInfo"(object)`|
|LogDebug|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogDebug"(string)`|
|LogDebug.object|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogDebug"(object)`|
|LogTrace|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogTrace"(string)`|
|LogTrace.object|`Slot`|Read only|reference to the dynamic impulse receiver `sync "LogTrace"(object)`|

References to individual impulse receivers will be set to `null` depending on the given log level.
This affects dynamic impulse triggers within the user's code:
- They will not evaluate their input.
- They will not continue the impulse to their output.

### Dynamic Impulses

|Tag|Type|Description|
|-|-|-|
|LogError|`string` (sync)|creates a log entry with the additional data `Level="Error"` and `Message=<value>`|
|LogError|`object` (sync)|creates a log entry with the additional data `Level="Error"` and `Message=<value>`|
|LogWarn|`string` (sync)|creates a log entry with the additional data `Level="Warn"` and `Message=<value>`|
|LogWarn|`object` (sync)|creates a log entry with the additional data `Level="Warn"` and `Message=<value>`|
|LogInfo|`string` (sync)|creates a log entry with the additional data `Level="Info"` and `Message=<value>`|
|LogInfo|`object` (sync)|creates a log entry with the additional data `Level="Info"` and `Message=<value>`|
|LogDebug|`string` (sync)|creates a log entry with the additional data `Level="Debug"` and `Message=<value>`|
|LogDebug|`object` (sync)|creates a log entry with the additional data `Level="Debug"` and `Message=<value>`|
|LogTrace|`string` (sync)|creates a log entry with the additional data `Level="Trace"` and `Message=<value>`|
|LogTrace|`object` (sync)|creates a log entry with the additional data `Level="Trace"` and `Message=<value>`|

*) TODO: specify the details of AddLogRecordForLink

