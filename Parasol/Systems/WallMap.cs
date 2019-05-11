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
		public List<Stair> stairs = new List<Stair>();
		public List<Door> doors = new List<Door>();
		public ScreenTransition screen;
		public bool rightTransition;

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

		public Rectangle CheckForDoor(Rectangle input)
		{
			for (int i = 0; i < doors.Count; i++)
			{
				if (doors[i] != null && doors[i].door.Intersects(input))
				{
					Door.doorEnter = true;
					if (input.Left < doors[i].door.Left)
					{
						rightTransition = true;
					}
					else if (input.Left > doors[i].door.Left)
					{
						rightTransition = false;
					}
					
					return doors[i].door;
				}
			}
			return Rectangle.Empty;
		}

		public Rectangle StairCollision(Rectangle input)
		{
			for (int i = 0; i < stairs.Count; i++)
			{
				if (stairs[i] != null && stairs[i].stair.Intersects(input) == true)
				{ 
					if (stairs[i].goesUpToRight == false)
					{
						Stair.stairsGoUpToRight = false;
					} 
					else 
					{ 
						Stair.stairsGoUpToRight = true;
					}
					return stairs[i].stair; 
				}
			}
			return Rectangle.Empty;
		}

		public void DrawWalls(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < walls.Count; i++)
			{
				if (walls[i] != null && walls[i].active == true)
				{ 
				spriteBatch.Draw(wallTexture, 
								new Vector2(walls[i].wall.X, walls[i].wall.Y),
								walls[i].wall, //source rect
								Color.White,   // color
								0.0f,			//rotation
								Vector2.Zero,	//origin
								1.0f,			//scale
								SpriteEffects.None,
								1.0f);			//layer depth
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

	public class Door
	{
		public Rectangle door;
		public bool active = true;
		static public bool doorEnter;

		public Door()
		{ }

		public Door(Rectangle inputRectangle)
		{
			door = inputRectangle;
		}
	}

	public class Stair
	{
		public Rectangle stair;
		public bool active = true;
		public bool goesUpToRight;
		public static bool stairsGoUpToRight = true;

		public Stair()
		{ }

		public Stair(Rectangle inputRectangle)
		{
			stair = inputRectangle;
			goesUpToRight = true;
		}
		public Stair(Rectangle inputRectangle, bool upToRight)
		{
			stair = inputRectangle;
			goesUpToRight = upToRight;
		}
	}
}
