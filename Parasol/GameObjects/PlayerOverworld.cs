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
	class PlayerOverworld : Entity
	{
		//FSM enumerator
		public enum State
		{
			idle,
			walk
		}
		//directions
		private bool playerUp;
		private bool playerDown;
		private bool playerLeft;
		private bool playerRight;
		private bool playerMenu;

		public State PlayerState { get; set; }
		public AnimatedSprite objectAnimated;


		public PlayerOverworld(Vector2 initPosition)
		{
			position = initPosition;
			applyGravity = false;
		}

		public override void Initialize()
		{
			PlayerState = State.idle;
			objectType = "player";
			//health = HUD.playerMaxHealth;

			accel = 0.75f;
			friction = 1.2f;
			maxSpeed = 1.2f;

			base.Initialize();
		}

		public override void Load(ContentManager content)
		{
			#region  INITIALIZE TEXTURE ATLAS AND ANIMATION

			var spriteWidth = 16;
			var spriteHeight = 16;
			var objectTexture = content.Load<Texture2D>("Sprites/Player/s_player_overworld_atlas");
			var objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

			var animationFactory = new SpriteSheetAnimationFactory(objectAtlas);

			animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
			animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 0, 1 }, frameDuration: 0.2f, isLooping: true));

			#endregion


			objectAnimated = new AnimatedSprite(animationFactory, "idle");
			objectSprite = objectAnimated;

			base.Load(content);

			objectSprite.Origin = Vector2.Zero + origin;
			//set custom hitbox
			boundingBoxTopLeft = Vector2.Zero;
			boundingBoxBottomRight = new Vector2(16, 16);
		}

		public override void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{
			if (!Door.doorEnter)
			{
				//Check Input
				PlayerInput();

				//FSM Check
				StateMachine(PlayerState, objects, wallMap);

				base.Update(objects, wallMap, gametime);

				//are we in a door

				var testRectDoor = InDoor(wallMap);
				if (testRectDoor != Rectangle.Empty)
				{
					applyGravity = false;
				}
			}
			objectSprite.Position = position;
			objectAnimated.Update(gametime);
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
					PlayerState = State.idle;
					break;
			}
		}

		private void Idle(WallMap wallMap)
		{
			//stop moving
			velocity.X = TendToZero(velocity.X, friction);
			velocity.Y = TendToZero(velocity.X, friction);

			//switch to walk
			if (playerLeft || playerRight || playerUp || playerDown)
			{
				PlayerState = State.walk;
			}
		}

		private void Walk(WallMap wallMap)
		{
			//stop moving
			velocity.X = TendToZero(velocity.X, friction);
			velocity.Y = TendToZero(velocity.X, friction);

			if (playerLeft == true)
			{
				MoveLeft();
				objectAnimated.Effect = SpriteEffects.FlipHorizontally;
			}
			if (playerRight == true)
			{
				MoveRight();
				objectAnimated.Effect = SpriteEffects.None;
			}
			if (playerUp == true)
			{
				MoveUp();
				objectAnimated.Effect = SpriteEffects.None;
			}
			if (playerDown == true)
			{
				MoveDown();
				objectAnimated.Effect = SpriteEffects.None;
			}
			if (!playerLeft && !playerRight && !playerUp && !playerDown)
			{
				PlayerState = State.idle;
			}
		}

		protected Rectangle InDoor(WallMap wallMap)
		{
			Rectangle futureBoundingBox = new Rectangle((int)((BoundingBox.X)),
														(int)((BoundingBox.Y)),
														(int)(BoundingBox.Width),
														(int)(BoundingBox.Height));


			return wallMap.CheckForDoor(futureBoundingBox);
		}

		#region PLAYER INPUTS

		private void PlayerInput()
		{
			Input.Update();

			// check for directional movement
			playerLeft = Input.IsKeyDown(Keys.Left) == true || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed;
			playerRight = Input.IsKeyDown(Keys.Right) == true || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed;

			playerUp = Input.IsKeyDown(Keys.Up) == true || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed;
			playerDown = Input.IsKeyDown(Keys.Down) == true || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed;


			//check for button presses
			
			playerMenu = Input.KeyPressed(Keys.Enter) == true || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed;
		}


		#endregion

	}
}
