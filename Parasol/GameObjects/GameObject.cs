using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using System;

namespace Parasol
{		
	public class GameObject
	{
		protected Texture2D texture;
		public Vector2 position;
		public Color color;
		public float scale = 1.0f, rotation = 0.0f;
		public float layerDepth = 0.5f;

		//will it be affected by walls
		public Vector2 velocity;
		public bool canCollide = true;
		public bool active = true;
		public bool isHurt = false;

		public Vector2 knockbackDir = new Vector2(1, 1);
		public Vector2 knockback;

		public Vector2 direction;
		protected Vector2 origin;

		public bool invincible = false;
		protected AnimatedSprite objectAnimated;
		protected Sprite objectSprite = null;

		//type identifier
		public string objectType = null;

		//bounding boxes
		bool drawBoundingBoxes = false;

		//choose whether graviy affects or not
		public bool applyGravity = true;

		protected Vector2 boundingBoxTopLeft, boundingBoxBottomRight;
		Texture2D boundingBoxTexture;
		

		public Rectangle BoundingBox
		{
			get 
			{
				return new Rectangle((int)(position.X + (boundingBoxTopLeft.X) - origin.X),
									(int)(position.Y + (boundingBoxTopLeft.Y) - origin.Y),
									(int)(boundingBoxBottomRight.X - (boundingBoxTopLeft.X)),
									(int)(boundingBoxBottomRight.Y - (boundingBoxTopLeft.Y)));
			}
		
		}

		public GameObject()
		{ }

		public virtual void Initialize()
		{
			direction = new Vector2(1, 0);
			color = Color.White;

#if DEBUG
			drawBoundingBoxes = true;
#endif
		}

		public virtual void Load(ContentManager content)
		{
			boundingBoxTexture = content.Load<Texture2D>("Sprites/Collision/s_wall");
			CalculateCenter();

			if (texture != null)
			{
				boundingBoxBottomRight.X = texture.Width;
				boundingBoxBottomRight.Y = texture.Height;
			}
		}

		public virtual void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{ }

		public virtual bool CheckCollision(Rectangle input)
		{
			return BoundingBox.Intersects(input);
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{	
			//draw bounding box
			if (boundingBoxTexture != null && drawBoundingBoxes == true && active == true) 
			{
				spriteBatch.Draw(boundingBoxTexture,
								new Vector2(BoundingBox.X, BoundingBox.Y),
								BoundingBox,
								new Color(255, 0, 0, 80),
								rotation,
								Vector2.Zero,
								1f,
								SpriteEffects.None,
								0.2f);
			}

			//draw object
			if (objectSprite != null && active == true)
			{
				spriteBatch.Draw(objectSprite);
			}
		}

		private void CalculateCenter()
		{
			if (texture == null) { return; }

			origin.X = texture.Width / 2;
			origin.Y = texture.Height / 2;
		}
	}
}
