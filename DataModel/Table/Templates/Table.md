# `Slot`

### Space: "Table"

|Name|Type|Access|Description|
|-|-|-|-|
|UserPartitions|`Slot`|Read only|contains instances of [`UserPartition`](UserPartition.md) (max. one owned partition per user, each owned by a single user)|
|LocalUserPartition|`Slot`|Read/Write (local per user)|references the partition within `UserPartitions:Slot` which is owned by the local user, see note below for detailed instructions*|
|Items|`Slot`|Read only|contains instances of [`Item`](Item.md)|

Note:

`LocalUserPartition:Slot` is driven by a `ReferenceUserOverride<Slot>` with the default set to `null`, `CreateOverrideOnWrite=true` and `PersistentOverrides=false`.

This setup is to make sure that `LocalUserPartition:Slot` either references `null` or the partition the local user has created for themselves. (implies they overwrite the value after creating their partition)

`ReferenceUserOverride<T>` cannot guarantee the correct value just after it has been synced from another user and before its drive has been evaluated. Once dynamic variables are fixed so they don't share a similar delay it will be necessary to check `Owner:User` on the partition before attempting to add/remove data. Implementations which are expected to work under those circumstances _must_ implement a check to confirm that the partition's `Owner` is the local user.