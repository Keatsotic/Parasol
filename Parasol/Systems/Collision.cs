using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Parasol
{
	public class Collision
	{
		public List<Wall> walls = new List<Wall>();

		Texture2D wallTexture;

		public int mapWidth = 15;
		public int mapHeight = 9;
		public int tileSize = 16;

		public void Load(ContentManager content)
		{
			wallTexture = content.Load<Texture2D>("Sprites/BG/s_wall");
		}
	}

	public class Wall
	{
		public Rectangle wall;
		public bool active;

		public Wall()
		{ }

		public Wall(Rectangle inputRectangle)
		{
			
		}
	}
}
