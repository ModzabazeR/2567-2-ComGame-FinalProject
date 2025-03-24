using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.GameObject.Weapon;
using Spine;

namespace FinalProject.GameObject.Entity;

public class Player : Movable
{
	private SkeletonRenderer skeletonRenderer;
	private const float moveSpeed = 300f;
	private const float gravity = 1000f;
	private const float jumpForce = -500f;
	private bool canJump = false;
	private Weapon.Weapon _primaryWeapon;
	private Weapon.Weapon _secondaryWeapon;
	private Weapon.Weapon _currentWeapon;

	public Player(Skeleton skeleton, AnimationState animationState, Vector2 position, GraphicsDevice graphicsDevice)
	: base(position)
	{
		this.skeleton = skeleton;
		this.animationState = animationState;
		this.skeletonRenderer = new SkeletonRenderer(graphicsDevice); // Initialize here

		this.skeleton.X = position.X;
		this.skeleton.Y = position.Y;
		this.skeleton.UpdateWorldTransform();
	}
	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector2 previousPosition = Position;

		HandleInput();
		ApplyGravity(dt);
		HandleMovement(dt, platforms, previousPosition);

		// Update skeleton position
		skeleton.X = Position.X;
		skeleton.Y = Position.Y;


		base.Update(gameTime, platforms);
	}

	private void HandleInput()
	{
		if (Keyboard.GetState().IsKeyDown(Keys.A))
		{
			Velocity.X = -moveSpeed;
			animationState.SetAnimation(0, "WALK", true);
		}
		else if (Keyboard.GetState().IsKeyDown(Keys.D))
		{
			Velocity.X = moveSpeed;
			animationState.SetAnimation(0, "WALK", true);
		}
		else
		{
			Velocity.X = 0;
			animationState.SetAnimation(0, "IDLE", true);
		}

		if (canJump && Keyboard.GetState().IsKeyDown(Keys.W))
		{
			Velocity.Y = jumpForce;

			canJump = false;
		}

		UpdateFacingDirection(Velocity.X);
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

	public override void Draw(SpriteBatch spriteBatch)
	{

		// Use Spine renderer instead of base draw
		skeletonRenderer.Begin();
		skeletonRenderer.Draw(skeleton);
		skeletonRenderer.End();
		base.Draw(spriteBatch);
	}
}
