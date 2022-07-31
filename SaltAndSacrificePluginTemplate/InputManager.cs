using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaltAndSacrificeFreeCam {
    public enum Direction {
        Up,Right,Down,Left
    }
    internal class InputManager {
        public  Keys Modifier1 = Keys.LeftControl;
        public  Keys Modifier2 = Keys.LeftAlt;
        public  Keys PlayerHUD = Keys.NumPad5;
        public  Keys ZoomPlus = Keys.NumPad1;
        public  Keys ZoomMinus = Keys.NumPad3;
        public  Keys SpeedPlus = Keys.NumPad7;
        public  Keys SpeedMinus = Keys.NumPad9;
        public readonly Dictionary<Direction,Keys> Move = new Dictionary<Direction, Keys>() { { Direction.Up, Keys.NumPad8 }, 
            { Direction.Right, Keys.NumPad6 }, { Direction.Down, Keys.NumPad2 }, { Direction.Left, Keys.NumPad4 }};

        public readonly Keys[] MonstersVisibilityKeys =  { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7 };




        KeyboardState kState;
        List<Keys> pressedKeys = new List<Keys>();

        public void UpdateKeyboeardState(KeyboardState newKeyboardState) {
            kState = newKeyboardState;
        }

        public void ResolveKey(Keys key, bool isSinglePressKey, Action actionToTakeOnPress) {
            if (kState.IsKeyDown(key)) {
                if (!(pressedKeys.Contains(key) && isSinglePressKey)) {
                    actionToTakeOnPress.Invoke();
                    pressedKeys.Add(key);
                } 
            } else {
                if (pressedKeys.Contains(key)) {
                    pressedKeys.Remove(key);
                }
               
            }
        }

        public bool IsModified(int level) {
            if(level == 1) {
                return kState.IsKeyDown(Modifier1);
            } else if(level ==2) {
                return kState.IsKeyDown(Modifier2);
            } else {
                return false;
            }
        }


    }
}
