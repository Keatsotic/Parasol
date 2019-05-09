using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Parasol
{
	public class Player : Entity
	{
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

		public Player()
		{}

		public Player(Vector2 initPosition)
		{
			position = initPosition;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Load(ContentManager content)
		{
			texture = content.Load<Texture2D>("Sprites/Player/s_player_idle");
			base.Load(content);

			//set custom hitbox
			boundingBoxTopLeft = new Vector2(9, 8);
			boundingBoxBottomRight = new Vector2(21, 32);
		}

		public override void Update(List<GameObject> objects, WallMap wallMap)
		{
			CheckInput(objects, wallMap);
			Input.Update();
			base.Update(objects, wallMap);
		}

		public override void Draw(SpriteBatch spritebatch)
		{
			base.Draw(spritebatch);
		}

		private void CheckInput(List<GameObject> objects, WallMap wallMap)
		{
			if (applyGravity == false)
			{
				// top down style controls

				if (Input.IsKeyDown(Keys.Up) == true)
				{
					MoveUp();
				}
				if (Input.IsKeyDown(Keys.Down) == true)
				{
					MoveDown();
				}
				if (Input.IsKeyDown(Keys.Left) == true)
				{
					MoveLeft();
				}
				if (Input.IsKeyDown(Keys.Right) == true)
				{
					MoveRight();
				}
			}
			else
			{
				// platformer style controls

				if (Input.IsKeyDown(Keys.Left) == true)
				{
					MoveLeft();
				}
				if (Input.IsKeyDown(Keys.Right) == true)
				{
					MoveRight();
				}
				if (Input.IsKeyDown(Keys.Up) == true)
				{
					Jump(wallMap);
				}
				if (Input.IsKeyDown(Keys.Down) == true)
				{
					//Duck
				}
			}
		}


	}
}
