using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FinalProject.GameObject.Entity;

public class Player : Movable
{
	private const float moveSpeed = 300f;
	private const float gravity = 1000f;
	private const float jumpForce = -500f;
	private bool canJump = false;

	public Player(Dictionary<string, Animation> animations, Vector2 position)
		: base(animations, position)
	{
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector2 previousPosition = Position;

		HandleInput();
		ApplyGravity(dt);
		HandleMovement(dt, platforms, previousPosition);

		base.Update(gameTime, platforms);
	}

	private void HandleInput()
	{
		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.A))
		{
			Velocity.X = -moveSpeed;
			_animationManager.Play(_animations["Run"]);
		}
		else if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.D))
		{
			Velocity.X = moveSpeed;
			_animationManager.Play(_animations["Run"]);
		}
		else
		{
			Velocity.X = 0;
			_animationManager = new AnimationManager(_animations["Idle"]);
		}

		UpdateFacingDirection(Velocity.X);

		if (canJump && Singleton.Instance.CurrentKey.IsKeyDown(Keys.W) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.W))
		{
			Velocity.Y = jumpForce;
			canJump = false;
			isOnGround = false;
			_animationManager.Play(_animations["Jump"]);
		}
	}

	private void ApplyGravity(float dt)
	{
		if (!isOnGround)
			Velocity.Y += gravity * dt;
	}

	private void HandleMovement(float dt, List<Rectangle> platforms, Vector2 previousPosition)
	{
		// Horizontal movement
		Position.X += Velocity.X * dt;
		UpdateBounds();

		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				if (Velocity.X > 0)
					Position.X = platform.Left - Bounds.Width;
				else if (Velocity.X < 0)
					Position.X = platform.Right;
				Velocity.X = 0;
				break;
			}
		}

		// Vertical movement
		Position.Y += Velocity.Y * dt;
		UpdateBounds();

		isOnGround = false;
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				if (Velocity.Y > 0)
				{
					if (previousPosition.Y + Bounds.Height <= platform.Y + 5)
					{
						Position.Y = platform.Y - Bounds.Height;
						Velocity.Y = 0;
						isOnGround = true;
						canJump = true;
					}
				}
				else if (Velocity.Y < 0)
				{
					Position.Y = platform.Bottom;
					Velocity.Y = 0;
				}
				break;
			}
		}
	}
}
