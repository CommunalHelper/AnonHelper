using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.Anonhelper {
    public static class CustomDashHooks {
        public static void Load() {
            On.Celeste.LevelLoader.LoadingThread += CustomDashInitialize;
            On.Celeste.Player.DashBegin += CustomDashBegin;
            On.Celeste.Player.DashEnd += CustomDashEnd;
            On.Celeste.Player.Die += CustomDashDeath;
            On.Celeste.PlayerHair.GetHairColor += GetCustomDashHairColor;
        }

        public static void Unload() {
            On.Celeste.LevelLoader.LoadingThread -= CustomDashInitialize;
            On.Celeste.Player.DashBegin -= CustomDashBegin;
            On.Celeste.Player.DashEnd -= CustomDashEnd;
            On.Celeste.Player.Die -= CustomDashDeath;
            On.Celeste.PlayerHair.GetHairColor -= GetCustomDashHairColor;
        }

        private static void CustomDashInitialize(On.Celeste.LevelLoader.orig_LoadingThread orig, LevelLoader self) {
            orig.Invoke(self);
            ResetDashSession();
        }

        private static void CustomDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self) {
            if (AnonModule.Session.HasCloudDash) {
                AnonModule.Session.HasCloudDash = false;
                self.Add(new Coroutine(CloudDelay(self)));
            }

            if (AnonModule.Session.HasCoreDash) {
                AnonModule.Session.HasCoreDash = false;
                self.Add(new Coroutine(CoreDelay(self)));
            }

            if (AnonModule.Session.HasJellyDash) {
                AnonModule.Session.HasJellyDash = false;
                self.Add(new Coroutine(JellyDelay(self)));
            }

            if (AnonModule.Session.HasFeatherDash) {
                AnonModule.Session.HasFeatherDash = false;
                self.Add(new Coroutine(FeatherDelay(self)));
            }

            if (AnonModule.Session.HasBoosterDash) {
                AnonModule.Session.HasBoosterDash = false;
                self.Add(new Coroutine(BoosterDelay(self)));
            }

            if (AnonModule.Session.HasSuperDash) {
                AnonModule.Session.HasSuperDash = false;
                AnonModule.Session.StartedSuperDash = true;
            }

            orig.Invoke(self);
        }

        private static void CustomDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self) {
            // We have to split this logic up because SuperDash checks are also done in DashUpdate and DashCoroutine
            if (AnonModule.Session.StartedSuperDash) {
                AnonModule.Session.StartedSuperDash = false;
            }
            
            orig.Invoke(self);
        }

        private static PlayerDeadBody CustomDashDeath(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
            ResetDashSession();
            return orig.Invoke(self, direction, evenIfInvincible, registerDeathInStats);
        }

        private static Color GetCustomDashHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index) {
            if (AnonModule.Session.HasCloudDash) {
                return Color.LightCyan;
            } else if (AnonModule.Session.HasCoreDash && Engine.Scene is Level level && level.Session != null) {
                return level.Session.CoreMode switch {
                    Session.CoreModes.None => Color.DarkRed,
                    Session.CoreModes.Cold => Calc.HexToColor("639bff"),
                    Session.CoreModes.Hot => Color.DarkRed,
                    _ => throw new Exception("invalid coremode"),
                };
            } else if (AnonModule.Session.HasJellyDash) {
                return Calc.HexToColor("1e7bde");
            } else if (AnonModule.Session.HasFeatherDash) {
                return Calc.HexToColor("ffd65c");
            } else if (AnonModule.Session.HasBoosterDash) {
                return Calc.HexToColor("e02817");
            } else if (AnonModule.Session.HasSuperDash) {
                return Calc.HexToColor("006fc2");
            }

            return orig.Invoke(self, index);
        }

        private static IEnumerator CloudDelay(Player player) {
            yield return SaveData.Instance.Assists.SuperDashing ? 0.2f : 0.08f;
            player.Scene.Add(new AnonCloud(player.Position + (player.Speed / 17) + (player.Speed.X >= 288 ? new Vector2(1 / 2, 1) : new Vector2(0, -1)), false, true, false));
        }

        private static IEnumerator CoreDelay(Player player) {
            yield return SaveData.Instance.Assists.SuperDashing ? 0.2f : 0.08f;
            player.Scene.Add(new DestructableBounceBlock(player.Position + (player.Speed / 17) + (Math.Abs(player.Speed.Y) >= 240 ? new Vector2(-8, 3) : new Vector2(-8, 2)), 16, 16));
        }

        private static IEnumerator JellyDelay(Player player) {
            yield return SaveData.Instance.Assists.SuperDashing ? 0.2f : 0.1f;
            player.Scene.Add(new Glider(player.Position + (player.Speed / 17) + (player.Speed.X >= 288 ? new Vector2(1 / 2, 1) : new Vector2(0, -1)), true, false));
        }

        private static IEnumerator FeatherDelay(Player player) {
            yield return 0.3f;
            player.StateMachine.State = Player.StStarFly;
        }

        private static IEnumerator BoosterDelay(Player player) {
            player.Scene.Add(new OneUseBooster(player.Position + new Vector2(0, -7), true));
            yield return 1f;
        }

        private static void ResetDashSession() {
            AnonModule.Session.HasCloudDash = false;
            AnonModule.Session.HasCoreDash = false;
            AnonModule.Session.HasJellyDash = false;
            AnonModule.Session.HasFeatherDash = false;
            AnonModule.Session.HasBoosterDash = false;
            AnonModule.Session.HasSuperDash = false;
            AnonModule.Session.StartedSuperDash = false;
        }
    }
}
