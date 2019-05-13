using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Parasol.SaveLoad;

namespace Parasol
{
	public class ScreenTransition
	{ 
		Texture2D fadeTexture;
		Rectangle fadeScreenRec;

		public bool fadeIn = true;
		public bool fadeComplete;
		private bool fadeOut;
		public bool fading;
		private int fadeAlpha = 0;
		private const int screenTimerMax = 50;
		private int screenTimer = screenTimerMax;

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
									 string roomNumber)
		{
			Game1.roomNumber = roomNumber;
			//sliding screens
			// if we are transitioning to a level and not a room, the number of chars in the room will be more than 3, 
			//so the room number will be the level number instead
			if (transitionDirection == "Left" || transitionDirection == "Right")
			{
				if (transitionDirection == "Left")
				{
					Camera.cameraMin.X = Camera.position.X;
					objects[0].position.Y = objects[0].position.Y;
					objects[0].position.X += 0.9f;
					Camera.cameraMin.X += 8;
					Camera.cameraMax.X += 8;
				}
				if (transitionDirection == "Right")
				{
					Camera.cameraMax.X = Camera.position.X;
					objects[0].position.Y = objects[0].position.Y;
					objects[0].position.X -= 0.9f;
					Camera.cameraMin.X -= 8;
					Camera.cameraMax.X -= 8;
				}
				screenTimer--;

				if (screenTimer < 0)
				{
					sceneManager.UnloadObjects(false, objects);
					Door.doorEnter = false;
					screenTimer = screenTimerMax;
					objects[0].applyGravity = true;
					sceneManager.LoadLevel(content, graphics, objects, Game1.levelNumber, Game1.roomNumber, false);
				}


			}
			if (transitionDirection == "StairTransition") // change rooms while staying on the stairs
			{
				sceneManager.UnloadObjects(true, objects);
				Door.doorEnter = false;
				screenTimer = screenTimerMax;
				sceneManager.LoadLevel(content, graphics, objects, Game1.levelNumber, Game1.roomNumber, false);
			}

			if (transitionDirection == "Fade") //
			{
				FadeInOut();
		
				if (fadeComplete && screenTimer <= 0)
				{
					Game1.roomNumber = "1";
					sceneManager.UnloadObjects(true, objects);
					sceneManager.LoadLevel(content, graphics, objects, Game1.levelNumber, Game1.roomNumber, true);
				}
				if (fadeComplete)
				{
					fadeComplete = false;
					fadeOut = true;
					screenTimer = screenTimerMax;

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
				fadeAlpha += 8;

				if (fadeAlpha >= 255)
				{
					fadeIn = false;
					fadeComplete = true;
				}
			}
			if (fadeOut)
			{
				fadeAlpha -= 8;

				if (fadeAlpha <= 0)
				{
					Door.doorEnter = false;
					fadeOut = false;
					fadeIn = true;
					fading = false;
					screenTimer = screenTimerMax;
				}
			}
		}	
	}
}
