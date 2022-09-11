using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.Anonhelper {
    [CustomEntity("Anonhelper/JellyRefill")]
    public class JellyRefill : Entity {
        public static ParticleType P_Shatter = new() {
            Source = GFX.Game["particles/triangle"],
            Color = Calc.HexToColor("30a0e6"),
            Color2 = Calc.HexToColor("1e7bde"),
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.25f,
            LifeMax = 0.4f,
            Size = 1f,
            Direction = 4.712389f,
            DirectionRange = 0.87266463f,
            SpeedMin = 140f,
            SpeedMax = 210f,
            SpeedMultiplier = 0.005f,
            RotationMode = ParticleType.RotationModes.Random,
            SpinMin = (float)Math.PI / 2f,
            SpinMax = 4.712389f,
            SpinFlippedChance = true
        };

        public static ParticleType P_Glow = new() {
            LifeMin = 0.4f,
            LifeMax = 0.6f,
            Size = 1f,
            SizeRange = 0f,
            DirectionRange = (float)Math.PI * 2f,
            SpeedMin = 4f,
            SpeedMax = 8f,
            FadeMode = ParticleType.FadeModes.Late,
            Color = Calc.HexToColor("30a0e6"),
            Color2 = Calc.HexToColor("1e7bde"),
            ColorMode = ParticleType.ColorModes.Blink
        };

        public static ParticleType P_Regen = new() {
            LifeMin = 0.4f,
            LifeMax = 0.6f,
            Size = 1f,
            SizeRange = 0f,
            FadeMode = ParticleType.FadeModes.Late,
            Color = Calc.HexToColor("30a0e6"),
            Color2 = Calc.HexToColor("1e7bde"),
            ColorMode = ParticleType.ColorModes.Blink,
            SpeedMin = 30f,
            SpeedMax = 40f,
            SpeedMultiplier = 0.2f,
            DirectionRange = (float)Math.PI * 2f

        };

        public bool addDash;
        public bool HasJellyDash = false;
        public bool jellyDashActive = false;
        public bool dashStarted = false;
        public Player player;

        private readonly Sprite sprite;
        private readonly Sprite flash;
        private readonly Image outline;
        private readonly Wiggler wiggler;
        private readonly BloomPoint bloom;
        private readonly VertexLight light;
        private readonly bool oneUse;
        private readonly SineWave sine;
        private Level level;
        private float respawnTimer;

        public JellyRefill(Vector2 position, bool oneUse)
            : base(position) {
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new PlayerCollider(OnPlayer));
            this.oneUse = oneUse;
            string str = "objects/JellyRefill/";
            Add(outline = new Image(GFX.Game[str + "outline"]));
            outline.CenterOrigin();
            outline.Visible = false;
            Add(sprite = new Sprite(GFX.Game, str + "idle"));
            sprite.AddLoop("idle", "", 1f);
            sprite.Play("idle");
            sprite.CenterOrigin();
            Add(flash = new Sprite(GFX.Game, str + "flash"));
            flash.Add("flash", "", 0.05f);
            flash.OnFinish = delegate {
                flash.Visible = false;
            };
            flash.CenterOrigin();
            Add(wiggler = Wiggler.Create(1f, 4f, delegate (float v) {
                sprite.Scale = flash.Scale = Vector2.One * (1f + (v * 0.2f));
            }));
            Add(new MirrorReflection());
            Add(bloom = new BloomPoint(0.8f, 16f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));
            Add(sine = new SineWave(0.6f, 0f));
            sine.Randomize();
            UpdateY();
            Depth = -100;
        }

        public JellyRefill(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("oneUse")) {
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            level = SceneAs<Level>();
        }

        public override void Update() {
            player = SceneAs<Level>().Tracker.GetEntity<Player>();
            base.Update();
            if (respawnTimer > 0f) {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f) {
                    Respawn();
                }
            } else if (Scene.OnInterval(0.1f)) {
                level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
            }

            UpdateY();
            light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
            if (Scene.OnInterval(2f) && sprite.Visible) {
                flash.Play("flash", restart: true);
                flash.Visible = true;
            }
        }

        private void Respawn() {
            if (!Collidable) {
                Collidable = true;
                sprite.Visible = true;
                outline.Visible = false;
                Depth = -100;
                wiggler.Start();
                Audio.Play("event:/game/general/diamond_return", Position);
                level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
            }
        }

        private void UpdateY() {
            Sprite obj = flash;
            Sprite obj2 = sprite;
            float num2 = bloom.Y = sine.Value * 2f;
            obj.Y = obj2.Y = num2;
        }

        public override void Render() {
            if (sprite.Visible) {
                sprite.DrawOutline();
            }

            base.Render();
        }

        private void OnPlayer(Player player) {
            if (!AnonModule.session.HasJellyDash) {
                player.UseRefill(false);
                AnonModule.session.HasJellyDash = true;
                Audio.Play("event:/game/general/diamond_touch", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                Collidable = false;
                Add(new Coroutine(RefillRoutine(player)));
                respawnTimer = 2.5f;
            }
        }
        private IEnumerator RefillRoutine(Player player) {
            Celeste.Freeze(0.05f);
            yield return null;
            level.Shake();
            sprite.Visible = flash.Visible = false;
            if (!oneUse) {
                outline.Visible = true;
            }

            Depth = 8999;
            yield return 0.05f;
            float num = player.Speed.Angle();
            level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num - ((float)Math.PI / 2f));
            level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num + ((float)Math.PI / 2f));
            SlashFx.Burst(Position, num);
            if (oneUse) {
                RemoveSelf();
            }
        }
    }
}
