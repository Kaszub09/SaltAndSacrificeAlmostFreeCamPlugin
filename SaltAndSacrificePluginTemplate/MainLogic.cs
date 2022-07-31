using Bestiary.monsters;
using Common;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using ProjectMage.director;
using SalmonMaps.director.bloom;
using System;
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

        static string[] MonsterTypeNames = { "NPC","MONSTER", "CHEST", "SWITCH", "TRAP", "HARVEST", "CRITTER", "TRAVEL" };

        static ObjectVisibilitySettings visSettings = new ObjectVisibilitySettings();
        static InputManager input = new InputManager();

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
        [HarmonyLib.HarmonyPatch(typeof(ProjectMage.director.GameDraw), "DrawGame")]
        public static void DrawText() {
            if (show) {
                SpriteTools.BeginAlpha();

                int drawTop = 300;
                var loc = ProjectMage.character.CharMgr.character[GetMainPlayer().charIdx].loc;

                Menumancer.hud.Text.DrawText(new System.Text.StringBuilder("PLAYER LOCATION: " + loc.X + "|" + loc.Y),
                    new Common.Vector2(100, drawTop), Common.Color.White, 0.5f, 0);
                drawTop += 50;
                Menumancer.hud.Text.DrawText(new System.Text.StringBuilder("SPEED: " + speed),
                    new Common.Vector2(100, drawTop), Common.Color.White, 0.5f, 0);
                drawTop += 50;
                Menumancer.hud.Text.DrawText(new System.Text.StringBuilder("ZOOM: " + zoom),
                    new Common.Vector2(100, drawTop), Common.Color.White, 0.5f, 0);
                drawTop += 50;

                for (int i = 0; i < MonsterTypeNames.Length; i++) {
                    Menumancer.hud.Text.DrawText(new System.Text.StringBuilder(MonsterTypeNames[i]+": " + visSettings[i]),
                         new Common.Vector2(100, drawTop), Common.Color.White, 0.3f, 0);
                    drawTop += 30;
                }


                SpriteTools.End();
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ProjectMage.config.InputProfile), "Update", new[] { typeof(int) })]
        public static void UpdateInput()
        {
            input.UpdateKeyboeardState(Keyboard.GetState());

            //SPEED
            input.ResolveKey(input.SpeedPlus, false, () => { speed += 1; });
            input.ResolveKey(input.SpeedMinus, false, () => { speed -= 1; });

            //PLAYER LOCATION
            var loc = ProjectMage.character.CharMgr.character[GetMainPlayer().charIdx].loc;
            input.ResolveKey(input.Move[Direction.Up], false, () => { loc.Y -=  1 * speed; });
            input.ResolveKey(input.Move[Direction.Down], false, () => { loc.Y += 1 * speed; });
            input.ResolveKey(input.Move[Direction.Left], false, () => { loc.X -= 1 * speed; });
            input.ResolveKey(input.Move[Direction.Right], false, () => { loc.X += 1 * speed; });
			ProjectMage.character.CharMgr.character[GetMainPlayer().charIdx].loc = new Vector2(loc.X, loc.Y);

            //ZOOM
            input.ResolveKey(input.ZoomPlus, input.IsModified(1)|| input.IsModified(2), () => {
                if (input.IsModified(1)) {
                    zoom = (float) Math.Round(zoom,0);
                    zoom += 1;
                } else if (input.IsModified(2)) {
                    zoom = (float)Math.Round(zoom, 0);
                    zoom += 10;
                } else {
                    zoom += 1;
                }
            });
            input.ResolveKey(input.ZoomMinus, input.IsModified(1) || input.IsModified(2), () => {
                if (input.IsModified(1)) {
                    zoom = (float)Math.Round(zoom, 0);
                    zoom -= 1;
                } else if (input.IsModified(2)) {
                    zoom = (float)Math.Round(zoom, 0);
                    zoom -= 10;
                } else {
                    zoom -= 1;
                }
            });
            GetMainPlayer().camMgr.zoom =-1*zoom;

            //PLAYER and HUD visibility
            input.ResolveKey(input.PlayerHUD, true, () => {
                visSettings["PLAYER"] = !visSettings["PLAYER"];
                visSettings["HUD"] = !visSettings["HUD"];
            });


            //MONSTER visibility
            foreach(var key in input.MonstersVisibilityKeys) {
                input.ResolveKey(key, true, () => { visSettings[key - Keys.D0] = !visSettings[key - Keys.D0]; });
            }

        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProjectMage.character.Character), nameof(ProjectMage.character.Character.Draw), new[] { typeof(Vector2), typeof(float) })]
        static bool SkipDrawingCharacter(ProjectMage.character.Character __instance, Vector2 loc, float scale) {

            if (__instance.ID == GetMainPlayer().charIdx) {
                return visSettings["PLAYER"];
            }

            if (__instance.monsterIdx > -1) {
                MonsterDef monsterDef = MonsterCatalog.monsterDef[__instance.monsterIdx];
                return visSettings[monsterDef.type];
            }

            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProjectMage.character.Character), nameof(ProjectMage.character.Character.DrawShadow), new[] { typeof(MonsterDef), typeof(bool) })]
        static bool SkipDrawingShadow(ProjectMage.character.Character __instance, MonsterDef mDef, bool direct) {
            if (__instance.ID == GetMainPlayer().charIdx) {
                return visSettings["PLAYER"];
            }

            if (__instance.monsterIdx > -1) {
                MonsterDef monsterDef = MonsterCatalog.monsterDef[__instance.monsterIdx];
                return visSettings[monsterDef.type];
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
