using Bestiary.monsters;
using Common;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using ProjectMage.director;
using SalmonMaps.director.bloom;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace SaltAndSacrificeFreeCam
{
	// if your methods are not being patched in,
	// check that you have a patch attribute on the containing class
	[HarmonyLib.HarmonyPatch]
	internal class MainLogic
	{
		static float speed = 10f;
        static float zoom = 10f;
        static bool show = true;
        static bool wasShowKeyPressed = false;

        // a Prefix will run at the start of the target method
        // this method will run at the start of ProjectMage.Game1.Initialize()
        // since the target method is an instance method, we can get the instance
        // by having the first argument named __instance
        [HarmonyLib.HarmonyPrefix]
		[HarmonyLib.HarmonyPatch(typeof(ProjectMage.Game1), "Initialize")]
		public static void DrawPluginText(ProjectMage.Game1 __instance)
		{
			// this logs green text to the BepInEx console window
			System.ConsoleColor colour = System.Console.ForegroundColor;
			System.Console.ForegroundColor = System.ConsoleColor.Green;
			System.Console.WriteLine($"Game1 initialising => {__instance}");
			System.Console.ForegroundColor = colour;
		}

		// a Postfix will run at the end of the target method
		// this method will run at the end of ProjectMage.player.PlayerMgr.Draw()
		[HarmonyLib.HarmonyPostfix]
		[HarmonyLib.HarmonyPatch(typeof(ProjectMage.player.PlayerMgr), "Draw")]
		public static void DrawPluginText()
		{
			var loc = ProjectMage.character.CharMgr.character[GetMainPlayer().charIdx].loc;
            if (show)
            {
                // this draws text to the screen
                Menumancer.hud.Text.DrawText(new System.Text.StringBuilder("PLAYER LOCATION: " + loc.X + "|" + loc.Y   ),
				    new Common.Vector2(100, 100), Common.Color.White, 0.5f, 0);
                Menumancer.hud.Text.DrawText(new System.Text.StringBuilder("SPEED: " + speed),
                    new Common.Vector2(100, 150), Common.Color.White, 0.5f, 0);
                Menumancer.hud.Text.DrawText(new System.Text.StringBuilder("ZOOM: " + zoom),
                    new Common.Vector2(100, 200), Common.Color.White, 0.5f, 0);
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ProjectMage.player.PlayerMgr), "Draw")]
        public static void DrawIconsd()
        {
			var kState = Keyboard.GetState();

			//speed multiplier
			speed += kState.IsKeyDown(Keys.NumPad7) ? 1 : 0;
            speed -= kState.IsKeyDown(Keys.NumPad9) ? 1 : 0;

			//char locoation
            var loc = ProjectMage.character.CharMgr.character[GetMainPlayer().charIdx].loc;
            loc.X -= kState.IsKeyDown(Keys.NumPad4) ? 1* speed : 0;
            loc.X += kState.IsKeyDown(Keys.NumPad6) ? 1 * speed : 0;
            loc.Y -= kState.IsKeyDown(Keys.NumPad8) ? 1 * speed : 0;
            loc.Y += kState.IsKeyDown(Keys.NumPad2) ? 1 * speed : 0;
			ProjectMage.character.CharMgr.character[GetMainPlayer().charIdx].loc = new Vector2(loc.X, loc.Y);

            //zoom multiplier
            zoom += kState.IsKeyDown(Keys.NumPad1) ? 0.1f : 0;
            zoom -= kState.IsKeyDown(Keys.NumPad3) ? 0.1f : 0;
            GetMainPlayer().camMgr.zoom =-1*zoom;

            //change show
            if (kState.IsKeyDown(Keys.NumPad5))
            {
                if (!wasShowKeyPressed)
                {
                    show = !show;
                    wasShowKeyPressed = true;
                }
            }
            else
            {
                wasShowKeyPressed = false;
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProjectMage.character.Character), nameof(ProjectMage.character.Character.Draw), new[] { typeof(Vector2), typeof(float) })]
        static bool SkipDrawingHUDAndSomeObjects(ProjectMage.character.Character __instance, Vector2 loc, float scale)
        {

            ProjectMage.gamestate.GameState.hideHUD = !show;
            if (__instance.monsterIdx > -1)
            {
                MonsterDef monsterDef = MonsterCatalog.monsterDef[__instance.monsterIdx];
                switch (monsterDef.type)
                {
                    case 0: //NPC
                    case 1: //MONSTER
                    case 6://"Critter";
                    case 7://travel 
                        return show;
                    case 2://"Chest";
                    case 3://"Switch";
                    case 4:// "Trap";
                    case 5://"Harvest";
                        return true;
                    default:
                        return show;

                }


            }

            return true;
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(BloomComponent), "Init")]

        //public static bool Init(GraphicsDevice dev, ContentManager Content)
        //{
        //    GameDraw.depthOnlyEffect = null;
        //    return false;

        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(GameDraw), "Init")]

        //    public static void Init(GraphicsDevice dev, ContentManager Content)
        //{
        //    GameDraw.depthOnlyEffect = null;


        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(SalmonMaps.map.LayerTintCatalog), "SetBloomTint")]

        //public static bool dasd() { 
        //    return false;
        //}

        // a ReversePatch will have its logic replaced with that of the target method
        // this method will call ProjectMage.player.PlayerMgr.GetMainPlayer()
        [HarmonyLib.HarmonyReversePatch]
		[HarmonyLib.HarmonyPatch(typeof(ProjectMage.player.PlayerMgr), "GetMainPlayer")]
		internal static ProjectMage.player.Player GetMainPlayer()
		{
			// this exception and any other logic in this method will be replaced by the reverse patch
			throw new System.NotImplementedException("Gets replaced with PlayerMgr.GetMainPlayer()");
		}
	}
}
