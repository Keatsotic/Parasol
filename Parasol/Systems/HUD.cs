using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using System;

namespace Parasol
{
	public class HUD : GameObject
	{
		public static int playerMaxHealth = 5;
		public static int playerHealth;
		public static int playerLives = 0;
		public static int rupeeCount = 0;
		public static int rupeeDisplay = 0;

		private Texture2D healthFullTexture;
		private Texture2D healthEmptyTexture;
		private Texture2D livesTexture;
		private Texture2D rupeeTexture;

		private SpriteFont font;

		//text box 
		TextBox textBox = new TextBox();

		public override void Initialize()
		{
			playerHealth = playerMaxHealth;
			rupeeDisplay = rupeeCount;
			base.Initialize();
		}

		public override void Update(List<GameObject> _objects, WallMap wallMap, GameTime gameTime)
		{
			if (rupeeCount >= 100)
			{
				playerLives++;
				rupeeCount -= 100;
			}
			RupeeAddition();
			//textBox.Update(_objects, wallMap, gameTime);
			base.Update(_objects, wallMap, gameTime);
		}

		public override void Load(ContentManager content)
		{
			font = content.Load<SpriteFont>("Fonts/DefaultFont");
			healthFullTexture = content.Load<Texture2D>("Sprites/HUD/healthFullTexture");
			healthEmptyTexture = content.Load<Texture2D>("Sprites/HUD/healthEmptyTexture");
			livesTexture = content.Load<Texture2D>("Sprites/HUD/s_playerLives");
			rupeeTexture = content.Load<Texture2D>("Sprites/HUD/rupee");
			//textBox.Load(content);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (healthFullTexture != null)
			{
				spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Resolution.GetTransformationMatrix());

				for (int i = 0; i < playerMaxHealth; i++)
				{
					spriteBatch.Draw(healthEmptyTexture, new Vector2(12 + (i * 16), 12), Color.White);
				}
				for (int i = 0; i < playerHealth; i++)
				{
					spriteBatch.Draw(healthFullTexture, new Vector2(12 + (i * 16), 12), Color.White);
				}
				spriteBatch.Draw(livesTexture, new Vector2(16, 32), Color.White);
				spriteBatch.DrawString(font,""+ playerLives, new Vector2(32, 32), Color.White);

				spriteBatch.Draw(rupeeTexture, new Vector2(450, 16), Color.White);
				spriteBatch.DrawString(font,""+ rupeeDisplay, new Vector2(440, 16), Color.White);


				textBox.Draw(spriteBatch);

				spriteBatch.End();
			}
		}

		public void RupeeAddition()
		{
			if (rupeeDisplay < rupeeCount)
			{
				rupeeDisplay++;
			}
			if (rupeeDisplay > rupeeCount)
			{
				rupeeDisplay++;
				if (rupeeDisplay > 100)
				{
					rupeeDisplay -= 100;
				}
			}
		}
	}
}