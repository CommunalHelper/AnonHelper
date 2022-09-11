using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.Entities;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Anonhelper;
using MonoMod.Utils;
using MonoMod;
using Celeste;

namespace Celeste.Mod.Anonhelper
{
    [CustomEntity("Anonhelper/InvisibleSeekerBarrier")]
    [TrackedAs(typeof(SeekerBarrier))]
    public class InvisibleSeekerBarrier : SeekerBarrier
    {
        public Hitbox HideCollider;

        public Hitbox RegCollider;

        public InvisibleSeekerBarrier(Vector2 position, float width, float height)
		: base(position, width, height)
        { Visible = false;
            RegCollider = new Hitbox(width, height);
            HideCollider = new Hitbox(0f, height);

            base.Collider = RegCollider;

        }
        public InvisibleSeekerBarrier(EntityData data, Vector2 offset)
       : this(data.Position + offset, data.Width, data.Height)
        {
        }


        public override void Added(Scene scene)
        {
            base.Added(scene);
            scene.Tracker.GetEntity<SeekerBarrierRenderer>().Untrack(this);
        }

        public override void Render()
        {

        }


    }
}

