using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaltAndSacrificeFreeCam {
    public class ObjectVisibilitySettings {
        bool[] MonsterVisibility = new bool[8] { true, true, true, true, true, true, true, true };
        Dictionary<string, bool> OtherVisibility = new Dictionary<string, bool>() { { "HUD", true }, { "PLAYER", true } };

        public bool IsMonsterVisible(int type) {
            if (type >= 0 && type <= 7) {
                return MonsterVisibility[type];
            } else {
                return true;
            }
        }

        public void ChangeMonsterVisibility(int type, bool isVisible) {
            if (type >= 0 && type <= 7) {
                MonsterVisibility[type] = isVisible;
            }
        }

        public bool IsVisible(string id) {
            if (!OtherVisibility.ContainsKey(id)) {
                OtherVisibility.Add(id, true);
            }
            return OtherVisibility[id];
        }

        public void ChangeVisibility(string id, bool isVisible) {
            if (!OtherVisibility.ContainsKey(id)) {
                OtherVisibility.Add(id, isVisible);
            } else {
                OtherVisibility[id] = isVisible;
            }
        }

        public bool this[string id] {
            get { return IsVisible(id); }
            set { ChangeVisibility(id, value); }
        }

        public bool this[int type] {
            get { return IsMonsterVisible(type); }
            set { ChangeMonsterVisibility(type, value); }
        }
    }
}
