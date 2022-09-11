using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.Anonhelper {
    [CustomEntity("Anonhelper/DestructableBounceBlock")]
    public class DestructableBounceBlock : Solid {
        private readonly List<Image> hotImages;
        private readonly List<Image> coldImages;
        private readonly Sprite hotCenterSprite;
        private readonly Sprite coldCenterSprite;
        private readonly bool notCoreMode;
        private Vector2 bounceDir;
        private States state;
        private Vector2 startPos;
        private float moveSpeed;
        private float windUpStartTimer;
        private float windUpProgress;
        private bool iceMode;
        private bool iceModeNext;
        private float respawnTimer;
        private float bounceEndTimer;
        private Vector2 bounceLift;
        private float reappearFlash;
        private bool reformed;
        private Vector2 debrisDirection;

        public DestructableBounceBlock(Vector2 position, float width, float height) 
            : base(position, width, height, safe: false) {
            reformed = true;
            state = States.Waiting;
            startPos = Position;
            hotImages = BuildSprite(GFX.Game["objects/bumpblocknew/fire00"]);
            hotCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterFire");
            hotCenterSprite.Position = new Vector2(Width, Height) / 2f;
            hotCenterSprite.Visible = false;
            Add(hotCenterSprite);
            coldImages = BuildSprite(GFX.Game["objects/bumpblocknew/ice00"]);
            coldCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterIce");
            coldCenterSprite.Position = new Vector2(Width, Height) / 2f;
            coldCenterSprite.Visible = false;
            Add(coldCenterSprite);
            Add(new CoreModeListener(OnChangeMode));
        }

        public DestructableBounceBlock(EntityData data, Vector2 offset) 
            : this(data.Position + offset, data.Width, data.Height) {
            notCoreMode = data.Bool("notCoreMode");
        }

        public enum States {
            Waiting,
            WindingUp,
            Bouncing,
            BounceEnd,
            Broken
        }

        private List<Image> BuildSprite(MTexture source) {
            List<Image> list = new();
            int num = source.Width / 8;
            int num2 = source.Height / 8;
            for (int i = 0; i < Width; i += 8) {
                for (int j = 0; j < Height; j += 8) {
                    int num3 = i != 0 ? !(i >= Width - 8f) ? Calc.Random.Next(1, num - 1) : num - 1 : 0;
                    int num4 = j != 0 ? !(j >= Height - 8f) ? Calc.Random.Next(1, num2 - 1) : num2 - 1 : 0;
                    Image image = new(source.GetSubtexture(num3 * 8, num4 * 8, 8, 8)) {
                        Position = new Vector2(i, j)
                    };
                    list.Add(image);
                    Add(image);
                }
            }

            return list;
        }
        private void ToggleSprite() {
            hotCenterSprite.Visible = !iceMode;
            coldCenterSprite.Visible = iceMode;
            foreach (Image hotImage in hotImages) {
                hotImage.Visible = !iceMode;
            }

            foreach (Image coldImage in coldImages) {
                coldImage.Visible = iceMode;
            }
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            iceModeNext = iceMode = SceneAs<Level>().CoreMode == Session.CoreModes.Cold || notCoreMode;
            ToggleSprite();
        }

        private void OnChangeMode(Session.CoreModes coreMode) {
            iceModeNext = coreMode == Session.CoreModes.Cold;
        }

        private void CheckModeChange() {
            if (iceModeNext != iceMode) {
                iceMode = iceModeNext;
                ToggleSprite();
            }
        }

        public override void Render() {
            Vector2 position = Position;
            Position += Shake;
            if (state != States.Broken && reformed) {
                base.Render();
            }

            if (reappearFlash > 0f) {
                float num = Ease.CubeOut(reappearFlash);
                float num2 = num * 2f;
                Draw.Rect(X - num2, Y - num2, Width + (num2 * 2f), Height + (num2 * 2f), Color.White * num);
            }

            Position = position;
        }

        public override void Update() {
            base.Update();
            reappearFlash = Calc.Approach(reappearFlash, 0f, Engine.DeltaTime * 8f);
            if (state == States.Waiting) {
                CheckModeChange();
                moveSpeed = Calc.Approach(moveSpeed, 100f, 400f * Engine.DeltaTime);
                Vector2 vector = Calc.Approach(ExactPosition, startPos, moveSpeed * Engine.DeltaTime);
                Vector2 liftSpeed = (vector - ExactPosition).SafeNormalize(moveSpeed);
                liftSpeed.X *= 0.75f;
                MoveTo(vector, liftSpeed);
                windUpProgress = Calc.Approach(windUpProgress, 0f, 1f * Engine.DeltaTime);
                Player player = WindUpPlayerCheck();
                if (player != null) {
                    moveSpeed = 80f;
                    windUpStartTimer = 0f;
                    bounceDir = iceMode ? -Vector2.UnitY : (player.Center - Center).SafeNormalize();
                    state = States.WindingUp;
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    if (iceMode) {
                        StartShaking(0.2f);
                        Audio.Play("event:/game/09_core/iceblock_touch", Center);
                    } else {
                        Audio.Play("event:/game/09_core/bounceblock_touch", Center);
                    }
                }
            } else if (state == States.WindingUp) {
                Player player2 = WindUpPlayerCheck();
                if (player2 != null) {
                    bounceDir = iceMode ? -Vector2.UnitY : (player2.Center - Center).SafeNormalize();
                }

                if (windUpStartTimer > 0f) {
                    windUpStartTimer -= Engine.DeltaTime;
                    windUpProgress = Calc.Approach(windUpProgress, 0f, 1f * Engine.DeltaTime);
                    return;
                }

                moveSpeed = Calc.Approach(moveSpeed, iceMode ? 35f : 40f, 600f * Engine.DeltaTime);
                float num = iceMode ? 0.333f : 1f;
                Vector2 vector2 = startPos - (bounceDir * (iceMode ? 16f : 10f));
                Vector2 vector3 = Calc.Approach(ExactPosition, vector2, moveSpeed * num * Engine.DeltaTime);
                Vector2 liftSpeed2 = (vector3 - ExactPosition).SafeNormalize(moveSpeed * num);
                liftSpeed2.X *= 0.75f;
                MoveTo(vector3, liftSpeed2);
                windUpProgress = Calc.ClampedMap(Vector2.Distance(ExactPosition, vector2), 16f, 2f);
                if (iceMode && Vector2.DistanceSquared(ExactPosition, vector2) <= 12f) {
                    StartShaking(0.1f);
                } else if (!iceMode && windUpProgress >= 0.5f) {
                    StartShaking(0.1f);
                }

                if (Vector2.DistanceSquared(ExactPosition, vector2) <= 2f) {
                    if (iceMode) {
                        Break();
                    } else {
                        state = States.Bouncing;
                    }

                    moveSpeed = 0f;
                }
            } else if (state == States.Bouncing) {
                moveSpeed = Calc.Approach(moveSpeed, 140f, 800f * Engine.DeltaTime);
                Vector2 vector4 = startPos + (bounceDir * 24f);
                Vector2 vector5 = Calc.Approach(ExactPosition, vector4, moveSpeed * Engine.DeltaTime);
                bounceLift = (vector5 - ExactPosition).SafeNormalize(Math.Min(moveSpeed * 3f, 200f));
                bounceLift.X *= 0.75f;
                MoveTo(vector5, bounceLift);
                windUpProgress = 1f;
                if (ExactPosition == vector4 || (!iceMode && WindUpPlayerCheck() == null)) {
                    debrisDirection = (vector4 - startPos).SafeNormalize();
                    state = States.BounceEnd;
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    moveSpeed = 0f;
                    bounceEndTimer = 0.05f;
                    ShakeOffPlayer(bounceLift);
                }
            } else if (state == States.BounceEnd) {
                bounceEndTimer -= Engine.DeltaTime;
                if (bounceEndTimer <= 0f) {
                    Break();
                }
            } else {
                if (state != States.Broken) {
                    return;
                }

                Depth = 8990;
                reformed = false;
                if (respawnTimer > 0f) {
                    respawnTimer -= Engine.DeltaTime;
                    return;
                }

                RemoveSelf();
            }
        }

        private Player WindUpPlayerCheck() {
            Player player = CollideFirst<Player>(Position - Vector2.UnitY);
            if (player != null && player.Speed.Y < 0f) {
                player = null;
            }

            if (player == null) {
                player = CollideFirst<Player>(Position + Vector2.UnitX);
                if (player == null || player.StateMachine.State != Player.StClimb || player.Facing != Facings.Left) {
                    player = CollideFirst<Player>(Position - Vector2.UnitX);
                    if (player == null || player.StateMachine.State != Player.StClimb || player.Facing != Facings.Right) {
                        player = null;
                    }
                }
            }

            return player;
        }

        private void ShakeOffPlayer(Vector2 liftSpeed) {
            Player player = WindUpPlayerCheck();
            if (player != null) {
                player.StateMachine.State = Player.StNormal;
                player.Speed = liftSpeed;
                player.StartJumpGraceTime();
            }
        }

        private void Break() {
            if (!iceMode) {
                Audio.Play("event:/game/09_core/bounceblock_break", Center);
            }

            Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            state = States.Broken;
            Collidable = false;
            DisableStaticMovers();
            respawnTimer = 1.6f;
            Vector2 direction = new(0f, 1f);
            if (!iceMode) {
                direction = debrisDirection;
            }

            Vector2 center = Center;
            for (int i = 0; i < Width; i += 8) {
                for (int j = 0; j < Height; j += 8) {
                    if (iceMode) {
                        direction = (new Vector2(X + i + 4f, Y + j + 4f) - center).SafeNormalize();
                    }

                    Scene.Add(Engine.Pooler.Create<BreakDebris>().Init(new Vector2(X + i + 4f, Y + j + 4f), direction, iceMode));
                }
            }

            float num = debrisDirection.Angle();
            Level level = SceneAs<Level>();
            for (int k = 0; k < Width; k += 4) {
                for (int l = 0; l < Height; l += 4) {
                    Vector2 vector = Position + new Vector2(2 + k, 2 + l) + Calc.Random.Range(-Vector2.One, Vector2.One);
                    float direction2 = iceMode ? (vector - center).Angle() : num;
                    level.Particles.Emit(iceMode ? BounceBlock.P_IceBreak : BounceBlock.P_FireBreak, vector, direction2);
                }
            }
        }

        [Pooled]
        public class RespawnDebris : Entity {
            private Image sprite;
            private Vector2 from;
            private Vector2 to;
            private float percent;
            private float duration;

            public RespawnDebris Init(Vector2 from, Vector2 to, bool ice, float duration) {
                List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
                MTexture texture = Calc.Random.Choose(atlasSubtextures);
                if (sprite == null) {
                    Add(sprite = new Image(texture));
                    sprite.CenterOrigin();
                } else {
                    sprite.Texture = texture;
                }

                Position = this.from = from;
                percent = 0f;
                this.to = to;
                this.duration = duration;
                return this;
            }

            public override void Update() {
                if (percent > 1f) {
                    RemoveSelf();
                    return;
                }

                percent += Engine.DeltaTime / duration;
                Position = Vector2.Lerp(from, to, Ease.CubeIn(percent));
                sprite.Color = Color.White * percent;
            }

            public override void Render() {
                sprite.DrawOutline(Color.Black);
                base.Render();
            }
        }

        [Pooled]
        public class BreakDebris : Entity {
            private Image sprite;
            private Vector2 speed;
            private float percent;
            private float duration;

            public BreakDebris Init(Vector2 position, Vector2 direction, bool ice) {
                List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
                MTexture texture = Calc.Random.Choose(atlasSubtextures);
                if (sprite == null) {
                    Add(sprite = new Image(texture));
                    sprite.CenterOrigin();
                } else {
                    sprite.Texture = texture;
                }

                Position = position;
                direction = Calc.AngleToVector(direction.Angle() + Calc.Random.Range(-0.1f, 0.1f), 1f);
                speed = direction * (ice ? Calc.Random.Range(20, 40) : Calc.Random.Range(120, 200));
                percent = 0f;
                duration = Calc.Random.Range(2, 3);
                return this;
            }

            public override void Update() {
                base.Update();
                if (percent >= 1f) {
                    RemoveSelf();
                    return;
                }

                Position += speed * Engine.DeltaTime;
                speed.X = Calc.Approach(speed.X, 0f, 180f * Engine.DeltaTime);
                speed.Y += 200f * Engine.DeltaTime;
                percent += Engine.DeltaTime / duration;
                sprite.Color = Color.White * (1f - percent);
            }

            public override void Render() {
                sprite.DrawOutline(Color.Black);
                base.Render();
            }
        }
    }
}
