using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

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
		protected Vector2 direction;

		public bool canCollide = true;
		protected Vector2 boundingBoxTopLeft, boundingBoxBottomRight;

		Texture2D boundingBoxTexture;
		const bool drawBoundingBoxes = true;
		

		public Rectangle BoundingBox
		{
			get 
			{
				return new Rectangle((int)(position.X + (boundingBoxTopLeft.X * scale)),
									(int)(position.Y + (boundingBoxTopLeft.Y * scale)),
									(int)((boundingBoxBottomRight.X - (boundingBoxTopLeft.X)) * scale),
									(int)((boundingBoxBottomRight.Y - (boundingBoxTopLeft.Y)) * scale));
			}
		
		}

		protected Vector2 center;

		public GameObject()
		{ }

		public virtual void Initialize()
		{
			direction = new Vector2(1, 0);
			color = Color.White;
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

		public virtual void Update(List<GameObject> objects, WallMap wallMap)
		{ }

		public virtual bool CheckCollision(Rectangle input)
		{
			return BoundingBox.Intersects(input);
		}

		public virtual void Draw(SpriteBatch spritebatch)
		{	
			//draw bounding box
			if (boundingBoxTexture != null && drawBoundingBoxes == true && active == true) 
			{
				spritebatch.Draw(boundingBoxTexture,
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
			if (texture != null && active == true)
			{
				spritebatch.Draw(texture,
								position,
								null,
								color,
								rotation,
								Vector2.Zero,
								scale,
								SpriteEffects.None,
								layerDepth);
			}
		}

		private void CalculateCenter()
		{
			if (texture == null) { return; }

			center.X = texture.Width / 2;
			center.Y = texture.Height / 2;
		}
	}
}
