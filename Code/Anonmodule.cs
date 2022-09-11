using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod;
using Celeste.Mod.Anonhelper;
using Microsoft.Xna.Framework;
using Monocle;
using YamlDotNet.Serialization;
using MonoMod.Utils;
using System.ComponentModel;
using Anonhelper;

namespace Celeste.Mod.Anonhelper
{
    public class AnonModule : EverestModule
    {
        public bool superjumping;

        public static AnonModule Instance;
        public override Type SessionType => typeof(AnonhelperSession);
        public static AnonhelperSession session => (AnonhelperSession)Instance._Session;

        public static SpriteBank spriteBank;
        public AnonModule()
        {
            Instance = this;
        }

        public override void LoadContent(bool firstLoad)
        {
            base.LoadContent(firstLoad);
            spriteBank = new SpriteBank(GFX.Game, "Graphics/carelessAnonymous/Sprites.xml");
        }

        public override void Load()
        {
            On.Celeste.PlayerHair.GetHairColor += CloudDashHairColor;
            On.Celeste.Player.DashBegin += cloudDashBegin;
            On.Celeste.Player.DashEnd += RefillDashEnd;
            On.Celeste.Player.Die += CloudDeath;
            On.Celeste.Level.Render += Level_Render;
            On.Celeste.LevelLoader.LoadingThread += ResetDash;
            On.Celeste.Booster.BoostRoutine += BoosterDeath;
           //On.Celeste.PlayerHair.GetHairScale += SuperHair;
            //On.Celeste.Player.GetCurrentTrailColor += CloudTrail;
        }

        public IEnumerator BoosterDeath(On.Celeste.Booster.orig_BoostRoutine orig, Booster self, Player player, Vector2 direction)
        {
            if (self is OneUseBooster)
            {
                self.Scene.Remove(new DynData<Booster>(self).Get<Entity>("outline"));
            }
            yield return  new SwapImmediately(orig(self, player, direction));
            if (self is OneUseBooster)
            {
                yield return 0.8f;
                self.RemoveSelf();
            }
        }
        public Color CloudDashHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
        {
            if (session.HasCloudDash)
            {
                return Color.LightCyan;
            }
            else if (session.HasCoreDash && Engine.Scene as Level !=null && (Engine.Scene as Level).Session != null)
            {
                switch((Engine.Scene as Level).Session.CoreMode)
                {
                    case Session.CoreModes.None: return Color.DarkRed;
                    case Session.CoreModes.Cold:
                        Console.WriteLine("Cold");
                        return Calc.HexToColor("639bff");
                    case Session.CoreModes.Hot: return Color.DarkRed;
                    default: throw new Exception("invalid coremode");
                }
            }
            else if (session.HasJellyDash)
            {
                return Calc.HexToColor("1e7bde");
            }
            else if (session.HasFeatherDash)
            {
                return Calc.HexToColor("ffd65c");
            }
            else if (session.HasBoosterDash)
            {
                return Calc.HexToColor("e02817");
            }
            else if (session.HasSuperDash)
            { 
                return Calc.HexToColor("006fc2");
            }
            return orig.Invoke(self, index);

        }
       /* public void SuperHair(On.Celeste.PlayerHair.orig_GetHairScale orig, Player self)
        { if (session.HasSuperDash & session.HasCloudDash)

                
        }*/

       /* public Color CloudTrail(On.Celeste.Player.orig_GetCurrentTrailColor orig, bool wasDashB)
        {
            if (session.HasCloudDash)
            {
                return Color.LightCyan;
            }
            else;
            return orig.Invoke(wasDashB); 
        }*/
        private static void Level_Render(On.Celeste.Level.orig_Render orig, Level self)
        {
            foreach (Entity e in self.Tracker.GetEntities<SeekerBarrier>())
            {
                if (e is InvisibleSeekerBarrier) { e.Collider = ((InvisibleSeekerBarrier)e).HideCollider; }
            }
            orig.Invoke(self);
            foreach (Entity e in self.Tracker.GetEntities<SeekerBarrier>())
            {
                if (e is InvisibleSeekerBarrier) { e.Collider = ((InvisibleSeekerBarrier)e).RegCollider; }
            }
        }

