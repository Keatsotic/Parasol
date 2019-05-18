using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Parasol
{
	public class DamageObject : GameObject
	{
		private int timer;
		private const int maxTimer = 3;
		

		private Rectangle damageBoundingBox = new Rectangle(-16, -16, 16, 16);

		public DamageObject()
		{
			active = false;
		}

		public DamageObject(Vector2 inputPosition)
		{
			objectType = "Damage";
			position = inputPosition;
			active = false;
		}

		public override void Initialize()
		{
			canCollide = false;
			base.Initialize();

			boundingBoxTopLeft = Vector2.Zero;
			boundingBoxBottomRight = new Vector2(16, 16);
		}

		public override void Load(ContentManager content)
		{
			texture = content.Load<Texture2D>("Sprites/Collision/s_wall");
			base.Load(content);
		}

		public override void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{
			if (timer > 0 && active == true)
			{
				timer--;
				CheckCollision(objects, wallMap);
			} 
			else 
			{
				active = false;
			}
			damageBoundingBox.X = (int)position.X;
			damageBoundingBox.Y = (int)position.Y;
			base.Update(objects, wallMap, gametime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
#if DEBUG
			if (texture != null && active == true)
			{
				spriteBatch.Draw(texture,
									position,
									damageBoundingBox,
									new Color(255, 0, 0, 80),
									rotation,
									new Vector2(8, 8),
									1f,
									SpriteEffects.None,
									0.1f);
			}
#endif
			base.Draw(spriteBatch);
		}

		public void StartTimer()
		{
			timer = maxTimer;
		}

		private void CheckCollision(List<GameObject> objects, WallMap wallMap)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				if (objects[i].objectType == "Enemy" && objects[i].CheckCollision(damageBoundingBox) == true)
				{
					if (objects[0].position.X > objects[i].BoundingBox.X)
					{
						objects[0].knockbackDir.X = 1;
					}
					else
					{
						objects[0].knockbackDir.X = -1;
					}
					active = false;
					objects[0].velocity.X = 0;
					objects[0].velocity = objects[i].knockback * objects[0].knockbackDir;
					objects[i].isHurt = true;
				}
			}
		}
	}
}
