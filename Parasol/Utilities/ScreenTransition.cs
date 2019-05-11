using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Parasol.SaveLoad;

namespace Parasol
{
	public class ScreenTransition
	{
		public int screenTimer = 50;
		static public string roomPlaceHolder;

		Texture2D fadeTexture;
		Rectangle fadeScreenRec;

		public bool fadeIn = true;
		public bool fadeComplete;
		private bool fadeOut;
		private bool fading;
		private int fadeAlpha = 0;

		public ScreenTransition()
		{
		}

		public void Load(ContentManager content)
		{
			fadeTexture = content.Load<Texture2D>("Sprites/pixel");
			fadeScreenRec = new Rectangle(-20, -20, Resolution.VirtualWidth + 40, Resolution.VirtualHeight + 40);
		}

		public void Draw(SpriteBatch _spriteBatch)
		{
			_spriteBatch.Begin(SpriteSortMode.BackToFront,
							   BlendState.AlphaBlend,
							   SamplerState.PointClamp,
							   null,
							   null,
							   null,
							   Camera.GetTransformMatrix());

			_spriteBatch.Draw(fadeTexture,
								  new Vector2(Camera.position.X - (Resolution.VirtualWidth / 2) - 20,
											  Camera.position.Y - (Resolution.VirtualHeight / 2) - 20),
								  fadeScreenRec,
								  new Color(0, 0, 0, fadeAlpha),
								  0f,
								  Vector2.Zero,
								  1f,
								  SpriteEffects.None,
								  0f);
			_spriteBatch.End();
		}

		public void StartScreenTransition(List<GameObject> objects,
									 GraphicsDeviceManager graphics,
									 ContentManager content,
									 SceneManager sceneManager,
									 string direction,
									 string roomNumber)
		{
			//sliding screens
			if (roomPlaceHolder != "Overworld" && Game1.levelNumber != "Overworld" && !fading)
			{
				if (direction == "right")
				{
					Camera.cameraMin.X = Camera.position.X;
					objects[0].position.Y = objects[0].position.Y;
					objects[0].position.X += 0.5f;
					Camera.cameraMin.X += 8;
					Camera.cameraMax.X += 8;
				}
				if (direction == "left")
				{
					Camera.cameraMax.X = Camera.position.X;
					objects[0].position.Y = objects[0].position.Y;
					objects[0].position.X -= 0.5f;
					Camera.cameraMin.X -= 8;
					Camera.cameraMax.X -= 8;
				}
				screenTimer--;

				if (screenTimer < 0)
				{
					//sceneManager.UnloadObjects(false, objects);
					roomNumber = roomPlaceHolder;
					Game1.roomNumber = roomNumber;
					Door.doorEnter = false;
					screenTimer = 50;
					//Game1.LoadLevel(content, graphics.GraphicsDevice, objects, Game1.levelNumber, false);
					//Entity.applyGravity = true;
				}


			}
			/*
			if ((roomPlaceHolder == "Overworld" || Game1.levelNumber == "Overworld") || fading)
			{
				FadeInOut();

				if (fadeComplete && screenTimer <= 0 && roomPlaceHolder == "Overworld")
				{
					sceneCreator.UnloadObjects(true, objects);
					Game1.levelNumber = roomPlaceHolder;
					Entity.applyGravity = false;
					sceneCreator.LevelLoader(content, graphics.GraphicsDevice, objects, Game1.levelNumber, "1", true);
				}
				else if (fadeComplete && screenTimer <= 0 && Game1.levelNumber == "Overworld")
				{
					sceneCreator.UnloadObjects(true, objects);
					Game1.levelNumber = roomPlaceHolder;
					Entity.applyGravity = true;
					sceneCreator.LevelLoader(content, graphics.GraphicsDevice, objects, Game1.levelNumber, "1", true);
				}
				if (fadeComplete)
				{
					fadeComplete = false;
					fadeOut = true;
					screenTimer = 50;

					// save the game
					XmlSerialization.WriteToXmlFile("SaveFile0" + Game1.saveSlot + ".txt", Game1._destroyedPermanent);
					XmlSerialization.WriteToXmlFile("HealthFile0" + Game1.saveSlot + ".txt", HUD.playerMaxHealth);
				}
			}
		}

		public void FadeInOut()
		{
			fading = true;
			// fade screen
			screenTimer -= 2;
			if (fadeIn)
			{
				fadeAlpha += 10;

				if (fadeAlpha >= 255)
				{
					fadeIn = false;
					fadeComplete = true;
				}
			}
			if (fadeOut)
			{

				fadeAlpha -= 10;

				if (fadeAlpha <= 0)
				{
					Door.doorEnter = false;
					fadeOut = false;
					fadeIn = true;
					fading = false;
					screenTimer = 50;
				}
			}
		}
		*/
		}
	}
}
