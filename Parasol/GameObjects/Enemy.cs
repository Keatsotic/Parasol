using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using System;

namespace Parasol
{
	public class Enemy : Entity
	{ 
		
		public Enemy()
		{ }

		public override void Initialize()
		{
			objectType = "Enemy";
			base.Initialize();
			knockback = new Vector2(2f, 0);
			invincibleTimerMax = 30;
		}

		public override void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{
			base.Update(objects, wallMap, gametime);

			if (isHurt && !invincible)
			{
				health--;
				isHurt = false;
				invincible = true;
				invincibleTimer = invincibleTimerMax;
			}

			if (invincible)
			{
				invincibleTimer--;
				if (invincibleTimer % 4 == 0)
				{ objectAnimated.Color = new Color(255, 0, 0, 255); }
				else { objectAnimated.Color = new Color(125, 125, 125, 255); }

				if (invincibleTimer <= 0)
				{
					invincible = false;
					isHurt = false;
					objectAnimated.Color = new Color(255, 255, 255, 255);
				}
			}

		}

	}
}
