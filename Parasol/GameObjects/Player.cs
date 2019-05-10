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

namespace Parasol
{
	public class Player : Entity
	{
		public Texture2D anchorPoint;
		
		//FSM enumerator
		enum PlayerState
		{
			Idle,
			Walk, 
			Jump,
			Attack,
			Duck, 
			DuckAttack,
			Stairs
		}
		public AnimatedSprite objectAnimated;


		public Player()
		{}

		public Player(Vector2 initPosition)
		{
			objectType = "player";
			position = initPosition;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Load(ContentManager content)
		{
			anchorPoint = content.Load<Texture2D>("Sprites/Collision/s_wall");
			texture = content.Load<Texture2D>("Sprites/Player/s_player_idle");

			// initialize spritesheet
			var spriteWidth = 54;
			var spriteHeight = 35;
			var objectTexture = content.Load<Texture2D>("Sprites/Player/s_player_atlas");
			var objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

			var animationFactory = new SpriteSheetAnimationFactory(objectAtlas);

			animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
			animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 0, 1, 2, 3 }, frameDuration: 0.2f, isLooping: true));
			animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 4, 5, }, frameDuration: 0.4f, isLooping: false));
			animationFactory.Add("duck", new SpriteSheetAnimationData(new[] { 6 }));
			animationFactory.Add("attack", new SpriteSheetAnimationData(new[] { 7, 8, 9, 10 }, frameDuration: 0.1f, isLooping: false));


			objectAnimated = new AnimatedSprite(animationFactory, "attack");
			objectSprite = objectAnimated;

			base.Load(content);

			objectSprite.Origin = Vector2.Zero + origin;
			//set custom hitbox
			boundingBoxTopLeft = new Vector2(22, 6);
			boundingBoxBottomRight = new Vector2(33, 35);
		}

		public override void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{
			CheckInput(objects, wallMap);
			Input.Update();

			base.Update(objects, wallMap, gametime);

			//update sprite
			objectAnimated.Update(gametime);
			objectSprite.Position = position;
		}

		public override void Draw(SpriteBatch spritebatch)
		{
			base.Draw(spritebatch);
			spritebatch.Draw(anchorPoint, position, Color.White);
		}


		#region Finite State Machine

		private void CheckInput(List<GameObject> objects, WallMap wallMap)
		{
			if (applyGravity == false)
			{
				// top down style controls

				if (Input.IsKeyDown(Keys.Up) == true)
				{
					objectAnimated.Play("walk");
					MoveUp();
				}
				if (Input.IsKeyDown(Keys.Down) == true)
				{
					objectAnimated.Play("walk");
					MoveDown();
				}
				if (Input.IsKeyDown(Keys.Left) == true)
				{
					objectAnimated.Play("walk");
					MoveLeft();
				}
				if (Input.IsKeyDown(Keys.Right) == true)
				{
					objectAnimated.Play("walk");
					MoveRight();
				}
			}
			else
			{
				// platformer style controls

				if (Input.IsKeyDown(Keys.Left) == true)
				{
					objectAnimated.Play("walk");
					objectAnimated.Effect = SpriteEffects.FlipHorizontally;
					MoveLeft();
				}
				if (Input.IsKeyDown(Keys.Right) == true)
				{
					objectAnimated.Play("walk");
					objectAnimated.Effect = SpriteEffects.None;
					MoveRight();
				}
				if (Input.KeyPressed(Keys.Up) == true)
				{
					objectAnimated.Play("walk");
					Jump(wallMap);
				}
				if (Input.IsKeyDown(Keys.Down) == true)
				{
					objectAnimated.Play("duck");
					//Duck
				}
				if(Input.KeyPressed(Keys.V))
				{
					objectAnimated.Play("attack", () => objectAnimated.Play("idle"));
				}
			}
		}
		#endregion

	}
}
