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

		//init scenemanager
		private SceneManager sceneManager = new SceneManager();
		private ScreenTransition screenTransition = new ScreenTransition();
		private HUD hud = new HUD();

		//int level vars
		public static string levelNumber = "1";
		public static string roomNumber = "1";
		public static string previousLevel;

		//create list of all objects
		public List<GameObject> objects = new List<GameObject>();
		public static List<string> destroyedPermanent = new List<string>();
		static public int saveSlot;

		public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			//set the resolution

			Resolution.Init(ref graphics);
			Resolution.SetVirtualResolution(480, 270);
			Resolution.SetResolution(1920, 1080, false);

		}


        protected override void Initialize()
        {
			// TODO: Add your initialization logic here

			//init camera
			Camera.Initialize();
			Camera.cameraOffset = new Vector2(Resolution.VirtualWidth / 2, Resolution.VirtualHeight / 2);
			graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			hud.Initialize();

			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
			base.Initialize();
		}


		protected override void LoadContent()
		{
			// TODO: use this.Content to load your game content here
			hud.Load(Content);
			screenTransition.Load(Content);
			spriteBatch = new SpriteBatch(GraphicsDevice);
			sceneManager.LoadLevel(Content, graphics, objects, levelNumber, roomNumber, true);
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
			
				//check for door collisions
			if (Door.doorEnter == true) { screenTransition.StartScreenTransition(objects, graphics, Content, sceneManager, Door.transitionDirection, roomNumber); }

			//update input values
			hud.Update(objects, null, gameTime);
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

			
			sceneManager.Draw();
			DrawObjects();
			screenTransition.Draw(spriteBatch);
			spriteBatch.End();
			hud.Draw(spriteBatch);
			base.Draw(gameTime);
        }

		//camera methods
		private void UpdateCamera()
		{
			//Camera.LookAt(new Vector2(0, 0));
			if (objects.Count == 0) { return; }
			Camera.Update(objects[0].position + new Vector2(16, 0));
		}



		#region Object List Methods

		public void UpdateObjects(GameTime gameTime)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Update(objects, sceneManager.wallMap, gameTime);
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
