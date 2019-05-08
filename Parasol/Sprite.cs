using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Parasol
{
	class Sprite
	{
		private Texture2D _texture;
		public Vector2 _position;
		private Color _color;

		private float _speed = 2.0f;

		public Sprite(Texture2D texture, Color color)
		{
			_texture = texture;
			_color = color;
		}

		public void Update()
		{
			//Move player
			if (Keyboard.GetState().IsKeyDown(Keys.Up))
			{
				_position.Y-= _speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Down))
			{
				_position.Y += _speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				_position.X -= _speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				_position.X += _speed;
			}
		}

		public void Draw(SpriteBatch spritebatch) 
		{
			spritebatch.Draw(_texture, _position, Color.White);
		}
	}
}
