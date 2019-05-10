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
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

		//map vars
		public TiledMap tiledMap;
		public TiledMapRenderer renderer;
		public TiledMapObjectLayer objectLayer;

		//create list of all objects
		public List<GameObject> objects = new List<GameObject>();
		public WallMap wallMap = new WallMap();
		
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

			//set the resolution
			
			Resolution.Init(ref graphics);
			Resolution.SetVirtualResolution(426, 240); // resolution of assets
			Resolution.SetResolution(1920, 1080, false);
			
		}


        protected override void Initialize()
        {
			// TODO: Add your initialization logic here

			//init camera
			Camera.Initialize();
			Camera.updateYAxis = false;

			base.Initialize();
		}


		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			
			LoadLevel();
			// TODO: use this.Content to load your game content here
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

		#region Load Levels

		public void LoadLevel()
		{

			wallMap.Load(Content);

			//map
			tiledMap = Content.Load<TiledMap>("TiledMaps/m_level_01");
			renderer = new TiledMapRenderer(graphics.GraphicsDevice);

			//access walls in map
			var tiledMapWallsLayer = tiledMap.GetLayer<TiledMapTileLayer>("Wall");


			if (tiledMapWallsLayer != null)
			{
				for (var i = 0; i < tiledMapWallsLayer.Width; i++)
				{
					for (var j = 0; j < tiledMapWallsLayer.Height; j++)
					{
						if (tiledMapWallsLayer.TryGetTile(i, j, out TiledMapTile? tile))
						{
							if (!tile.Value.IsBlank)
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


			//access objects in map
			objectLayer = tiledMap.GetLayer<TiledMapObjectLayer>("Room_1");

			if (objectLayer != null)
			{
				for (int i = 0; i < objectLayer.Objects.Length; i++)
				{
					if (objectLayer.Objects[i].Type == "Player")
					{
						objects.Add(new Player(new Vector2(objectLayer.Objects[i].Position.X,
																objectLayer.Objects[i].Position.Y)));
					}
				}
			}

			//load objects


			LoadObjects();
		}
#endregion

		#region Object List Methods

		public void LoadObjects()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Initialize();
				objects[i].Load(Content);
			}
		}

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

		//camera methods
		private void UpdateCamera()
		{
			if (objects.Count == 0) { return; }

			Camera.Update(objects[0].position + new Vector2(0, 0));
		}
	}
}
