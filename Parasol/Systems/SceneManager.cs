using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System;

namespace Parasol
{
	public class SceneManager
	{
		//map vars
		private TiledMap tiledMap;
		private TiledMapRenderer renderer;
		public TiledMapObjectLayer objectLayer;
		private Vector2 roomMin;
		private Vector2 roomMax;
		private int parallax;

		private SoundEffect bgMusic;
		private SoundEffectInstance soundEffectInstance;
		private string levelNumberHolder;

		//wallMap vars
		public WallMap wallMap = new WallMap();
		public TiledMapObjectLayer ObjectLayer { get; private set; } = null;

		//lists of objects / walls / doors to clean up after
		private List<GameObject> killObjects = new List<GameObject>();
		private List<Wall> killWalls = new List<Wall>();
		private List<Door> killDoors = new List<Door>();
		private int willKillPlayer;


		//temp vars
		public string levelNameHolder;

		public SceneManager()
		{ }

		#region LEVEL LOADER

		public void LoadLevel(ContentManager content,
								GraphicsDeviceManager graphics,
								List<GameObject> objects,
								string levelNumber,
								string roomNumber,
								bool restartMusic)
		{

			bgMusic = content.Load<SoundEffect>("Sounds/Music/J04");

			if (soundEffectInstance != null && levelNumber != levelNumberHolder)
			{
				soundEffectInstance.Stop(true);
				soundEffectInstance.Dispose();
			}

			if (restartMusic)
			{
				soundEffectInstance = bgMusic.CreateInstance();
				soundEffectInstance.Play();
			}

			levelNumberHolder = levelNumber;
			

			//Load and Draw the map and walls
			wallMap.Load(content);
			tiledMap = content.Load<TiledMap>("TiledMaps/m_level_" + levelNumber);
			renderer = new TiledMapRenderer(graphics.GraphicsDevice);



			//access walls in map
			var tiledMapWallsLayer = tiledMap.GetLayer<TiledMapTileLayer>("Wall");

			//access stairs in map
			var tiledMapInteractLayer = tiledMap.GetLayer<TiledMapTileLayer>("Interact");

			//access objects in map
			objectLayer = tiledMap.GetLayer<TiledMapObjectLayer>("Room_" + roomNumber);

			if (objectLayer != null)
			{
				for (int i = 0; i < objectLayer.Objects.Length; i++)
				{
					if (objectLayer.Objects[i].Type == "Player" && objects.Count == 0)
					{
						if (Door.transitionDirection == "StairTransition")
						{
							if (objectLayer.Objects[i].Name == "PlayerStartStairs")
							{
								objects.Add(new Player(new Vector2(objectLayer.Objects[i].Position.X,
																		objectLayer.Objects[i].Position.Y)));
							}


						}
						else if (Door.transitionDirection != "StairTransition" && objectLayer.Objects[i].Name == "PlayerStart")
						{
							if (Game1.levelNumber != "Overworld")
							{
								objects.Add(new Player(new Vector2(objectLayer.Objects[i].Position.X,
																				objectLayer.Objects[i].Position.Y)));
							} 
							else 
							{
								objects.Add(new PlayerOverworld(new Vector2(objectLayer.Objects[i].Position.X,
																					objectLayer.Objects[i].Position.Y)));
							}
						}
						//if the player exist, initialize and load
						if (objects.Count > 0)
						{
							objects[0].Initialize();
							objects[0].Load(content);
						}
					}
					if (objectLayer.Objects[i].Type == "Camera") // set camera max and min
					{
						if (objectLayer.Objects[i].Name == "cameraMin")
						{
							Camera.cameraMin = objectLayer.Objects[i].Position + Camera.cameraOffset;
							roomMin = objectLayer.Objects[i].Position;
						}
						if (objectLayer.Objects[i].Name == "cameraMax")
						{
							Camera.cameraMax = objectLayer.Objects[i].Position - Camera.cameraOffset;
							roomMax = objectLayer.Objects[i].Position;
						}
					}
					if (objectLayer.Objects[i].Type == "CameraRestrict") //set camera restrictions
					{
						if (objectLayer.Objects[i].Name == "Horizontal")
						{
							Camera.updateYAxis = false;
						}
						if (objectLayer.Objects[i].Name == "Vertical")
						{
							Camera.updateYAxis = true;
						}
					}
					if (objectLayer.Objects[i].Type == "Door") // make doors
					{
						wallMap.doors.Add(new Door(new Rectangle((int)objectLayer.Objects[i].Position.X,
																(int)objectLayer.Objects[i].Position.Y,
																(int)objectLayer.Objects[i].Size.Width,
																(int)objectLayer.Objects[i].Size.Height),
																objectLayer.Objects[i].Name));
					}
				}
			}
			// draw the walls
			if (tiledMapWallsLayer != null)
			{
				for (var i = 0; i < tiledMapWallsLayer.Width; i++)
				{
					for (var j = 0; j < tiledMapWallsLayer.Height; j++)
					{
						if ((i > (roomMin.X - tiledMapWallsLayer.TileWidth) / tiledMapWallsLayer.TileWidth && 
						j > (roomMin.Y - tiledMapWallsLayer.TileHeight) / tiledMapWallsLayer.TileHeight) && 
						(i <= (roomMax.X + tiledMapWallsLayer.TileWidth) / tiledMapWallsLayer.TileWidth && 
						j <= (roomMax.Y + tiledMapWallsLayer.TileHeight) / tiledMapWallsLayer.TileHeight))
						{
							if (tiledMapWallsLayer.TryGetTile(i, j, out TiledMapTile? tile))
							{
								if (tile.Value.GlobalIdentifier == 1) // make walls
								{
									wallMap.walls.Add(new Wall(new Rectangle(i * tiledMapWallsLayer.TileWidth,
																			j * tiledMapWallsLayer.TileHeight,
																			tiledMapWallsLayer.TileWidth,
																			tiledMapWallsLayer.TileHeight)));
								}
							}
						}
					}
				}
			}
			if (tiledMapInteractLayer != null)
			{
				for (var i = 0; i < tiledMapInteractLayer.Width; i++)
				{
					for (var j = 0; j < tiledMapInteractLayer.Height; j++)
					{
						if ((i >= roomMin.X / tiledMapInteractLayer.TileWidth && j >= roomMin.Y / tiledMapInteractLayer.TileHeight) && (i <= roomMax.X / tiledMapInteractLayer.TileWidth && j <= roomMax.Y / tiledMapInteractLayer.TileHeight))
						{
							if (tiledMapInteractLayer.TryGetTile(i, j, out TiledMapTile? tile))
							{
								if (tile.Value.GlobalIdentifier == 2) // make stairs
								{
									//use for stair directions
									if (!tile.Value.IsFlippedHorizontally)
									{
										wallMap.stairs.Add(new Stair(new Rectangle(i * tiledMapInteractLayer.TileWidth,
																				j * tiledMapInteractLayer.TileHeight,
																				tiledMapInteractLayer.TileWidth,
																				tiledMapInteractLayer.TileHeight)));
									}
									else
									{
										wallMap.stairs.Add(new Stair(new Rectangle(i * tiledMapInteractLayer.TileWidth,
																					j * tiledMapInteractLayer.TileHeight,
																					tiledMapInteractLayer.TileWidth,
																					tiledMapInteractLayer.TileHeight), false));

									}
								}
							}
						}
					}
				}
			}
			//load objects
			LoadObjects(content, objects);
		}

