# `Slot`

TODO: rework idea -> each row has its own data dictionaries
advantages:
- no user partitioning necessary
-> simpler code
-> no need for garbage collection

possible disadvantage:
- more allocations for low column count/many different types

=> design, implement, compare performance/complexity with column-based

### Space: "Table"

|Name|Type|Access|Description|
|-|-|-|-|
|Items|`Slot`|Read only|contains instances of [`Item`](Item.md)|
|Columns|`Slot`|Read only|TODO: provide a way to quickly query which columns are available, needs a way to cleanup duplicates|

