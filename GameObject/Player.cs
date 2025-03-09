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
	private const float moveSpeed = 300f;
	private const float gravity = 800f;
	private const float jumpForce = -400f;
	private bool isOnGround;
	private bool canJump = false;

	public Player(Texture2D texture, Vector2 position)
	{
		this.texture = texture;
		Position = position;
		Bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
	}

	public void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector2 previousPosition = Position;

		// Horizontal movement
		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Left))
			Velocity.X = -moveSpeed;
		else if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Right))
			Velocity.X = moveSpeed;
		else
			Velocity.X = 0;

		// Jump input
		if (canJump && Singleton.Instance.CurrentKey.IsKeyDown(Keys.Space) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.Space))
		{
			Velocity.Y = jumpForce;
			canJump = false;
			isOnGround = false;
		}

		// Apply gravity
		if (!isOnGround)
			Velocity.Y += gravity * dt;

		// Update position
		Position += Velocity * dt;

		// Update bounds
		Bounds = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);

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
		Bounds = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, Position, Color.White);
	}
}
