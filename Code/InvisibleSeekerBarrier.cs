using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Anonhelper {
    [CustomEntity("Anonhelper/InvisibleSeekerBarrier")]
    [TrackedAs(typeof(SeekerBarrier))]
    public class InvisibleSeekerBarrier : SeekerBarrier {
        public Hitbox HideCollider;
        public Hitbox RegCollider;

        public InvisibleSeekerBarrier(Vector2 position, float width, float height) 
            : base(position, width, height) {
            Visible = false;
            RegCollider = new Hitbox(width, height);
            HideCollider = new Hitbox(0f, height);
            Collider = RegCollider;
        }

        public InvisibleSeekerBarrier(EntityData data, Vector2 offset) 
            : this(data.Position + offset, data.Width, data.Height) { 
        }

        public static void Load() {
            On.Celeste.Level.Render += Level_Render;
        }

        public static void Unload() {
            On.Celeste.Level.Render -= Level_Render;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            scene.Tracker.GetEntity<SeekerBarrierRenderer>().Untrack(this);
        }

        private static void Level_Render(On.Celeste.Level.orig_Render orig, Level self) {
            foreach (Entity e in self.Tracker.GetEntities<SeekerBarrier>()) {
                if (e is InvisibleSeekerBarrier barrier) {
                    e.Collider = barrier.HideCollider;
                }
            }

            orig.Invoke(self);
            foreach (Entity e in self.Tracker.GetEntities<SeekerBarrier>()) {
                if (e is InvisibleSeekerBarrier barrier) {
                    e.Collider = barrier.RegCollider;
                }
            }
        }
    }
}

