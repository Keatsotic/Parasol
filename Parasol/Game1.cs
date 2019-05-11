using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;


namespace Parasol
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

			/// <summary>
			/// these objects will get moved into the sceneManager when they are finished
			/// </summary>
		//map vars
		public TiledMap tiledMap;
		public TiledMapRenderer renderer;
		public TiledMapObjectLayer objectLayer;
		public Vector2 roomMin;
		public Vector2 roomMax;

		public SoundEffect bgMusic;
		public SoundEffectInstance soundEffectInstance;


		// /////////////////////

		public static string levelNumber = "1";
		public static string roomNumber = "1";
		public static string previousLevel;

		//create list of all objects
		public List<GameObject> objects = new List<GameObject>();
		public WallMap wallMap = new WallMap();

		
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			//set the resolution

			Resolution.Init(ref graphics);
			Resolution.SetVirtualResolution(400, 240);
			Resolution.SetResolution(1200, 720, false);

		}


        protected override void Initialize()
        {
			// TODO: Add your initialization logic here

			//init camera
			Camera.Initialize();
			Camera.cameraOffset = new Vector2(Resolution.VirtualWidth / 2, Resolution.VirtualHeight / 2);


			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
			base.Initialize();
		}


		protected override void LoadContent()
		{
			// TODO: use this.Content to load your game content here
			spriteBatch = new SpriteBatch(GraphicsDevice);
			bgMusic = Content.Load<SoundEffect>("Sounds/Music/J04");
			LoadLevel();
		}


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

    
        protected override void Update(GameTime gameTime)
        {
			/// <param name="gameTime">Provides a snapshot of timing values.</param>
			
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

			//update input values

			UpdateObjects(gameTime);
			UpdateCamera();
			base.Update(gameTime);
        }


        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			Resolution.BeginDraw();
			spriteBatch.Begin(SpriteSortMode.BackToFront,
								BlendState.AlphaBlend,
								SamplerState.PointClamp,
								null,
								null,
								null,
								Camera.GetTransformMatrix());

			renderer.Draw(tiledMap, Camera.GetTransformMatrix());
			DrawObjects(); 
			spriteBatch.End();


            base.Draw(gameTime);
        }

		//camera methods
		private void UpdateCamera()
		{
			Camera.LookAt(new Vector2(0, 0));
			if (objects.Count == 0) { return; }
			Camera.Update(objects[0].position + new Vector2(16, 0));
		}

		#region LEVEL LOADER

		public void LoadLevel()
		{
			//Load and Draw the map and walls
			wallMap.Load(Content);
			tiledMap = Content.Load<TiledMap>("TiledMaps/m_level_0"+ levelNumber);
			renderer = new TiledMapRenderer(graphics.GraphicsDevice);

			soundEffectInstance = bgMusic.CreateInstance();
			soundEffectInstance.Play();

			//access walls in map
			var tiledMapWallsLayer = tiledMap.GetLayer<TiledMapTileLayer>("Wall");

			//access objects in map
			objectLayer = tiledMap.GetLayer<TiledMapObjectLayer>("Room_" + roomNumber);

			if (objectLayer != null)
			{
				for (int i = 0; i < objectLayer.Objects.Length; i++)
				{
					if (objectLayer.Objects[i].Type == "Player")
					{
						objects.Add(new Player(new Vector2(objectLayer.Objects[i].Position.X,
																objectLayer.Objects[i].Position.Y)));
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
						if ((i > roomMin.X/ tiledMapWallsLayer.TileWidth && j > roomMin.Y/ tiledMapWallsLayer.TileHeight) && (i < roomMax.X/tiledMapWallsLayer.TileWidth && j < roomMax.Y/tiledMapWallsLayer.TileHeight))
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

			LoadObjects();
		}

		public void LoadObjects()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Initialize();
				objects[i].Load(Content);
			}
		}
		#endregion

		#region Object List Methods

		public void UpdateObjects(GameTime gameTime)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Update(objects, wallMap, gameTime);
			}
		}

		public void DrawObjects()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Draw(spriteBatch);
			}
		}
		#endregion

	}
}
