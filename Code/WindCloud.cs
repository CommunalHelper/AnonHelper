using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod;
using System;
using System.Collections;

namespace Celeste.Mod.Anonhelper {
    [CustomEntity("Anonhelper/WindCloud")]
    public class WindCloud : AnonCloud {
        public static ParticleType P_WindCloud = new() {
            Source = GFX.Game["particles/cloud"],
            Color = Calc.HexToColor("b0500b"),
            FadeMode = ParticleType.FadeModes.None,
            LifeMin = 0.25f,
            LifeMax = 0.3f,
            Size = 0.7f,
            SizeRange = 0.25f,
            ScaleOut = true,
            Direction = 4.712389f,
            DirectionRange = 0.17453292f,
            SpeedMin = 10f,
            SpeedMax = 20f,
            SpeedMultiplier = 0.01f,
            Acceleration = new Vector2(0f, 90f)
        };

        public static ParticleType P_FragileWindCloud = new() {
            Source = GFX.Game["particles/cloud"],
            Color = Calc.HexToColor("9c3310"),
            FadeMode = ParticleType.FadeModes.None,
            LifeMin = 0.25f,
            LifeMax = 0.3f,
            Size = 0.7f,
            SizeRange = 0.25f,
            ScaleOut = true,
            Direction = 4.712389f,
            DirectionRange = 0.17453292f,
            SpeedMin = 10f,
            SpeedMax = 20f,
            SpeedMultiplier = 0.01f,
            Acceleration = new Vector2(0f, 90f)
        };
        public WindCloud(Vector2 position, bool fragile, bool small)
            : base(position, false, fragile, small) {
            Small = small;
            Add(new WindMover(WindMove));
            particleType = fragile ? P_FragileWindCloud : P_WindCloud;
        }

        public WindCloud(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("fragile"), data.Bool("small")) { }

        private void WindMove(Vector2 wind) {
            if (waiting) {
                if (wind.X != 0f) {
                    MoveH(wind.X * 1f);
                }

                if (wind.Y != 0f) {
                    MoveV(wind.Y * 1f);
                }
            }
        }

        [MonoModLinkTo("Celeste.JumpThru", "System.Void Update")]
        private extern void JumpThru_Update();

        public override void Update() {
            JumpThru_Update();
            scale.X = Calc.Approach(scale.X, 1f, 1f * Engine.DeltaTime);
            scale.Y = Calc.Approach(scale.Y, 1f, 1f * Engine.DeltaTime);
            timer += Engine.DeltaTime;
            sprite.Position = GetPlayerRider() != null ? Vector2.Zero
                : Calc.Approach(sprite.Position, new Vector2(0f, (float)Math.Sin(timer * 2f)), Engine.DeltaTime * 4f);
            if (respawnTimer > 0f) {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f) {
                    waiting = true;
                    Y = startY;
                    speed = 0f;
                    scale = Vector2.One;
                    Collidable = true;
                    sprite.Play("spawn");
                    sfx.Play("event:/game/04_cliffside/cloud_pink_reappear");
                }

                return;
            }

            if (waiting) {
                Player playerRider = GetPlayerRider();
                if (playerRider != null && playerRider.Speed.Y >= 0f) {
                    canRumble = true;
                    speed = 180f;
                    scale = new Vector2(1.3f, 0.7f);
                    waiting = false;
                    startY = Y;
                    if (pink) {
                        Audio.Play("event:/game/04_cliffside/cloud_pink_boost", Position);
                    } else {
                        Audio.Play("event:/game/04_cliffside/cloud_blue_boost", Position);
                    }
                }

                return;
            }

            if (returning) {
                speed = Calc.Approach(speed, 180f, 600f * Engine.DeltaTime);
                MoveTowardsY(startY, speed * Engine.DeltaTime);
                if (ExactPosition.Y == startY) {
                    returning = false;
                    waiting = true;
                    speed = 0f;
                }

                return;
            }

            if (Collidable && !HasPlayerRider() && fragile) {
                Collidable = false;
                sprite.Play("fade");
            }

            if (speed < 0f && canRumble) {
                canRumble = false;
                if (HasPlayerRider()) {
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                }
            }

            if (speed < 0f && Scene.OnInterval(0.02f)) {
                (Scene as Level).ParticlesBG.Emit(particleType, 1, Position + new Vector2(0f, 2f), new Vector2(Collider.Width / 2f, 1f), (float)Math.PI / 2f);
            }

            if (speed < 0f) {
                sprite.Scale.Y = Calc.Approach(sprite.Scale.Y, 0f, Engine.DeltaTime * 4f);
            }

            if (Y >= startY) {
                speed -= 1200f * Engine.DeltaTime;
            } else {
                speed += 1200f * Engine.DeltaTime;
                if (speed >= -100f) {
                    Player playerRider2 = GetPlayerRider();
                    if (playerRider2 != null && playerRider2.Speed.Y >= 0f) {
                        playerRider2.Speed.Y = -200f;
                    }

                    if (fragile) {
                        Collidable = false;
                        sprite.Play("fade");

                    } else {
                        scale = new Vector2(0.7f, 1.3f);
                        returning = true;
                    }
                }
            }

            float num = speed;
            if (num < 0f) {
                num = -220f;
            }

            MoveV(speed * Engine.DeltaTime, num);
            foreach (CloudBarrier entity in Scene.Tracker.GetEntities<CloudBarrier>()) {
                entity.Collidable = true;
                bool collide = CollideCheck(entity);
                entity.Collidable = false;
                if (collide) {
                    Collidable = false;
                    Add(new Coroutine(DestroyAnimationRoutine()));
                    return;
                }
            }
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            string text = fragile ? "windfragile" : "windcloud";
            if (Small) {
                Collider.Position.X += 2f;
                Collider.Width -= 6f;
                text += "Remix";
            }

            Remove(sprite);
            Add(sprite = AnonModule.spriteBank.Create(text));
            sprite.Origin = new Vector2(sprite.Width / 2f, 8f);
            sprite.OnFrameChange = delegate (string s) {
                if (s == "spawn" && sprite.CurrentAnimationFrame == 6) {
                    wiggler.Start();
                }

                if (s == "fade" && sprite.CurrentAnimationFrame == 4 && fragile) {
                    RemoveSelf();
                }
            };
        }

        public IEnumerator DestroyAnimationRoutine() {
            Audio.Play("event:/game/04_cliffside/cloud_pink_boost", Position);
            sprite.Play("fade");
            yield return 1f;
            RemoveSelf();
        }
    }
}
