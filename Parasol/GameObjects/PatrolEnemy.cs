using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using System;

namespace Parasol
{
	class PatrolEnemy : Enemy
	{
		private float previousX;
		 
		public PatrolEnemy()
		{}

		public PatrolEnemy(Vector2 initPosition)
		{
			position = initPosition;
		}

		//FSM enumerator
		public enum State
		{
			idle,
			walk
		}

		public State EnemyState { get; set; }


		public override void Initialize()
		{
			EnemyState = State.walk;
			health = 3;

			accel = 0.1f;
			friction = 0f;
			maxSpeed = 0.9f;
			knockback = new Vector2(3, -2);

			base.Initialize();
		}

		public override void Load(ContentManager content)
		{
			#region  INITIALIZE TEXTURE ATLAS AND ANIMATION

			var spriteWidth = 54;
			var spriteHeight = 35;
			var objectTexture = content.Load<Texture2D>("Sprites/Player/s_player_atlas");
			var objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

			var animationFactory = new SpriteSheetAnimationFactory(objectAtlas);

			animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
			animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 0, 1 }, frameDuration: 0.2f, isLooping: true));

			#endregion


			objectAnimated = new AnimatedSprite(animationFactory, "walk");
			objectSprite = objectAnimated;

			base.Load(content);

			objectSprite.Origin = Vector2.Zero + origin;
			objectAnimated.Color = new Color(255, 255, 255, 255);

			//set custom hitbox
			//set custom hitbox
			boundingBoxTopLeft = new Vector2(22, 6);
			boundingBoxBottomRight = new Vector2(33, 35);
		}

		public override void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{
			if (HUD.canMove)
			{
				//FSM Check
				StateMachine(EnemyState, objects, wallMap);

				base.Update(objects, wallMap, gametime);

				if (OneEdgeOfGround(wallMap) == Rectangle.Empty || position.X == previousX)
				{
					velocity.X = 0f;
					direction = -direction;
				}

				if (health <= 0)
				{
					objects.Remove(this);
				}
			}
			objectSprite.Position = position;
			objectAnimated.Update(gametime);
			previousX = position.X;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}

		private void StateMachine(State playerState, List<GameObject> objects, WallMap wallMap)
		{
			switch (playerState)
			{
				case State.idle:
					Idle(wallMap);
					objectAnimated.Play("idle");
					break;
				case State.walk:
					Walk(wallMap);
					objectAnimated.Play("walk");
					break;
				default:
					EnemyState = State.idle;
					break;
			}
		}

		private void Idle(WallMap wallMap)
		{
			//stop moving
			velocity.X = TendToZero(velocity.X, friction);
			velocity.Y = TendToZero(velocity.X, friction);

			//switch to walk

		}

		private void Walk(WallMap wallMap)
		{
			//stop moving
			if (direction.X == 1)
			{
				MoveRight();
				objectAnimated.Effect = SpriteEffects.None;
			}
			else
			{
				MoveLeft();
				objectAnimated.Effect = SpriteEffects.FlipHorizontally;
			}

			velocity.X = TendToZero(velocity.X, friction);


		}

		protected Rectangle OneEdgeOfGround(WallMap wallMap)
		{
			if (direction.X == 1)
			{
				Rectangle futureBoundingBox = new Rectangle((int)((BoundingBox.X + BoundingBox.Width)),
															(int)((BoundingBox.Y + 1)),
															(int)(BoundingBox.Width),
															(int)(BoundingBox.Height));
				return wallMap.CheckCollision(futureBoundingBox);

			} 
			else
			{
				Rectangle futureBoundingBox = new Rectangle((int)((BoundingBox.X - BoundingBox.Width)),
																(int)((BoundingBox.Y) + (1)),
																(int)(BoundingBox.Width),
																(int)(BoundingBox.Height));
				return wallMap.CheckCollision(futureBoundingBox);
			}
			
		}
	}
}

