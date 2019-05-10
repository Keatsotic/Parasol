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
	public class WallMap
	{
		public List<Wall> walls = new List<Wall>();

		Texture2D wallTexture;

		//public int mapWidth = 15;
		//public int mapHeight = 9;
		public int tileSize = 16;

		public void Load(ContentManager content)
		{
			wallTexture = content.Load<Texture2D>("Sprites/Collision/s_wall");
		}

		public Rectangle CheckCollision(Rectangle input)
		{ 
			for (int i = 0; i < walls.Count; i++)
			{
				if (walls[i] != null && walls[i].wall.Intersects(input) == true)
				{ return walls[i].wall; }
			}
			return Rectangle.Empty;
		}

		public void DrawWalls(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < walls.Count; i++)
			{
				if (walls[i] != null && walls[i].active == true)
				{ spriteBatch.Draw(wallTexture, 
								new Vector2(walls[i].wall.X, walls[i].wall.Y),
								walls[i].wall, //source rect
								Color.White,   // color
								0.0f,			//rotation
								Vector2.Zero,	//origin
								1.0f,			//scale
								SpriteEffects.None,
								0.7f);			//layer depth
				}
			}
		}
	}

	public class Wall
	{
		public Rectangle wall;
		public bool active = true; 

		public Wall()
		{ }

		public Wall(Rectangle inputRectangle)
		{
			wall = inputRectangle;
		}
	}
}
