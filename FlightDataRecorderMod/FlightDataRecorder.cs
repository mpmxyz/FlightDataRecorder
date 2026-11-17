
using Elements.Core;

using FlightDataRecorder.Generators;

using FrooxEngine;

using HarmonyLib;

using ResoniteModLoader;

namespace FlightDataRecorder;

public class FlightDataRecorder : ResoniteMod {
	internal const string VERSION_CONSTANT = "2.0.0";
	public override string Name => "FlightDataRecorder";
	public override string Author => "mpmxyz";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/mpmxyz/FlightDataRecorder/";

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> enabled = new("enabled", "Does the mod do anything?", () => true);

	public override void OnEngineInit() {
		//Harmony harmony = new("mpmxyz.FlightDataRecorder");
		//harmony.PatchAll();
		DevCreateNewForm.AddAction("", "FlightDataRecorder Data Handlers", (slot) => {
			slot.Name = "Data Handlers";
			slot.AttachComponent<DynamicReferenceVariable<Slot>>(beforeAttach: (variable) => {
				variable.VariableName.Value = "ProtoFlux.ItemData.Handlers";
				variable.Reference.Target = slot;
			});

			List<IHandler> handlers = [
				new TypeHandler(),/*
				new ReferenceHandler<IWorldElement>(),

				new ValueHandler<bool>(),
				new ValueHandler<bool2>(),
				new ValueHandler<bool3>(),
				new ValueHandler<bool4>(),

				new ValueHandler<byte>(),
				new ValueHandler<byte2>(),
				new ValueHandler<byte3>(),
				new ValueHandler<byte4>(),

				new ValueHandler<sbyte>(),
				new ValueHandler<sbyte2>(),
				new ValueHandler<sbyte3>(),
				new ValueHandler<sbyte4>(),

				new ValueHandler<short>(),
				new ValueHandler<short2>(),
				new ValueHandler<short3>(),
				new ValueHandler<short4>(),

				new ValueHandler<ushort>(),
				new ValueHandler<ushort2>(),
				new ValueHandler<ushort3>(),
				new ValueHandler<ushort4>(),

				new ValueHandler<int>(),
				new ValueHandler<int2>(),
				new ValueHandler<int3>(),
				new ValueHandler<int4>(),

				new ValueHandler<uint>(),
				new ValueHandler<uint2>(),
				new ValueHandler<uint3>(),
				new ValueHandler<uint4>(),

				new ValueHandler<long>(),
				new ValueHandler<long2>(),
				new ValueHandler<long3>(),
				new ValueHandler<long4>(),

				new ValueHandler<ulong>(),
				new ValueHandler<ulong2>(),
				new ValueHandler<ulong3>(),
				new ValueHandler<ulong4>(),

				new ValueHandler<float>(),
				new ValueHandler<float2>(),
				new ValueHandler<float3>(),
				new ValueHandler<float4>(),

				new ValueHandler<double>(),
				new ValueHandler<double2>(),
				new ValueHandler<double3>(),
				new ValueHandler<double4>(),

				new ValueHandler<floatQ>(),
				new ValueHandler<float2x2>(),
				new ValueHandler<float3x3>(),
				new ValueHandler<float4x4>(),

				new ValueHandler<doubleQ>(),
				new ValueHandler<double2x2>(),
				new ValueHandler<double3x3>(),
				new ValueHandler<double4x4>(),

				new ValueHandler<color>(),
				new ValueHandler<color32>(),
				new ValueHandler<colorX>(),

				new ValueHandler<char>(),
				new ObjectHandler<string>(),
				new ObjectHandler<Uri>(),

				new ValueHandler<DateTime>(),
				new ValueHandler<TimeSpan>(),

				new ValueHandler<Half>(),
				new ValueHandler<decimal>(),

				new ValueHandler<Rect>(),
				new ValueHandler<BoundingBox>(),

				new ValueHandler<dummy>(),
				new ValueHandler<RefID>()*/
			];
			foreach (var handler in handlers) {
				handler.Build(slot);
			}

            slot.OpenInspectorForTarget();
		});
	}

}
