using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;


namespace Parasol
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

		//create list of all objects
		public List<GameObject> objects = new List<GameObject>();
		
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
			// TODO: Add your initialization logic here

			//set the resolution
			Resolution.Init(ref graphics);
			Resolution.SetVirtualResolution(400, 240);
			Resolution.SetResolution(1920, 1080, false);

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
			Input.Update();
			UpdateObjects();

			base.Update(gameTime);
        }


        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
			DrawObjects();
			spriteBatch.End();


            base.Draw(gameTime);
        }

		#region Object List Methods

		public void LoadLevel()
		{
			objects.Add(new Player(new Vector2(200, 200)));

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

		public void UpdateObjects()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Update(objects);
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
