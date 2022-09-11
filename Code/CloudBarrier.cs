using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.Anonhelper {
    [Tracked]
    [CustomEntity("Anonhelper/CloudBarrier")]
    public class CloudBarrier : Solid {
        public float Flash;
        public float Solidify;
        public bool Flashing;

        private readonly List<Vector2> particles = new();
        private readonly float[] speeds = new float[3] { 12f, 20f, 40f };
        private float solidifyDelay;

        public CloudBarrier(Vector2 position, float width, float height)
            : base(position, width, height, safe: false) {
            Collidable = false;
            for (int i = 0; i < Width * Height / 16f; i++) {
                particles.Add(new Vector2(Calc.Random.NextFloat(Width - 1f), Calc.Random.NextFloat(Height - 1f)));
            }
        }

        public CloudBarrier(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height) {
        }

        public override void Update() {
            if (Flashing) {
                Flash = Calc.Approach(Flash, 0f, Engine.DeltaTime * 4f);
                if (Flash <= 0f) {
                    Flashing = false;
                }
            } else if (solidifyDelay > 0f) {
                solidifyDelay -= Engine.DeltaTime;
            } else if (Solidify > 0f) {
                Solidify = Calc.Approach(Solidify, 0f, Engine.DeltaTime);
            }

            int num = speeds.Length;
            float height = Height;
            int i = 0;
            for (int count = particles.Count; i < count; i++) {
                Vector2 value = particles[i] + (Vector2.UnitY * speeds[i % num] * Engine.DeltaTime);
                value.Y %= height - 1f;
                particles[i] = value;
            }
        }

        public override void Render() {
            Color color = Color.White * 0.5f;
            Draw.Rect(Collider, Color.DarkOrange * 0.25f);
            foreach (Vector2 particle in particles) {
                Draw.Pixel.Draw(Position + particle, Vector2.Zero, color);
            }

            if (Flashing) {
                Draw.Rect(Collider, Color.White * Flash * 0.5f);
            }
        }
    }
}
