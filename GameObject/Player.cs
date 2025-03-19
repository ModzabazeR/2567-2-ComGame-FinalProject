using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject.GameObject;

public class Player
{
	public Vector2 Position;
	public Vector2 Velocity;
	public Rectangle Bounds;
	private Texture2D texture;

	private AnimationManager _animationManager;
	private Dictionary<string, Animation> _animations;
	private string _currentAnimationKey;

	private const float moveSpeed = 300f;
	private const float gravity = 800f;
	private const float jumpForce = -400f;
	private bool isOnGround;
	private bool canJump = false;

	public Player(Dictionary<string, Animation> animations, Vector2 position)
	{
		_animations = animations;
		_currentAnimationKey = "Idle";
		_animationManager = new AnimationManager(_animations[_currentAnimationKey]);
		Position = position;

		if (_animations[_currentAnimationKey]?.Texture != null)
			Bounds = new Rectangle((int)position.X, (int)position.Y,
								   _animations[_currentAnimationKey].FrameWidth,
								   _animations[_currentAnimationKey].FrameHeight);
		else
			throw new System.Exception("Player animation texture is null");

	}

	public void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector2 previousPosition = Position;

		// Horizontal movement
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


		// Jump input
		if (canJump && Singleton.Instance.CurrentKey.IsKeyDown(Keys.W) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.W))
		{
			Velocity.Y = jumpForce;
			canJump = false;
			isOnGround = false;
			_animationManager.Play(_animations["Jump"]);
		}

		// Apply gravity
		if (!isOnGround)
			Velocity.Y += gravity * dt;

		// Update position
		Position += Velocity * dt;

		// Update animation
		_animationManager.Update(gameTime);

		// Update bounds
		Bounds = new Rectangle((int)Position.X, (int)Position.Y, _animationManager.Animation.FrameWidth, _animationManager.Animation.FrameHeight);

		// Platform collision
		isOnGround = false;
		Rectangle futureBottomBounds = new Rectangle(
			Bounds.X,
			Bounds.Y + 1, // Check slightly below current position
			Bounds.Width,
			Bounds.Height
		);

		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				// Vertical collision
				if (Velocity.Y >= 0) // Moving downward or stationary
				{
					if (previousPosition.Y + Bounds.Height <= platform.Y + 5) // Was above platform
					{
						Position.Y = platform.Y - Bounds.Height;
						Velocity.Y = 0;
						isOnGround = true;
						canJump = true;
					}
				}
			}
			// Check if we're still on the platform
			else if (futureBottomBounds.Intersects(platform) && Velocity.Y >= 0)
			{
				isOnGround = true;
				canJump = true;
			}
		}

		// Update bounds after collision resolution
		Bounds = new Rectangle((int)Position.X, (int)Position.Y, _animationManager.Animation.FrameWidth, _animationManager.Animation.FrameHeight);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		bool flip = Velocity.X < 0;
		_animationManager.Draw(spriteBatch, Position, flip);

		// Draw bounding box for debugging
		DrawBoundingBox(spriteBatch);
	}

	private void DrawBoundingBox(SpriteBatch spriteBatch)
	{
		// Only draw debug visuals if debug mode is enabled
		if (!Singleton.Instance.ShowDebugInfo)
			return;

		// Create a 1x1 white texture that we can resize to draw the bounding box
		if (texture == null)
		{
			texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
			texture.SetData(new[] { Color.White });
		}

		// Draw the outline of the bounding box
		Color boundingBoxColor = Color.Red * 0.7f; // Semi-transparent red

		// Top line
		spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1), boundingBoxColor);
		// Bottom line
		spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y + Bounds.Height, Bounds.Width, 1), boundingBoxColor);
		// Left line
		spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height), boundingBoxColor);
		// Right line
		spriteBatch.Draw(texture, new Rectangle(Bounds.X + Bounds.Width, Bounds.Y, 1, Bounds.Height), boundingBoxColor);
	}
}
