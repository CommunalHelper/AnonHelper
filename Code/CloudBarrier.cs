using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Anonhelper;
using MonoMod.Utils;
using Celeste;

namespace Celeste.Mod.Anonhelper
{
	[Tracked]
    [CustomEntity("Anonhelper/CloudBarrier")]
    public class CloudBarrier:Solid
    {
		public float Flash;

		public float Solidify;

		public bool Flashing;

		private float solidifyDelay;

		private List<Vector2> particles = new List<Vector2>();

		private List<CloudBarrier> adjacent = new List<CloudBarrier>();

		private float[] speeds = new float[3]
		{
		12f,
		20f,
		40f
		};

		public CloudBarrier(Vector2 position, float width, float height)
			: base(position, width, height, safe: false)
		{
			Collidable = false;
			for (int i = 0; (float)i < base.Width * base.Height / 16f; i++)
			{
				particles.Add(new Vector2(Calc.Random.NextFloat(base.Width - 1f), Calc.Random.NextFloat(base.Height - 1f)));
			}
		}

		public CloudBarrier(EntityData data, Vector2 offset)
			: this(data.Position + offset, data.Width, data.Height)
		{
		}

		public override void Update()
		{
			if (Flashing)
			{
				Flash = Calc.Approach(Flash, 0f, Engine.DeltaTime * 4f);
				if (Flash <= 0f)
				{
					Flashing = false;
				}
			}
			else if (solidifyDelay > 0f)
			{
				solidifyDelay -= Engine.DeltaTime;
			}
			else if (Solidify > 0f)
			{
				Solidify = Calc.Approach(Solidify, 0f, Engine.DeltaTime);
			}
			int num = speeds.Length;
			float height = base.Height;
			int i = 0;
			for (int count = particles.Count; i < count; i++)
			{
				Vector2 value = particles[i] + Vector2.UnitY * speeds[i % num] * Engine.DeltaTime;
				value.Y %= height - 1f;
				particles[i] = value;
			}
		}


		public override void Render()
		{
			Color color = Color.White * 0.5f;
			Draw.Rect(Collider, Color.DarkOrange * 0.25f);
			foreach (Vector2 particle in particles)
			{
				Draw.Pixel.Draw(Position + particle, Vector2.Zero, color);
			}
			if (Flashing)
			{
				Draw.Rect(base.Collider, Color.White * Flash * 0.5f);
			}
		}
	}
}
