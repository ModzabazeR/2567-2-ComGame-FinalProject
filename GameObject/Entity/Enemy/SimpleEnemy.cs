using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FinalProject.GameObject.Entity.Enemy;

public class SimpleEnemy : Enemy
{
	private const float moveSpeed = 150f;
	private int _health = 10; // Add health field
	private bool movingRight = true;

	public SimpleEnemy(Dictionary<string, Animation> animations, Vector2 position)
		: base(animations, position)
	{
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

		// Check if enemy is about to walk off the platform
		Rectangle footSensor = new Rectangle(
			(int)(Position.X + (movingRight ? Bounds.Width - 5 : 0)),
			(int)(Position.Y + Bounds.Height + 2),
			5,
			5
		);

		bool hasGroundAhead = false;
		foreach (Rectangle platform in platforms)
		{
			if (footSensor.Intersects(platform))
			{
				hasGroundAhead = true;
				break;
			}
		}

		if (!hasGroundAhead && isOnGround)
		{
			ChangeDirection();
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

	public override void Spawn()
	{
		base.Spawn();
		movingRight = true;
		Velocity.X = moveSpeed;
	}

	public override void TakeDamage(int amount)
	{
		_health -= amount;
		if (_health <= 0)
		{
			Defeat();
		}
	}
}
