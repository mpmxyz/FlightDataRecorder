# `string`

never null, identifies how and what kind of data is stored

### Known values

|Value|Description|
|-|-|
|T|for values of type `T`|
|Type|for type values|
|Reference|for references (implementing `IWorldElement`)|
|ObjectContainer|for anything else (limitations: heavy-weight, non-networked and non-persistent)|

The following value types `T` are allowed:
- `bool`, `bool2`, `bool3`, `bool4`
- `byte`, `byte2`, `byte3`, `byte4`
- `sbyte`, `sbyte2`, `sbyte3`, `sbyte4`
- `short`, `short2`, `short3`, `short4`
- `ushort`, `ushort2`, `ushort3`, `ushort4`
- `int`, `int2`, `int3`, `int4`
- `uint`, `uint2`, `uint3`, `uint4`
- `long`, `long2`, `long3`, `long4`
- `ulong`, `ulong2`, `ulong3`, `ulong4`
- `float`, `float2`, `float3`, `float4`
- `double`, `double2`, `double3`, `double4`
- `floatQ`, `float2x2`, `float3x3`, `float4x4`
- `doubleQ`, `double2x2`, `double3x3`, `double4x4`
- `color`, `color32`, `colorX`
- `char`, `string`, `Uri`
- `DateTime`, `TimeSpan`
- `half`, `decimal`
- `Rect`, `BoundingBox`
- `dummy`, `RefID`
- `SphericalHarmonicsL1<T>`, `SphericalHarmonicsL2<T>`, `SphericalHarmonicsL3<T>`, `SphericalHarmonicsL4<T>`
- implementations of `IQuantity` or `IEncodable`
- `Nullable<T>`
- enums

Due to the large number of types and limited tooling in Resonite to handle them all it is within reason for implementations to only support a subset of them. Types without direct implementation _can_ be handled by the variant "ObjectContainer".

Implementations reading data created by others and supporting the variant "ObjectContainer" _should_ support reading values from "ObjectContainer" that are directly supported.

Support of types and the "ObjectContainer" variant _should_ be documented by the implementation.

TODO: describe how much the flight data recorder supports (likely all except generic types, interface implementations or enums + "ObjectContainer" support)