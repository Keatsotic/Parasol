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
	public class Player : GameObject
	{
		private int _speed = 3;

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
		}

		public override void Update(List<GameObject> objects)
		{
			CheckInput();
			base.Update(objects);
		}

		public override void Draw(SpriteBatch spritebatch)
		{
			base.Draw(spritebatch);
		}

		private void CheckInput()
		{
			if (Input.IsKeyDown(Keys.Up) == true)
			{
				position.Y -= _speed;
			}
			if (Input.IsKeyDown(Keys.Down) == true)
			{
				position.Y += _speed;
			}
			if (Input.IsKeyDown(Keys.Left) == true)
			{
				position.X -= _speed;
			}
			if (Input.IsKeyDown(Keys.Right) == true)
			{
				position.X += _speed;
			}
		}
	}
}
