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
		public float scale = 2.0f, rotation = 0.0f;
		public float layerDepth = 0.5f;
		public bool active = true;

		protected Vector2 center;

		public GameObject()
		{ }

		public virtual void Initialize()
		{
			color = Color.White;
		}

		public virtual void Load(ContentManager content)
		{

		}

		public virtual void Update(List<GameObject> objects)
		{
		
		}

		public virtual void Draw(SpriteBatch spritebatch)
		{
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