        private static void ResetDash(On.Celeste.LevelLoader.orig_LoadingThread orig, LevelLoader self)
        {
            orig.Invoke(self);
            session.HasCloudDash = false;
            session.HasCoreDash = false;
            session.HasJellyDash = false;
            session.HasBoosterDash = false;
            session.HasFeatherDash = false;
            session.HasSuperDash = false;
        }
        private void cloudDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
        {
            if (session.HasCloudDash)
            {
                session.HasCloudDash = false;
                self.Add(new Coroutine(CloudDelay(self)));  
            }
            if (session.HasCoreDash)
            {
                session.HasCoreDash = false;
                self.Add(new Coroutine(CoreDelay(self)));
            }
            if (session.HasJellyDash)
            {
                session.HasJellyDash = false;
                self.Add(new Coroutine(JellyDelay(self)));
            }
            if (session.HasFeatherDash)
            {
                session.HasFeatherDash = false;
                self.Add(new Coroutine(FeatherDelay(self)));
            }
            if (session.HasBoosterDash)
            {
                session.HasBoosterDash = false;
                self.Add(new Coroutine(Boosterplacer(self)));
            }
            if (session.HasSuperDash)
            {
                SaveData.Instance.Assists.SuperDashing = true;
                session.HasSuperDash = false;
            }
            orig.Invoke(self);
        }
        public void RefillDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self)
        {
            SaveData.Instance.Assists.SuperDashing = false;
            orig.Invoke(self);
        }
        private static PlayerDeadBody CloudDeath(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
        {
            session.HasCloudDash = false;
            session.HasCoreDash = false;
            session.HasJellyDash = false;
            session.HasFeatherDash = false;
            session.HasBoosterDash = false;
            session.HasSuperDash = false;
            return orig.Invoke(self, direction, evenIfInvincible, registerDeathInStats);
        }

        public IEnumerator CloudDelay(Player player)
        {
            if (SaveData.Instance.Assists.SuperDashing)
            {
                yield return 0.2f;
            }
            else
            {
                yield return 0.08f;
            }
            Vector2 v = new DynData<Player>(player).Get<Vector2>("lastAim");
            (player.Scene as Level).Add(new AnonCloud(player.Position + player.Speed / 17 + (player.Speed.X >= 288 ? new Vector2((1 / 2), 1) : new Vector2(0, -1)), false, true, false));
        }

        public IEnumerator CoreDelay(Player player)
        {
            if (SaveData.Instance.Assists.SuperDashing)
            {
                yield return 0.2f;
            }
            else
            {
                yield return 0.08f;
            }
            Vector2 v = new DynData<Player>(player).Get<Vector2>("lastAim");
            (player.Scene as Level).Add(new DestructableBounceBlock(player.Position + player.Speed / 17 + (Math.Abs(player.Speed.Y) >= 240 ? new Vector2(-8,3) : new Vector2(-8,2)), 16, 16));
        }

        public IEnumerator JellyDelay(Player player)
        {
            if (SaveData.Instance.Assists.SuperDashing)
            {
                yield return 0.2f;
            }
            else
            {
                yield return 0.1f;
            }
            Vector2 v = new DynData<Player>(player).Get<Vector2>("lastAim");
            Glider glider = new Glider(player.Position + player.Speed / 17 + (player.Speed.X >= 288 ? new Vector2((1 / 2), 1) : new Vector2(0, -1)), true, false);
            DynData<Glider> dyn = new DynData<Glider>(glider);
            Sprite sprite = dyn.Get<Sprite>("sprite");
            new DynData<Sprite>(sprite)["atlas"] = GFX.Game;
            sprite.Add("spawn", "objects/glider/death", 0.01f, "idle", 7, 6, 5, 4, 3, 2, 1, 0);
            (player.Scene as Level).Add(glider);
            sprite.Play("spawn");
        }

        public IEnumerator FeatherDelay(Player player)
        {
            //if (Math.Abs(player.DashDir.X) <=0.2f)
            //{ yield return .5f; }
            //if ((player.DashDir.Y) <= 0.2f)
            //{ yield return 1f; }
            yield return .3f;
            player.StateMachine.State = 19; 
        }

        public IEnumerator Boosterplacer(Player player)
        {
                
                BoosterDelay(player);
                yield return 1f;
        }

        public void BoosterDelay(Player player)
        {
           (player.Scene as Level).Add(new OneUseBooster(player.Position + new Vector2 (0,-10), true));
            return;
        }       

        private Vector2 CorrectDashPrecision(Vector2 dir)
        {
            if (dir.X != 0f && Math.Abs(dir.X) < 0.001f)
            {
                dir.X = 0f;
                dir.Y = Math.Sign(dir.Y);
            }
            {
                dir.Y = 0f;
                dir.X = Math.Sign(dir.X);
            }
            return dir;
        }
        public override void Unload()
        {
            On.Celeste.PlayerHair.GetHairColor -= CloudDashHairColor;
            On.Celeste.Player.DashBegin -= cloudDashBegin;
            On.Celeste.Player.Die -= CloudDeath;
            On.Celeste.Level.Render -= Level_Render;
            On.Celeste.LevelLoader.LoadingThread -= ResetDash;
            On.Celeste.Booster.BoostRoutine -= BoosterDeath;
            On.Celeste.Player.DashEnd -= RefillDashEnd;
            //On.Celeste.Player.GetCurrentTrailColor -= CloudTrail;
        }
    }
}