
using Elements.Core;

using FrooxEngine;

using HarmonyLib;

using ResoniteModLoader;

namespace FlightDataRecorder;

public class FlightDataRecorder : ResoniteMod {
	internal const string VERSION_CONSTANT = "0.0.0";
	public override string Name => "FlightDataRecorder";
	public override string Author => "mpmxyz";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/mpmxyz/FlightDataRecorder/";

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> enabled = new("enabled", "Does the mod do anything?", () => true);

	public override void OnEngineInit() {
		Harmony harmony = new("mpmxyz.FlightDataRecorder");
		harmony.PatchAll();
	}

	/// <summary>
	/// This is a minimum example of a mod that adds an action to display "Hello World!" to the DevTool.
	/// </summary>
	[HarmonyPatch(typeof(DevTool), nameof(DevTool.GenerateMenuItems))]
	class DevTool_GenerateMenuItems_Patch {
		public static void Postfix(InteractionHandler tool, ContextMenu menu) {
			if (enabled.Value) {
				menu.AddLocalActionItem("DummyItem", null, colorX.Red, delegate (IButton b, ButtonEventData ev) {
					b.World.Debug.Text(ev.globalPoint, "Hello World!", colorX.White);
				});
			}
		}
	}
}
