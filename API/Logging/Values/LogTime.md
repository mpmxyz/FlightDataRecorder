# `double`, bit-converted to `long`

`LogTime` is a concept that will ensure that log entries recorded across multiple sessions or users are ordered chronologically.

In principle it is just `UTCNow` converted to Unix milliseconds.
`DoubleAsULong(ToUnixMilliseconds(UTCNow()))` could be used as the `OrderOffset` of records to sort them by creation time.

Only doing that can make order inconsistent across users due to differences in their system clock.

This can be combated by using `WorldTimeDouble` as Resonite keeps this consistent across users in a session.

This value is shifted by an offset to make it roughly match UTC in Unix milliseconds:
```
OrderOffset := DoubleAsULong(WorldTimeTouble() + WorldToUTCOffset)
```

The offset should be computed by an authority user _once_ in a session:
```
OnStart(HostOnly=true) -> WorldToUTCOffset := ToUnixMilliseconds(UTCNow()) / 1000.0 - WorldTimeDouble()
```

The value does not change within an update cycle.
Order of record slots is maintained by the implicit default order by `RefID`.
(see https://wiki.resonite.com/Slot#Hierarchy)