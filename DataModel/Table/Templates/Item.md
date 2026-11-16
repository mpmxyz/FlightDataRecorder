# `Slot`

An `Item` represents a line within the table. Columns of data can be attached to it.

### Properties

|Name|Type|Access|Description|
|-|-|-|-|
|Name|`string`||reserved|
|Active|`bool`||reserved,TODO: used for locking/highlight/selection/garbage collection?|
|Tag|`string`||reserved|
|OrderOffset|`long`|Read/Write?|bit-converted `T:double` to ensure time-consistent order|


### Space: "Item"

|Name|Type|Access|Description|
|-|-|-|-|
|Values|`SyncFieldDictionary<string,string>`|Read only, Collection: Read/Write|[`ColumnKey`](ColumnKey.md) -> Value.ToString()|
|Types|`SyncTypeDictionary<string>`|Read only, Collection: Read/Write|[`ColumnKey`](ColumnKey.md) -> Value.GetType()|
|Data.Variants|`SyncFieldDictionary<string,string>`|Read only, Collection: Read/Write|[`ColumnKey`](ColumnKey.md) -> [`Data.Variant`](Data.Variant), identifies how a value is read/written|

**When a value is added or modified all these dictionaries _must_ be written to.**

Support for more types can be added by adding instances of [`ItemData`](ItemData.md) as children. (best done with `ParentOverride` during duplication so variables are linked instantly)

Implementations may choose to integrate support for more types within the item template itself. The lack of expected dynamic variables within the item should be the _only_ condition to check if an `ItemData` instance has to be added.

`Data.Variant` is necessary because there is no 1 to 1 mapping between the type of a value and the data structure it is stored in.
For example, _all_ implementations of `IWorldElement` are stored within a `SyncRefDictionary<string,IWorldElement>` with the data variant `Reference`. 

Resonite does not offer a way to check that a type implements `IWorldElement` so the variant is used to help implementations choose which type of data structures to access.

Similarly it is possible (but not recommended) to store all data in [`ObjectContainer`](ObjectContainer.md) instances even if more direct ways of storage are defined. `Data.Variant` will make it clear how the data is stored.
