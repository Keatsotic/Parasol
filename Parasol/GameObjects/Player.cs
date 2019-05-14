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
	public class Player : Entity
	{
		//FSM enumerator
		public enum State
		{
			idle,
			walk, 
			jump,
			fall,
			attack,
			jumpAttack,
			duck, 
			duckAttack,
			hurt,
			stairs,
			dead
		}

		//player inputs
		//directions
		private bool playerUp;
		private bool playerDown;
		private bool playerLeft;
		private bool playerRight;

		//actions
		private bool playerAttack;
		private bool playerJump;
		private bool playerSpecial;
		private bool playerMenu;

		public State PlayerState { get; set; }
		public AnimatedSprite objectAnimated;
		private bool stopAnimating = false;

		private int attackTimer;
		private const int maxTimer = 10;
	

		private bool isOnStairs = false;
		private bool jumpOnStairs = false;
		private bool centered = false;
		private const float stairSpeed = 0.5f;

		private Vector2 standingBoundingBox = new Vector2(22, 6);
		private Vector2 duckBoundingBox = new Vector2(22, 12);

		static DamageObject damageObject = new DamageObject();

		private SoundEffect attackSFX;
		//private SoundEffect hurtSFX;
		//private SoundEffect deadSFX;


		public Player()
		{}

		public Player(Vector2 initPosition)
		{
			position = initPosition;
			damageObject = new DamageObject();
		}

		public override void Initialize()
		{
			PlayerState = State.idle;
			objectType = "player";
			health = HUD.playerMaxHealth;

			if (Door.transitionDirection == "StairTransition") { isOnStairs = true; PlayerState = State.stairs; }
			accel = 0.001f;
			friction = 0.4f;
			maxSpeed = 1.3f;


			damageObject.Initialize();


			base.Initialize();
		}

		public override void Load(ContentManager content)
		{
			texture = content.Load<Texture2D>("Sprites/Player/s_player_idle");

			damageObject.Load(content);

		#region  INITIALIZE TEXTURE ATLAS AND ANIMATION

			var spriteWidth = 54;
			var spriteHeight = 35;
			var objectTexture = content.Load<Texture2D>("Sprites/Player/s_player_atlas");
			var objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

			var animationFactory = new SpriteSheetAnimationFactory(objectAtlas);

			animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
			animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 3, 0, 1, 2, }, frameDuration: 0.2f, isLooping: true));
			animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 4 }));
			animationFactory.Add("fall", new SpriteSheetAnimationData(new[] { 5 }));
			animationFactory.Add("duck", new SpriteSheetAnimationData(new[] { 6 }));
			animationFactory.Add("attack", new SpriteSheetAnimationData(new[] { 8, 9, 10 }, frameDuration: 0.1f, isLooping: false));
			animationFactory.Add("duckAttack", new SpriteSheetAnimationData(new[] { 12, 13, 14 }, frameDuration: 0.1f, isLooping: false));
			
			#endregion


			objectAnimated = new AnimatedSprite(animationFactory, "idle");
			objectSprite = objectAnimated;

			attackSFX = content.Load<SoundEffect>("Sounds/sfx/a_attack2");

			base.Load(content);

			objectSprite.Origin = Vector2.Zero + origin;
			//set custom hitbox
			boundingBoxTopLeft = standingBoundingBox;
			boundingBoxBottomRight = new Vector2(33, 35);
		}

		public override void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{
			if (!Door.doorEnter)
			{
				//Check Input
				PlayerInput();

				//FSM Check
				StateMachine(PlayerState, objects, wallMap);

				//override player states: can always attack unless already attacking
				if (playerAttack == true && attackTimer <= 0)
				{
					if (PlayerState != State.duck) { PlayerState = State.attack; attackSFX.Play(); stopAnimating = false; }
					else { PlayerState = State.duckAttack; attackSFX.Play(); }
				}

				if (attackTimer > 0) { attackTimer--; }
				//update the damage object
				damageObject.Update(objects, wallMap, gametime);

				//update sprite
				if (stopAnimating == false)
				{
					objectAnimated.Update(gametime);
				}

				base.Update(objects, wallMap, gametime);

				//are we in a door

				var testRectDoor = InDoor(wallMap);
				if (testRectDoor != Rectangle.Empty)
				{
					applyGravity = false;
				}

				//check for death
				if (HUD.playerHealth <= 0)
				{ PlayerState = State.dead; }
			}
			objectSprite.Position = position;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			damageObject.Draw(spriteBatch);
			base.Draw(spriteBatch);
		}

		#region FINITE STATE MACHINE

		//switch between states
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
				case State.jump:
					Jump(wallMap);
					objectAnimated.Play("jump");
					break;
				case State.fall:
					Fall(wallMap);
					objectAnimated.Play("fall");
					break;
				case State.attack:
					Attack(objects, wallMap);
					var returnState = State.idle;
					if (isOnStairs == true) { returnState = State.stairs; }
					objectAnimated.Play("attack", () => PlayerState = returnState );
					break;
				case State.jumpAttack:
					objectAnimated.Play("jumpAttack", () => PlayerState = State.fall);
					break;
				case State.duckAttack:
					DuckAttack(objects, wallMap);
					objectAnimated.Play("duckAttack", () => PlayerState = State.duck);
					break;
				case State.duck:
					Duck(wallMap);
					objectAnimated.Play("duck");
					break;
				case State.stairs:
					Stairs(wallMap);
					objectAnimated.Play("walk");
					break;
				case State.hurt:
					Hurt();
					objectAnimated.Play("hurt", () => PlayerState = State.idle);
					break;
				case State.dead:
					Dead();
					boundingBoxTopLeft = new Vector2(14, 6);
					objectAnimated.Effect = SpriteEffects.FlipVertically;
					break;
				default:
					PlayerState = State.idle;
					break;
			}
		}

		// individual player states:

		private void Idle(WallMap wallMap)
		{
			//stop moving
			velocity.X = TendToZero(velocity.X, friction);

			//switch to walk
			if (playerLeft == true || playerRight == true)
			{
				PlayerState = State.walk;
			}
			//switch to jump
			if (playerJump == true && isJumping == false)
			{
				velocity.Y -= jumpHeight;
				PlayerState = State.jump;
				isJumping = true;
			}
			//switch to duck
			if (playerDown == true)
			{
				PlayerState = State.duck;
			}

			// climb stairs if contacting
			if (playerUp == true)
			{
				var stairTest = AtBottomOfStairs(wallMap);
				if (stairTest != Rectangle.Empty)
				{
					isOnStairs = true;
					PlayerState = State.stairs;
				}
			}
			if (playerDown == true)
			{
				var stairTest = AtTopOfStairs(wallMap);
				if ((stairTest != Rectangle.Empty) && (isOnGround))
				{
					isOnStairs = true;
					PlayerState = State.stairs;
				}
			}
		}


		private void Walk(WallMap wallMap)
		{
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
			//switch to jump
			if (playerJump == true && isJumping == false)
			{
				velocity.Y -= jumpHeight;
				PlayerState = State.jump;
				isJumping = true;
			}
			if (isOnGround == false)
			{
				PlayerState = State.fall;
			}


			//check to see if we are moving
			if (playerLeft == false && playerRight == false)
			{
				PlayerState = State.idle;
			}

			if (playerJump == true && isJumping == false)
			{
				velocity.Y -= jumpHeight;
				PlayerState = State.jump;
			}

			// climb stairs if contacting
			if (playerUp == true)
			{
				var stairTest = AtBottomOfStairs(wallMap);
				if (stairTest != Rectangle.Empty)
				{
					isOnStairs = true;
					PlayerState = State.stairs;
				}
			}
			if (playerDown == true)
			{
				var stairTest = AtTopOfStairs(wallMap);
				if ((stairTest != Rectangle.Empty) && (isOnGround))
				{
					isOnStairs = true;
					PlayerState = State.stairs;
				}
			}
		}


		private void Jump(WallMap wallMap)
		{
			if (velocity.Y > 0)
			{
				PlayerState = State.fall;
			}
			isJumping = false;
			if (playerLeft == true)
			{
				MoveLeft();
			}
			if (playerRight == true)
			{
				MoveRight();
			}
		}


		private void Fall(WallMap wallMap)
		{
			if (isOnGround == true)
			{
				isJumping = false;
				PlayerState = State.idle;
			}

			if (playerLeft == true)
			{
				MoveLeft();
			}
			if (playerRight == true)
			{
				MoveRight();
			}

			// climb stairs if contacting
			if (playerUp == true)
			{
				var stairTest = AtBottomOfStairs(wallMap);
				if (stairTest != Rectangle.Empty)
				{
					isOnStairs = true;
					jumpOnStairs = true;
					PlayerState = State.stairs;
				}
			}
		}


		private void Attack(List<GameObject> objects, WallMap wallMap)
		{

			if (attackTimer > maxTimer*(3/4) || attackTimer <= 0)
			{
				damageObject.StartTimer();
				damageObject.active = true;
				attackTimer = maxTimer;
			}
			damageObject.position = position + (new Vector2(18 + (Math.Abs(velocity.X) * 2), 0) * direction);

			if (isOnGround != true && !isOnStairs)
			{
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
			}
			else
			{
				velocity.X = TendToZero(velocity.X, friction);
			}

		}


		private void DuckAttack(List<GameObject> objects, WallMap wallMap)
		{
			if (attackTimer <= 0)
			{
				damageObject.position = position + new Vector2(0, 6) + (new Vector2(18, 0) * direction);
				damageObject.StartTimer();
				damageObject.active = true;
				attackTimer = maxTimer;
			}
		}


		private void Duck(WallMap wallMap)
		{
			velocity.X = TendToZero(velocity.X, friction);
			boundingBoxTopLeft = duckBoundingBox;
			if (playerDown == false)
			{
				boundingBoxTopLeft = standingBoundingBox;
				PlayerState = State.idle;
			}
		}


		private void Stairs(WallMap wallMap)
		{
			StairMaster(wallMap);

			if (direction.X < 0)
			{
				objectAnimated.Effect = SpriteEffects.FlipHorizontally;
			}
			else
			{
				objectAnimated.Effect = SpriteEffects.None;
			}

			//stop animating if not moving
			if (!playerUp && !playerDown && !playerLeft && !playerRight)
			{
				stopAnimating = true;
			}
			else
			{
				stopAnimating = false;
			}
		}


		private void Hurt()
		{
			if (invincible == false)
			{ 
				HUD.playerHealth -= 1;
				invincibleTimer = invincibleTimerMax;
			}
			PlayerState = State.idle;
		}


		private void Dead()
		{ }
			
		#endregion

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
			playerAttack = Input.KeyPressed(Keys.V) == true || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed;
			playerJump = Input.KeyPressed(Keys.Space) == true || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed;

			playerSpecial = Input.KeyPressed(Keys.B) == true || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed;
			playerMenu = Input.KeyPressed(Keys.Enter) == true || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed;
		}
		

		#endregion

		#region HELPER METHODS

		protected Rectangle AtTopOfStairs(WallMap wallMap)
		{
			Rectangle futureBoundingBox = new Rectangle((int)((BoundingBox.X)),
														(int)((BoundingBox.Y) + BoundingBox.Height+1),
														(int)(BoundingBox.Width),
														(int)(2));


			return wallMap.StairCollision(futureBoundingBox);
		}

		protected Rectangle AtBottomOfStairs(WallMap wallMap)
		{
			Rectangle futureBoundingBox = new Rectangle((int)((BoundingBox.X)),
														(int)((BoundingBox.Y+20)),
														(int)(BoundingBox.Width),
														(int)(BoundingBox.Height-20));


			return wallMap.StairCollision(futureBoundingBox);
		}

		protected Rectangle InDoor(WallMap wallMap)
		{
			Rectangle futureBoundingBox = new Rectangle((int)((BoundingBox.X)),
														(int)((BoundingBox.Y)),
														(int)(BoundingBox.Width),
														(int)(BoundingBox.Height));


			return wallMap.CheckForDoor(futureBoundingBox);
		}

		protected void StairMaster(WallMap wallMap)
		{
			//get ready to walk up stairs
			applyGravity = false;
			velocity = Vector2.Zero;
			canCollide = false;

			var topStairTest = AtTopOfStairs(wallMap);
			var bottomStairTest = AtBottomOfStairs(wallMap);

			var stairDir = 1;
			if (Stair.stairsGoUpToRight) { stairDir = 1; } else { stairDir = -1; }

			if(isOnGround == false) { jumpOnStairs = true; }

			if (jumpOnStairs == false)
			{
				//from the top of the stairs
				if (topStairTest != Rectangle.Empty)
				{
					if (Math.Round(position.X) != topStairTest.X + topStairTest.Width && centered == false && Stair.stairsGoUpToRight == true)
					{
						position.X = MathHelper.Lerp(position.X, topStairTest.X + topStairTest.Width, 0.3f);
					}
					else if (Math.Round(position.X) != topStairTest.X && centered == false && Stair.stairsGoUpToRight == false)
					{
						position.X = MathHelper.Lerp(position.X, topStairTest.X, 0.3f);
					}
					else { centered = true; }

				}
				

				//from the bottom of the stairs
				if (bottomStairTest != Rectangle.Empty)
				{
					//center player if they were walking
					if (Math.Round(position.X) != bottomStairTest.X && centered == false && Stair.stairsGoUpToRight == true)
					{
						position.X = MathHelper.Lerp(position.X, bottomStairTest.X, 0.3f);
					}
					else if (Math.Round(position.X) != bottomStairTest.X + bottomStairTest.Width && centered == false && Stair.stairsGoUpToRight == false)
					{
						position.X = MathHelper.Lerp(position.X, bottomStairTest.X + bottomStairTest.Width, 0.3f);
					}
					else { centered = true; }
				}
			}

			//center player if they were jumping
			else if (jumpOnStairs == true)
			{
				if(topStairTest != Rectangle.Empty)
				{
					var jumpCenter = 0f;
					if (stairDir == 1)
					{
						jumpCenter = (topStairTest.Y + topStairTest.Height - (stairDir * (position.X - topStairTest.X)));
					}
					else if (stairDir == -1)
					{
						jumpCenter = (topStairTest.Y + topStairTest.Height - ((topStairTest.X + topStairTest.Width) - position.X));
					}
				
					if (Math.Round(position.Y) == Math.Round(jumpCenter-15)) {
						centered = true;
					} else {
						position.Y = MathHelper.Lerp(position.Y, jumpCenter-15, 0.6f);
					}
				} else if (!centered) {
					applyGravity = true;
					isOnStairs = false;
					centered = false;
					canCollide = true;
					PlayerState = State.fall;
					jumpOnStairs = false;
				}
			}

			//now that we are centered on the stairs, move on the stair
			
			if (centered == true)
			{
				var hSpd = 0f;
				var vSpd = 0f;

				if (playerUp)
				{
					hSpd += stairSpeed * stairDir;
					vSpd -= stairSpeed;
					direction.X = stairDir;
				}
				if(playerDown)
				{
					hSpd += -stairSpeed * stairDir;
					vSpd -= -stairSpeed;
					direction.X = -stairDir;
				}
				if(playerLeft)
				{
					hSpd += -stairSpeed;
					vSpd -= -stairSpeed * stairDir;
					direction.X = -1;
				}
				if (playerRight)
				{
					hSpd += stairSpeed;
					vSpd -= stairSpeed * stairDir;
					direction.X = 1;
				}
				if (hSpd > 0.5f) { hSpd = 0.5f; }
				if (hSpd < -0.5f) { hSpd = -0.5f; }
				if (vSpd > 0.5f) { vSpd = 0.5f; }
				if (vSpd < -0.5f) { vSpd = -0.5f; }


				position.X += hSpd;
				position.Y += vSpd;
			}

			//check to see if we are still on stairs
			if (((playerDown == true) || (direction.X != stairDir)) && (topStairTest == Rectangle.Empty) 

			|| 

			((playerUp == true) || (direction.X == stairDir)) && (bottomStairTest == Rectangle.Empty))
			{
				applyGravity = true;
				isOnStairs = false;
				centered = false;
				canCollide = true;
				PlayerState = State.walk;
				jumpOnStairs = false;
			}
		}

		#endregion

	}
}
