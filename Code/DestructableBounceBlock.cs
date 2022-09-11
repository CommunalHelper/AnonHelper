﻿using System;
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

namespace Anonhelper
{
    [CustomEntity("Anonhelper/DestructableBounceBlock")]
    public class DestructableBounceBlock : Solid
    {
		private enum States
		{
			Waiting,
			WindingUp,
			Bouncing,
			BounceEnd,
			Broken
		}

		[Pooled]
		private class RespawnDebris : Entity
		{
			private Image sprite;

			private Vector2 from;

			private Vector2 to;

			private float percent;

			private float duration;

			public RespawnDebris Init(Vector2 from, Vector2 to, bool ice, float duration)
			{
				List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
				MTexture texture = Calc.Random.Choose(atlasSubtextures);
				if (sprite == null)
				{
					Add(sprite = new Image(texture));
					sprite.CenterOrigin();
				}
				else
				{
					sprite.Texture = texture;
				}
				Position = (this.from = from);
				percent = 0f;
				this.to = to;
				this.duration = duration;
				return this;
			}

			public override void Update()
			{
				if (percent > 1f)
				{
					RemoveSelf();
					return;
				}
				percent += Engine.DeltaTime / duration;
				Position = Vector2.Lerp(from, to, Ease.CubeIn(percent));
				sprite.Color = Color.White * percent;
			}

			public override void Render()
			{
				sprite.DrawOutline(Color.Black);
				base.Render();
			}
		}

		[Pooled]
		private class BreakDebris : Entity
		{
			private Image sprite;

			private Vector2 speed;

			private float percent;

			private float duration;

			public BreakDebris Init(Vector2 position, Vector2 direction, bool ice)
			{
				List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(ice ? "objects/bumpblocknew/ice_rubble" : "objects/bumpblocknew/fire_rubble");
				MTexture texture = Calc.Random.Choose(atlasSubtextures);
				if (sprite == null)
				{
					Add(sprite = new Image(texture));
					sprite.CenterOrigin();
				}
				else
				{
					sprite.Texture = texture;
				}
				Position = position;
				direction = Calc.AngleToVector(direction.Angle() + Calc.Random.Range(-0.1f, 0.1f), 1f);
				speed = direction * (ice ? Calc.Random.Range(20, 40) : Calc.Random.Range(120, 200));
				percent = 0f;
				duration = Calc.Random.Range(2, 3);
				return this;
			}

			public override void Update()
			{
				base.Update();
				if (percent >= 1f)
				{
					RemoveSelf();
					return;
				}
				Position += speed * Engine.DeltaTime;
				speed.X = Calc.Approach(speed.X, 0f, 180f * Engine.DeltaTime);
				speed.Y += 200f * Engine.DeltaTime;
				percent += Engine.DeltaTime / duration;
				sprite.Color = Color.White * (1f - percent);
			}

			public override void Render()
			{
				sprite.DrawOutline(Color.Black);
				base.Render();
			}
		}

		public static ParticleType P_Reform= BounceBlock.P_Reform;

		public static ParticleType P_FireBreak=BounceBlock.P_FireBreak;

		public static ParticleType P_IceBreak=BounceBlock.P_IceBreak;

		private const float WindUpDelay = 0f;

		private const float WindUpDist = 10f;

		private const float IceWindUpDist = 16f;

		private const float BounceDist = 24f;

		private const float LiftSpeedXMult = 0.75f;

		private const float RespawnTime = 1.6f;

		private const float WallPushTime = 0.1f;

		private const float BounceEndTime = 0.05f;

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

		private List<Image> hotImages;

		private List<Image> coldImages;

		private Sprite hotCenterSprite;

		private Sprite coldCenterSprite;

		private bool notCoreMode;

		public DestructableBounceBlock(Vector2 position, float width, float height) : base(position, width, height, safe: false)
		{
			reformed = true;
			state = States.Waiting;
			startPos = Position;
			hotImages = BuildSprite(GFX.Game["objects/bumpblocknew/fire00"]);
			hotCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterFire");
			hotCenterSprite.Position = new Vector2(base.Width, base.Height) / 2f;
			hotCenterSprite.Visible = false;
			Add(hotCenterSprite);
			coldImages = BuildSprite(GFX.Game["objects/bumpblocknew/ice00"]);
			coldCenterSprite = GFX.SpriteBank.Create("bumpBlockCenterIce");
			coldCenterSprite.Position = new Vector2(base.Width, base.Height) / 2f;
			coldCenterSprite.Visible = false;
			Add(coldCenterSprite);
			Add(new CoreModeListener(OnChangeMode));
		}

