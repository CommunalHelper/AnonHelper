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
using On.Celeste;

namespace Celeste.Mod.Anonhelper
{
    [CustomEntity("Anonhelper/FeatherBumper")]
    public class FeatherBumper : Entity
    {

		private const float RespawnTime = 0.6f;

		private const float MoveCycleTime = 1.81818187f;

		private const float SineCycleFreq = 0.44f;

		private Sprite sprite;

		private VertexLight light;

		private BloomPoint bloom;

		private Vector2? node;

		private bool goBack;

		private Vector2 anchor;

		private SineWave sine;

		private float respawnTimer;

		private bool fireMode;

		private Wiggler hitWiggler;

		private Vector2 hitDir;

		private bool canWobble=true;

		private bool bumperHit;

		public FeatherBumper(Vector2 position, Vector2? node)
			: base(position)
		{
			base.Collider = new Circle(12f);
			Add(new PlayerCollider(OnPlayer));
			Add(sine = new SineWave(0.44f, 0f).Randomize());
			Add(sprite = Anonhelper.AnonModule.spriteBank.Create("featherbumper"));
			Add(light = new VertexLight(Color.Teal, 1f, 16, 32));
			Add(bloom = new BloomPoint(.5f, 1f));
			this.node = node;
			anchor = Position;
			if (node.HasValue)
			{
				Vector2 start = Position;
				Vector2 end = node.Value;
				Tween tween = Tween.Create(Tween.TweenMode.Looping, Ease.CubeInOut, 1.81818187f, start: true);
				tween.OnUpdate = delegate (Tween t)
				{
					if (goBack)
					{
						anchor = Vector2.Lerp(end, start, t.Eased);
					}
					else
					{
						anchor = Vector2.Lerp(start, end, t.Eased);
					}
				};
				tween.OnComplete = delegate
				{
					goBack = !goBack;
				};
				Add(tween);
			}
			UpdatePosition();
		}

		public FeatherBumper(EntityData data, Vector2 offset)
			: this(data.Position + offset, data.FirstNodeNullable(offset))
		{
			canWobble = data.Bool("Wobble", false);
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			sprite.Visible = !fireMode;
		}

		private void UpdatePosition()
		{
			if (!canWobble) Position = anchor;
			else Position = anchor + new Vector2(sine.Value * 3f, sine.ValueOverTwo * 2f);
		}

		public override void Update()
		{
			base.Update();
			if (respawnTimer > 0f)
			{
				respawnTimer -= Engine.DeltaTime;
				if (respawnTimer <= 0f)
				{
					light.Visible = true;
					bloom.Visible = true;
					sprite.Play("on");
					Audio.Play("event:/game/06_reflection/pinballbumper_reset", Position);
				}
			}
			else if (base.Scene.OnInterval(0.05f))
			{
				float num = Calc.Random.NextAngle();
				ParticleType type = FlyFeather.P_Flying;
				float direction = num;
				float length = 8;
				SceneAs<Level>().Particles.Emit(type, 1, base.Center + Calc.AngleToVector(num, length), Vector2.One * 2f, direction);
			}
			UpdatePosition();
			bumperHit = false;
		}

		private IEnumerator bumperDelay(Player player)
		{
			yield return .4f;
			player.StateMachine.State = 19;
		}
		private void OnPlayer(Player player)
		{
			if (respawnTimer <= 0f)
			{
				Audio.Play("event:/game/06_reflection/pinballbumper_hit", Position);
				respawnTimer = 0.6f;
				Vector2 vector2 = player.ExplodeLaunch(Position, snapUp: false, sidesOnly: false);
				sprite.Play("hit", restart: true);
				bumperHit = true;
				light.Visible = false;
				bloom.Visible = false;
				SceneAs<Level>().DirectionalShake(vector2, 0.15f);
				SceneAs<Level>().Displacement.AddBurst(base.Center, 0.3f, 8f, 32f, 0.8f);
				SceneAs<Level>().Particles.Emit(FlyFeather.P_Collect, 12, base.Center + vector2 * 12f, Vector2.One * 3f, vector2.Angle());
				if (bumperHit)
				{
					Add(new Coroutine(bumperDelay(player))); 


				}
			}
		}
	}
}
