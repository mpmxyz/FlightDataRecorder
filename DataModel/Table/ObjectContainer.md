# `Slot`

|Name|Type|Description|
|-|-|-|
|Name|`string`|[ItemKey](ItemKey.md)|
|Active|`bool`|???|
|Tag|`string`|???|
|OrderOffset|`long`|???|

Dynamic Impulses:
|Tag|Type|Description|
|-|-|-|
|WriteValue|`object` (sync)|stores the given value, updates `Text` and `Type` dictionaries of parent|
|ReadValue|`Slot` (sync)|responds with `sync "ReadValue.OnRead"(object)` targeted at the given `Slot` with the stored value|
