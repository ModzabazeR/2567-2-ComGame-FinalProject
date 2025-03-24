// using System.Collections.Generic;
// using Microsoft.Xna.Framework;

// namespace FinalProject.GameObject.Entity.Enemy;

// public abstract class Enemy : Movable
// {
// 	protected const float gravity = 1000f;

// 	protected Enemy(Dictionary<string, Animation> animations, Vector2 position)
// 		: base(animations, position)
// 	{
// 	}

// 	public override void Update(GameTime gameTime, List<Rectangle> platforms)
// 	{
// 		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
// 		ApplyGravity(dt);
// 		UpdateFacingDirection(Velocity.X);
// 		base.Update(gameTime, platforms);
// 	}

// 	protected void ApplyGravity(float dt)
// 	{
// 		if (!isOnGround)
// 			Velocity.Y += gravity * dt;
// 	}

// 	protected void HandleVerticalCollisions(List<Rectangle> platforms)
// 	{
// 		isOnGround = false;
// 		foreach (Rectangle platform in platforms)
// 		{
// 			if (Bounds.Intersects(platform))
// 			{
// 				if (Velocity.Y > 0)
// 				{
// 					Position.Y = platform.Y - Bounds.Height;
// 					Velocity.Y = 0;
// 					isOnGround = true;
// 				}
// 				else if (Velocity.Y < 0)
// 				{
// 					Position.Y = platform.Bottom;
// 					Velocity.Y = 0;
// 				}
// 				break;
// 			}
// 		}
// 	}
// }