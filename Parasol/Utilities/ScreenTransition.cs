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

		Texture2D fadeTexture;
		Rectangle fadeScreenRec;

		public bool fadeIn = true;
		public bool fadeComplete;
		private bool fadeOut;
		private bool fading;
		private int fadeAlpha = 0;
		private int roomDir;

		public ScreenTransition()
		{
		}

		public void Load(ContentManager content)
		{
			fadeTexture = content.Load<Texture2D>("Sprites/Collision/s_wall");
			fadeScreenRec = new Rectangle(-20, -20, Resolution.VirtualWidth + 40, Resolution.VirtualHeight + 40);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(fadeTexture,
								  new Vector2(Camera.position.X - (Resolution.VirtualWidth / 2) - 20,
											  Camera.position.Y - (Resolution.VirtualHeight / 2) - 20),
								  fadeScreenRec,
								  new Color(0, 0, 0, fadeAlpha),
								  0f,
								  Vector2.Zero,
								  1f,
								  SpriteEffects.None,
								  0f);
		}

		public void StartScreenTransition(List<GameObject> objects,
									 GraphicsDeviceManager graphics,
									 ContentManager content,
									 SceneManager sceneManager,
									 string transitionDirection,
									 int roomNumber)
		{
			//sliding screens
			if (transitionDirection != "Overworld" && !fading)
			{
				if (transitionDirection == "left")
				{
					Camera.cameraMin.X = Camera.position.X;
					objects[0].position.Y = objects[0].position.Y;
					objects[0].position.X += 0.9f;
					Camera.cameraMin.X += 8;
					Camera.cameraMax.X += 8;
					roomDir = 1;
				}
				if (transitionDirection == "right")
				{
					Camera.cameraMax.X = Camera.position.X;
					objects[0].position.Y = objects[0].position.Y;
					objects[0].position.X -= 0.9f;
					Camera.cameraMin.X -= 8;
					Camera.cameraMax.X -= 8;
					roomDir = -1;
				}
				screenTimer--;

				if (screenTimer < 0)
				{
					sceneManager.UnloadObjects(false, objects);
					Door.doorEnter = false;
					Game1.roomNumber = roomNumber + roomDir;
					screenTimer = 50;
					objects[0].applyGravity = true;
					sceneManager.LoadLevel(content, graphics, objects, Game1.levelNumber, Game1.roomNumber, false);
				}


			}

			if (transitionDirection == "overworld") //
			{
				FadeInOut();

				if (fadeComplete && screenTimer <= 0)
				{
					sceneManager.UnloadObjects(true, objects);
					Game1.levelNumber = "0"; //
					if (objects.Count > 0) { objects[0].applyGravity = false; }
					sceneManager.LoadLevel(content, graphics, objects, Game1.levelNumber, 1, true);
				}
				else if (fadeComplete && screenTimer <= 0 && Game1.levelNumber == "0") //
				{
					sceneManager.UnloadObjects(true, objects);
					Game1.levelNumber = "1";   //
					if (objects.Count > 0) { objects[0].applyGravity = true; }
					sceneManager.LoadLevel(content, graphics, objects, Game1.levelNumber, 1, true);
				}
				if (fadeComplete)
				{
					fadeComplete = false;
					fadeOut = true;
					screenTimer = 50;

					// save the game
					XmlSerialization.WriteToXmlFile("SaveFile0" + Game1.saveSlot + ".txt", Game1.destroyedPermanent);
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
		
	}
}