		public DestructableBounceBlock(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height)
		{
			notCoreMode = data.Bool("notCoreMode");
		}

		private List<Image> BuildSprite(MTexture source)
		{
			List<Image> list = new List<Image>();
			int num = source.Width / 8;
			int num2 = source.Height / 8;
			for (int i = 0; (float)i < base.Width; i += 8)
			{
				for (int j = 0; (float)j < base.Height; j += 8)
				{
					int num3 = (i != 0) ? ((!((float)i >= base.Width - 8f)) ? Calc.Random.Next(1, num - 1) : (num - 1)) : 0;
					int num4 = (j != 0) ? ((!((float)j >= base.Height - 8f)) ? Calc.Random.Next(1, num2 - 1) : (num2 - 1)) : 0;
					Image image = new Image(source.GetSubtexture(num3 * 8, num4 * 8, 8, 8));
					image.Position = new Vector2(i, j);
					list.Add(image);
					Add(image);
				}
			}
			return list;
		}
		private void ToggleSprite()
		{
			hotCenterSprite.Visible = !iceMode;
			coldCenterSprite.Visible = iceMode;
			foreach (Image hotImage in hotImages)
			{
				hotImage.Visible = !iceMode;
			}
			foreach (Image coldImage in coldImages)
			{
				coldImage.Visible = iceMode;
			}
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			iceModeNext = (iceMode = (SceneAs<Level>().CoreMode == Session.CoreModes.Cold || notCoreMode));
			ToggleSprite();
		}

		private void OnChangeMode(Session.CoreModes coreMode)
		{
			iceModeNext = (coreMode == Session.CoreModes.Cold);
		}

		private void CheckModeChange()
		{
			if (iceModeNext != iceMode)
			{
				iceMode = iceModeNext;
				ToggleSprite();
			}
		}

		public override void Render()
		{
			Vector2 position = Position;
			Position += base.Shake;
			if (state != States.Broken && reformed)
			{
				base.Render();
			}
			if (reappearFlash > 0f)
			{
				float num = Ease.CubeOut(reappearFlash);
				float num2 = num * 2f;
				Draw.Rect(base.X - num2, base.Y - num2, base.Width + num2 * 2f, base.Height + num2 * 2f, Color.White * num);
			}
			Position = position;
		}

		public override void Update()
		{
			base.Update();
			reappearFlash = Calc.Approach(reappearFlash, 0f, Engine.DeltaTime * 8f);
			if (state == States.Waiting)
			{
				CheckModeChange();
				moveSpeed = Calc.Approach(moveSpeed, 100f, 400f * Engine.DeltaTime);
				Vector2 vector = Calc.Approach(base.ExactPosition, startPos, moveSpeed * Engine.DeltaTime);
				Vector2 liftSpeed = (vector - base.ExactPosition).SafeNormalize(moveSpeed);
				liftSpeed.X *= 0.75f;
				MoveTo(vector, liftSpeed);
				windUpProgress = Calc.Approach(windUpProgress, 0f, 1f * Engine.DeltaTime);
				Player player = WindUpPlayerCheck();
				if (player != null)
				{
					moveSpeed = 80f;
					windUpStartTimer = 0f;
					if (iceMode)
					{
						bounceDir = -Vector2.UnitY;
					}
					else
					{
						bounceDir = (player.Center - base.Center).SafeNormalize();
					}
					state = States.WindingUp;
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
					if (iceMode)
					{
						StartShaking(0.2f);
						Audio.Play("event:/game/09_core/iceblock_touch", base.Center);
					}
					else
					{
						Audio.Play("event:/game/09_core/bounceblock_touch", base.Center);
					}
				}
			}
			else if (state == States.WindingUp)
			{
				Player player2 = WindUpPlayerCheck();
				if (player2 != null)
				{
					if (iceMode)
					{
						bounceDir = -Vector2.UnitY;
					}
					else
					{
						bounceDir = (player2.Center - base.Center).SafeNormalize();
					}
				}
				if (windUpStartTimer > 0f)
				{
					windUpStartTimer -= Engine.DeltaTime;
					windUpProgress = Calc.Approach(windUpProgress, 0f, 1f * Engine.DeltaTime);
					return;
				}
				moveSpeed = Calc.Approach(moveSpeed, iceMode ? 35f : 40f, 600f * Engine.DeltaTime);
				float num = iceMode ? 0.333f : 1f;
				Vector2 vector2 = startPos - bounceDir * (iceMode ? 16f : 10f);
				Vector2 vector3 = Calc.Approach(base.ExactPosition, vector2, moveSpeed * num * Engine.DeltaTime);
				Vector2 liftSpeed2 = (vector3 - base.ExactPosition).SafeNormalize(moveSpeed * num);
				liftSpeed2.X *= 0.75f;
				MoveTo(vector3, liftSpeed2);
				windUpProgress = Calc.ClampedMap(Vector2.Distance(base.ExactPosition, vector2), 16f, 2f);
				if (iceMode && Vector2.DistanceSquared(base.ExactPosition, vector2) <= 12f)
				{
					StartShaking(0.1f);
				}
				else if (!iceMode && windUpProgress >= 0.5f)
				{
					StartShaking(0.1f);
				}
				if (Vector2.DistanceSquared(base.ExactPosition, vector2) <= 2f)
				{
					if (iceMode)
					{
						Break();
					}
					else
					{
						state = States.Bouncing;
					}
					moveSpeed = 0f;
				}
			}
			else if (state == States.Bouncing)
			{
				moveSpeed = Calc.Approach(moveSpeed, 140f, 800f * Engine.DeltaTime);
				Vector2 vector4 = startPos + bounceDir * 24f;
				Vector2 vector5 = Calc.Approach(base.ExactPosition, vector4, moveSpeed * Engine.DeltaTime);
				bounceLift = (vector5 - base.ExactPosition).SafeNormalize(Math.Min(moveSpeed * 3f, 200f));
				bounceLift.X *= 0.75f;
				MoveTo(vector5, bounceLift);
				windUpProgress = 1f;
				if (base.ExactPosition == vector4 || (!iceMode && WindUpPlayerCheck() == null))
				{
					debrisDirection = (vector4 - startPos).SafeNormalize();
					state = States.BounceEnd;
					Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
					moveSpeed = 0f;
					bounceEndTimer = 0.05f;
					ShakeOffPlayer(bounceLift);
				}
			}
			else if (state == States.BounceEnd)
			{
				bounceEndTimer -= Engine.DeltaTime;
				if (bounceEndTimer <= 0f)
				{
					Break();
				}
			}
			else
			{
				if (state != States.Broken)
				{
					return;
				}
				base.Depth = 8990;
				reformed = false;
				if (respawnTimer > 0f)
				{
					respawnTimer -= Engine.DeltaTime;
					return;
				} RemoveSelf();
			}
		}

