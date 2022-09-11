using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.Anonhelper {
    [CustomEntity("Anonhelper/FeatherBumper")]
    public class FeatherBumper : Entity {
        private readonly Sprite sprite;
        private readonly VertexLight light;
        private readonly BloomPoint bloom;
        private readonly SineWave sine;
        private readonly bool canWobble = true;
        private bool goBack;
        private Vector2 anchor;
        private float respawnTimer;
        private bool bumperHit;

        public FeatherBumper(Vector2 position, Vector2? node)
            : base(position) {
            Collider = new Circle(12f);
            Add(new PlayerCollider(OnPlayer));
            Add(sine = new SineWave(0.44f, 0f).Randomize());
            Add(sprite = AnonModule.spriteBank.Create("featherbumper"));
            Add(light = new VertexLight(Color.Teal, 1f, 16, 32));
            Add(bloom = new BloomPoint(.5f, 1f));
            anchor = Position;
            if (node.HasValue) {
                Vector2 start = Position;
                Vector2 end = node.Value;
                Tween tween = Tween.Create(Tween.TweenMode.Looping, Ease.CubeInOut, 1.81818187f, start: true);
                tween.OnUpdate = delegate (Tween t) {
                    anchor = goBack ? Vector2.Lerp(end, start, t.Eased) : Vector2.Lerp(start, end, t.Eased);
                };
                tween.OnComplete = delegate {
                    goBack = !goBack;
                };
                Add(tween);
            }

            UpdatePosition();
        }

        public FeatherBumper(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.FirstNodeNullable(offset)) {
            canWobble = data.Bool("Wobble", false);
        }

        private void UpdatePosition() {
            Position = !canWobble ? anchor : anchor + new Vector2(sine.Value * 3f, sine.ValueOverTwo * 2f);
        }

        public override void Update() {
            base.Update();
            if (respawnTimer > 0f) {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f) {
                    light.Visible = true;
                    bloom.Visible = true;
                    sprite.Play("on");
                    Audio.Play("event:/game/06_reflection/pinballbumper_reset", Position);
                }
            } else if (Scene.OnInterval(0.05f)) {
                float num = Calc.Random.NextAngle();
                ParticleType type = FlyFeather.P_Flying;
                float direction = num;
                float length = 8;
                SceneAs<Level>().Particles.Emit(type, 1, Center + Calc.AngleToVector(num, length), Vector2.One * 2f, direction);
            }

            UpdatePosition();
            bumperHit = false;
        }

        private IEnumerator BumperDelay(Player player) {
            yield return 0.4f;
            player.StateMachine.State = Player.StStarFly;
        }
        private void OnPlayer(Player player) {
            if (respawnTimer <= 0f) {
                Audio.Play("event:/game/06_reflection/pinballbumper_hit", Position);
                respawnTimer = 0.6f;
                Vector2 vector2 = player.ExplodeLaunch(Position, snapUp: false, sidesOnly: false);
                sprite.Play("hit", restart: true);
                bumperHit = true;
                light.Visible = false;
                bloom.Visible = false;
                SceneAs<Level>().DirectionalShake(vector2, 0.15f);
                SceneAs<Level>().Displacement.AddBurst(Center, 0.3f, 8f, 32f, 0.8f);
                SceneAs<Level>().Particles.Emit(FlyFeather.P_Collect, 12, Center + (vector2 * 12f), Vector2.One * 3f, vector2.Angle());
                if (bumperHit) {
                    Add(new Coroutine(BumperDelay(player)));

                }
            }
        }
    }
}
