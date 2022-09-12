using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections;

namespace Celeste.Mod.Anonhelper {
    [CustomEntity("Anonhelper/OneUseBooster")]
    public class OneUseBooster : Booster {
        public OneUseBooster(Vector2 position, bool red)
            : base(position, red) {
        }

        public OneUseBooster(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("red")) {
        }

        public static void Load() {
            On.Celeste.Booster.BoostRoutine += BoosterDeath;
        }

        public static void Unload() {
            On.Celeste.Booster.BoostRoutine -= BoosterDeath;
        }

        private static IEnumerator BoosterDeath(On.Celeste.Booster.orig_BoostRoutine orig, Booster self, Player player, Vector2 direction) {
            if (self is OneUseBooster) {
                self.Scene.Remove(DynamicData.For(self).Get<Entity>("outline"));
            }

            yield return new SwapImmediately(orig(self, player, direction));
            if (self is OneUseBooster) {
                yield return 0.8f;
                self.RemoveSelf();
            }
        }
    }
}
