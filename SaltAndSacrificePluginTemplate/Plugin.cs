namespace SaltAndSacrificeFreeCam
{
	// this is your plugin class which contains your entry point
	// this will be the first code that runs for your plugin
	[BepInEx.BepInPlugin("com.empio.salt_and_sacrifice_freecam_plugin", "Salt and Sacrifice FreeCam Plugin", "1.0")]
	public class Plugin : BepInEx.NetLauncher.Common.BasePlugin
	{
		// Load is called by the BepInEx preloader when it creates an instance of your plugin
		// Optimisation is disabled for this function as it can cause issues with patching if it is optimised.
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization | System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		public override void Load()
		{
			// this adds your class and method patches into the game code
			HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("salt_and_sacrifice_freecam_plugin");
			harmony.PatchAll();
		}
	}
}
