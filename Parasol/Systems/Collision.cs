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

namespace Parasol.Systems
{
	class Collision 
	{
		public virtual bool CheckCollisions(Entity entity, 
											WallMap wallMap, 
											List<GameObject> objects, 
											bool xAxis)
		{
			Rectangle futureBoundingBox = entity.BoundingBox;

			if (xAxis == true && entity.velocity.X != 0)
			{
				futureBoundingBox.X += (int)Math.Ceiling(entity.maxSpeed) * Math.Sign(entity.velocity.X);

				// pixel perfect x axis collisions
				Rectangle wallCollisionX = wallMap.CheckCollision(futureBoundingBox);

				if (wallCollisionX != Rectangle.Empty) //if there is a collision xspd away from player...
				{
					var hSpd_ = Math.Sign(entity.velocity.X);
					Rectangle pixelBoundingBox = entity.BoundingBox;
					
					var pixelCollision = wallMap.CheckCollision(pixelBoundingBox);
					while (pixelCollision == Rectangle.Empty)
					{
						pixelBoundingBox.X += hSpd_;
						pixelCollision = wallMap.CheckCollision(pixelBoundingBox);

						if (pixelCollision == Rectangle.Empty)
						{
							entity.position.X += hSpd_;
						}
					}
				
					hSpd_ = 0;
					return true;

				}

			}
			else if (xAxis == false && entity.velocity.Y != 0)
			{
				futureBoundingBox.Y += (int)Math.Ceiling(entity.velocity.Y);

				// pixel perfect y axis collisions
				Rectangle wallCollisionY = wallMap.CheckCollision(futureBoundingBox);

				if (wallCollisionY != Rectangle.Empty)
				{
					var vSpd_ = Math.Sign(entity.velocity.Y);

					Rectangle pixelBoundingBox = entity.BoundingBox;
					pixelBoundingBox.Y += vSpd_;

					var pixelCollision = wallMap.CheckCollision(pixelBoundingBox);
					while (pixelCollision == Rectangle.Empty)
					{
						pixelBoundingBox.Y += vSpd_;
						pixelCollision = wallMap.CheckCollision(pixelBoundingBox);
						if (pixelCollision == Rectangle.Empty)
						{
							entity.position.Y += vSpd_;
						}
					}
					vSpd_ = 0;
					return true;

				}

			}


			for (int i = 0; i < objects.Count; i++)
			{
				if (objects[i] != entity &&
					objects[i].active == true &&
					objects[i].canCollide == true &&
					objects[i].CheckCollision(futureBoundingBox) == true)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateMovement(Entity entity, List<GameObject> objects, WallMap wallMap)
		{
			if ((entity.velocity.X != 0) && CheckCollisions(entity, wallMap, objects, true) == true)
			{
				entity.velocity.X = 0;
			}
			entity.position.X += entity.velocity.X;

			if ((entity.velocity.Y != 0) && CheckCollisions(entity, wallMap, objects, false) == true)
			{
				entity.velocity.Y = 0;
			}
			entity.position.Y += entity.velocity.Y;

			//check if should apply gravity
			if (entity.applyGravity == true)
			{
				entity.ApplyGravity(wallMap);
			}

			entity.velocity.X = entity.TendToZero(entity.velocity.X, entity.friction);

			// friction on Y if not using gravity
			if (entity.applyGravity == false)
			{
				entity.velocity.Y = entity.TendToZero(entity.velocity.Y, entity.friction);
			}

		}

	}
}
