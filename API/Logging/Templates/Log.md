# `Slot`

Tables contain a list of flat semi-structured data where each line - [`Record`](../../Records/Templates/Record.md) in this specification - can have data associated with named columns - [`Key`](../../Records/Values/Key.md) in this specification.

The names `Record` and `Key` instead of `Line`/`Row` and `Column` have been chosen because the library mostly does not make assumptions on how items are arranged.
One could also work with individual items embedded in dedicated objects or with items connected as a graph.

There were two ideas on how to structure the items and their data:
1. dictionaries per key with simple slots to represent items
2. items with indidividual data dictionaries

In theory the first approach would result in less allocations per record.
Unfortunately its implementation is so complex (i.e. to handle concurrently created items) that it is even less performant in practice. (approx. double the runtime in a test)

For this reason each record is independent and contains its own data dictionaries.

### Space: "Table"

|Name|Type|Access|Description|
|-|-|-|-|
|Records|`Slot`|Read only|contains instances of [`Record`](Record.md) that each represent a log event|

TODO: metadata to store bones, snapshots of objects etc.
