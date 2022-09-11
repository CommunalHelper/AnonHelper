using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Anonhelper {
    [CustomEntity("Anonhelper/OneUseBooster")]
    public class OneUseBooster : Booster {
        public OneUseBooster(Vector2 position, bool red)
            : base(position, red) {
        }

        public OneUseBooster(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("red")) {
        }
    }
}
