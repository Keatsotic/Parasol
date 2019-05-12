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


namespace Parasol
{
	public class SceneManager
	{
		//map vars
		public TiledMap tiledMap;
		public TiledMapRenderer renderer;
		public TiledMapObjectLayer objectLayer;
		public Vector2 roomMin;
		public Vector2 roomMax;

		public SoundEffect bgMusic;
		public SoundEffectInstance soundEffectInstance;
		private string levelNumberHolder;

		//wallMap vars
		public WallMap wallMap = new WallMap();
		public TiledMapObjectLayer ObjectLayer { get; private set; } = null;

		//lists of objects / walls / doors to clean up after
		public List<GameObject> killObjects = new List<GameObject>();
		public List<Wall> killWalls = new List<Wall>();
		public List<Door> killDoors = new List<Door>();
		public int willKillPlayer;


		//temp vars
		public string levelNameHolder;

		public SceneManager()
		{ }

		#region LEVEL LOADER

		public void LoadLevel(ContentManager content,
								GraphicsDeviceManager graphics,
								List<GameObject> objects,
								string levelNumber,
								int roomNumber,
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
			tiledMap = content.Load<TiledMap>("TiledMaps/m_level_0" + 1);
			renderer = new TiledMapRenderer(graphics.GraphicsDevice);

			//access walls in map
			var tiledMapWallsLayer = tiledMap.GetLayer<TiledMapTileLayer>("Wall");

			//access objects in map
			objectLayer = tiledMap.GetLayer<TiledMapObjectLayer>("Room_" + roomNumber);

			if (objectLayer != null)
			{
				for (int i = 0; i < objectLayer.Objects.Length; i++)
				{
					if (objectLayer.Objects[i].Type == "Player" && objects.Count == 0)
					{
						objects.Add(new Player(new Vector2(objectLayer.Objects[i].Position.X,
																objectLayer.Objects[i].Position.Y)));
						objects[0].Initialize();
						objects[0].Load(content);
					}
					if (objectLayer.Objects[i].Type == "Camera")
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
					if (objectLayer.Objects[i].Type == "CameraRestrict")
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
				}
			}

			if (tiledMapWallsLayer != null)
			{
				for (var i = 0; i < tiledMapWallsLayer.Width; i++)
				{
					for (var j = 0; j < tiledMapWallsLayer.Height; j++)
					{
						if ((i >= roomMin.X / tiledMapWallsLayer.TileWidth && j >= roomMin.Y / tiledMapWallsLayer.TileHeight) && (i <= roomMax.X / tiledMapWallsLayer.TileWidth && j <= roomMax.Y / tiledMapWallsLayer.TileHeight))
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
								if (tile.Value.GlobalIdentifier == 2) // make stairs
								{
									//use for stair directions
									if (!tile.Value.IsFlippedHorizontally)
									{
										wallMap.stairs.Add(new Stair(new Rectangle(i * tiledMapWallsLayer.TileWidth,
																				j * tiledMapWallsLayer.TileHeight,
																				tiledMapWallsLayer.TileWidth,
																				tiledMapWallsLayer.TileHeight)));
									}
									else
									{
										wallMap.stairs.Add(new Stair(new Rectangle(i * tiledMapWallsLayer.TileWidth,
																					j * tiledMapWallsLayer.TileHeight,
																					tiledMapWallsLayer.TileWidth,
																					tiledMapWallsLayer.TileHeight), false));

									}
								}
								if (tile.Value.GlobalIdentifier == 3) // make stairs
								{
									wallMap.doors.Add(new Door(new Rectangle(i * tiledMapWallsLayer.TileWidth,
																			j * tiledMapWallsLayer.TileHeight,
																			tiledMapWallsLayer.TileWidth,
																			tiledMapWallsLayer.TileHeight)));
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
			renderer.Draw(tiledMap, Camera.GetTransformMatrix());
		}

		#endregion
	}
}
