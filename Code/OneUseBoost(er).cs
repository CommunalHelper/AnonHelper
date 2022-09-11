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
	[CustomEntity("Anonhelper/OneUseBooster")]
	public class OneUseBooster : Booster
	{

		public OneUseBooster(Vector2 position, bool red)
		: base(position, red)
		{
		}
		public OneUseBooster(EntityData data, Vector2 offset)
		: this(data.Position + offset, data.Bool("red"))
		{
		}
	}
}