		public void LoadObjects(ContentManager content, List<GameObject> objects)
		{
			for (int i = 1; i < objects.Count; i++)
			{
				objects[i].Initialize();
				objects[i].Load(content);
			}
		}

		
		public void UnloadObjects(bool killPlayer, List<GameObject> objects)
		{
			Game1.previousLevel = Game1.levelNumber;

			if (killPlayer == true)
			{
				willKillPlayer = 0;
			}
			else
			{
				willKillPlayer = 1;
			}

			for (int i = willKillPlayer; i < objects.Count; i++)
			{
				killObjects.Add(objects[i]);
			}
			foreach (var doors in wallMap.doors)
			{
				killDoors.Add(doors);
			}
			foreach (var walls in wallMap.walls)
			{
				killWalls.Add(walls);
			}
			//detroy old objects --- there must be a smarter way to do this....
			foreach (GameObject o in killObjects)
			{
				objects.Remove(o);
			}
			foreach (Door d in killDoors)
			{
				wallMap.doors.Remove(d);
			}
			foreach (Wall w in killWalls)
			{
				wallMap.walls.Remove(w);
			}
		}

		public void Draw()
		{
 			parallax = (int)Math.Round(Camera.position.X * 0.6f);
		
			Matrix layerMg = Matrix.CreateTranslation((Camera.position.X)*0.1f, 0, 0) * Camera.GetTransformMatrix();
			Matrix layerBg = Matrix.CreateTranslation(parallax-176, 0, 0) * Camera.GetTransformMatrix();

			if (tiledMap.GetLayer("FarBackground") != null)
			{
				renderer.Draw(tiledMap.GetLayer("FarBackground"), Resolution.GetTransformationMatrix());
			}
			if (tiledMap.GetLayer("Background") != null)
			{
				renderer.Draw(tiledMap.GetLayer("Background"), layerBg);
			}
			if (tiledMap.GetLayer("MidBackground") != null)
			{
				renderer.Draw(tiledMap.GetLayer("MidBackground"), layerMg);
			}
			renderer.Draw(tiledMap.GetLayer("Foreground"), Camera.GetTransformMatrix());
		}

		#endregion
	}
}
