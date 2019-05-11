using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;

namespace Parasol
{		
	public class GameObject
	{
		protected Texture2D texture;
		public Vector2 position;
		public Color color;
		public float scale = 1.0f, rotation = 0.0f;
		public float layerDepth = 0.5f;

		public bool active = true;
		public bool canMove = true;

		protected Vector2 direction;
		protected Vector2 origin;

		protected Sprite objectSprite = null;

		//type identifier
		public string objectType = null;

		//will it be affected by walls
		public bool canCollide = true;

		//bounding boxes
		bool drawBoundingBoxes = false;

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
								0.1f);
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