		private Player WindUpPlayerCheck()
		{
			Player player = CollideFirst<Player>(Position - Vector2.UnitY);
			if (player != null && player.Speed.Y < 0f)
			{
				player = null;
			}
			if (player == null)
			{
				player = CollideFirst<Player>(Position + Vector2.UnitX);
				if (player == null || player.StateMachine.State != 1 || player.Facing != Facings.Left)
				{
					player = CollideFirst<Player>(Position - Vector2.UnitX);
					if (player == null || player.StateMachine.State != 1 || player.Facing != Facings.Right)
					{
						player = null;
					}
				}
			}
			return player;
		}

		private void ShakeOffPlayer(Vector2 liftSpeed)
		{
			Player player = WindUpPlayerCheck();
			if (player != null)
			{
				player.StateMachine.State = 0;
				player.Speed = liftSpeed;
				player.StartJumpGraceTime();
			}
		}

		private void Break()
		{
			if (!iceMode)
			{
				Audio.Play("event:/game/09_core/bounceblock_break", base.Center);
			}
			Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
			state = States.Broken;
			Collidable = false;
			DisableStaticMovers();
			respawnTimer = 1.6f;
			Vector2 direction = new Vector2(0f, 1f);
			if (!iceMode)
			{
				direction = debrisDirection;
			}
			Vector2 center = base.Center;
			for (int i = 0; (float)i < base.Width; i += 8)
			{
				for (int j = 0; (float)j < base.Height; j += 8)
				{
					if (iceMode)
					{
						direction = (new Vector2(base.X + (float)i + 4f, base.Y + (float)j + 4f) - center).SafeNormalize();
					}
					base.Scene.Add(Engine.Pooler.Create<BreakDebris>().Init(new Vector2(base.X + (float)i + 4f, base.Y + (float)j + 4f), direction, iceMode));
				}
			}
			float num = debrisDirection.Angle();
			Level level = SceneAs<Level>();
			for (int k = 0; (float)k < base.Width; k += 4)
			{
				for (int l = 0; (float)l < base.Height; l += 4)
				{
					Vector2 vector = Position + new Vector2(2 + k, 2 + l) + Calc.Random.Range(-Vector2.One, Vector2.One);
					float direction2 = iceMode ? (vector - center).Angle() : num;
					level.Particles.Emit(iceMode ? P_IceBreak : P_FireBreak, vector, direction2);
				}
			}
		}
	}
}
