# `double` / bit-converted to `long`

`LogTime` is a concept that will ensure that log entries recorded across multiple sessions or users are ordered chronologically.

In principle it is just `UTCNow` converted to Unix milliseconds.
Just doing that could make order inconsistent due to differences in clock time between users.