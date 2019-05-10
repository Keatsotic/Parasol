using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Parasol.Systems;
using MonoGame.Extended.Sprites;


namespace Parasol
{
	public class Entity : GameObject
	{ 

		//life vars
		protected int health = 5;

		//movement vars
		public Vector2 velocity;
		protected float accel = 0.75f;
		public float friction = 1.2f;
		public float maxSpeed = 1.5f;

		//jumping vars
		protected float gravity = .3f;
		const float terminalVelocity = 18.0f;
		protected bool isJumping;
		const float jumpHeight = 6.0f;

		//choose whether graviy affects or not
		public bool applyGravity = true;

		//init collision object
		Collision collision = new Collision();

		public override void Initialize()
		{
			velocity = Vector2.Zero;
			gravity = gravity * scale;
			isJumping = false;
			base.Initialize();
		}

		public override void Update(List<GameObject> objects, WallMap wallMap, GameTime gametime)
		{
			UpdateMovement(objects, wallMap);

			//reset jumping bool
			if (OnGround(wallMap) != Rectangle.Empty)
			{
				isJumping = false;
			}
			base.Update(objects, wallMap, gametime);
		}

		#region Update Movement Checking Methods

		private void UpdateMovement(List<GameObject> objects, WallMap wallMap)
		{
			if(canCollide == true && (velocity.X != 0) && collision.CheckCollisions(this, wallMap, objects, true) == true)
			{
				velocity.X = 0;
			}
			position.X += velocity.X;

			if (canCollide == true && (velocity.Y != 0) && collision.CheckCollisions(this, wallMap, objects, false) == true)
			{
				velocity.Y = 0;
			}
			position.Y += velocity.Y;

			//check if should apply gravity
			if (applyGravity == true) 
			{
				ApplyGravity(wallMap);
			}

			velocity.X = TendToZero(velocity.X, friction);

			// friction on Y if not using gravity
			if (applyGravity == false)
			{
				velocity.Y = TendToZero(velocity.Y, friction);
			}

		}

		public void ApplyGravity(WallMap wallMap)
		{
			if (isJumping == true || OnGround(wallMap) == Rectangle.Empty)
			{
				velocity.Y += gravity;

				if (velocity.Y > terminalVelocity)
				{
					velocity.Y = terminalVelocity;
				}
			}
		}

		#endregion

		#region  Movement Action Methods

		protected void MoveRight()
		{
			velocity.X += accel + friction;

			if (velocity.X > maxSpeed) 
			{
				velocity.X = maxSpeed;
			}
			direction.X = 1;
		}

		protected void MoveLeft()
		{
			velocity.X -= accel + friction;

			if (velocity.X < -maxSpeed)
			{
				velocity.X = -maxSpeed;
			}
			direction.X = -1;
		}

		protected void MoveDown()
		{
			velocity.Y += accel + friction;

			if (velocity.Y > maxSpeed)
			{
				velocity.Y = maxSpeed;
			}
			direction.Y = 1;
		}

		protected void MoveUp()
		{
			velocity.Y -= accel + friction;

			if (velocity.Y < -maxSpeed)
			{
				velocity.Y = -maxSpeed;
			}
			direction.Y = -1;
		}

		protected bool Jump(WallMap wallmap)
		{
			if (isJumping == true) { return false; }
			var testRect = OnGround(wallmap);
			if (velocity.Y <= gravity && testRect != Rectangle.Empty)
			{
				velocity.Y -= jumpHeight;
				isJumping = true;
				return true;
			}
			return false;
		}

		#endregion

		#region Helper Functions

		protected Rectangle OnGround(WallMap wallMap)
		{
			Rectangle futureBoundingBox = new Rectangle((int)((BoundingBox.X)),
														(int)((BoundingBox.Y ) + (1)),
														(int)(BoundingBox.Width),
														(int)(BoundingBox.Height));


			return wallMap.CheckCollision(futureBoundingBox);
		}

		public float TendToZero(float val, float amount)
		{
			if (val > 0.0f && (val -= amount) < 0.0f) return 0f;
			if (val < 0.0f && (val += amount) > 0.0f) return 0f;
			return val;
		}
		#endregion
	}
}
