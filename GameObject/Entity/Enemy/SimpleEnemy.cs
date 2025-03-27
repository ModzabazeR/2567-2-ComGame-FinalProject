using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FinalProject.GameObject.Entity.Enemy;

public class SimpleEnemy : Enemy
{
	private const float moveSpeed = 150f;
	private bool movingRight = true;

	public SimpleEnemy(Dictionary<string, Animation> animations, Vector2 position)
		: base(animations, position)
	{
		Velocity.X = moveSpeed;
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		HandleMovement(dt, platforms);
		UpdateAnimation();

		base.Update(gameTime, platforms);
	}

	private void HandleMovement(float dt, List<Rectangle> platforms)
	{
		// Horizontal movement
		Position.X += Velocity.X * dt;
		UpdateBounds();

		// Check horizontal collisions
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				if (Velocity.X > 0)
					Position.X = platform.Left - Bounds.Width;
				else
					Position.X = platform.Right;

				ChangeDirection();
				break;
			}
		}

		// Vertical movement
		Position.Y += Velocity.Y * dt;
		UpdateBounds();
		HandleVerticalCollisions(platforms);
	}

	private void ChangeDirection()
	{
		movingRight = !movingRight;
		Velocity.X = movingRight ? moveSpeed : -moveSpeed;
	}

	private void UpdateAnimation()
	{
		if (Velocity.X != 0)
			_animationManager.Play(_animations["Walk"]);
		else
			_animationManager.Play(_animations["Idle"]);
	}
}
