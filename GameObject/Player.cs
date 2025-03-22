using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject.GameObject;

public class Player : GameCharacter
{
	private const float moveSpeed = 300f;
	private const float jumpForce = -500f;
	private bool canJump = false;
	private Texture2D _debugTexture;
	private string _lastState = "";

	public Player(Dictionary<string, Animation> animations, Vector2 position) : base(position)
	{
		spriteAnimations = animations;
		animationManager = new AnimationManager(spriteAnimations["Idle"]);

		if (spriteAnimations["Idle"]?.Texture != null)
			Bounds = new Rectangle((int)position.X, (int)position.Y,
								   spriteAnimations["Idle"].FrameWidth,
								   spriteAnimations["Idle"].FrameHeight);
		else
			throw new Exception("Player animation texture is null");
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector2 previousPosition = Position;

		// Horizontal movement
		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.A))
		{
			Velocity = new Vector2(-moveSpeed, Velocity.Y);
			currentAction = "WALK";
		}
		else if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.D))
		{
			Velocity = new Vector2(moveSpeed, Velocity.Y);
			currentAction = "WALK";
		}
		else
		{
			Velocity = new Vector2(0, Velocity.Y);
			currentAction = "IDLE";
		}

		// Jumping
		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Space) && canJump && isOnGround)
		{
			Velocity = new Vector2(Velocity.X, jumpForce);
			isOnGround = false;
			canJump = false;
			currentAction = "JUMP";
		}

		// Allow jump again if key is released
		if (Singleton.Instance.CurrentKey.IsKeyUp(Keys.Space))
		{
			canJump = true;
		}

		// Apply gravity
		if (!isOnGround)
			Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * dt);

		// Update position based on velocity
		HandleCollisions(platforms, dt);

		// Update animation based on current state
		string currentState = "IDLE";
		if (Math.Abs(Velocity.X) > 0.1f)
		{
			currentState = "WALK";
		}

		// Only change animation if state changed and we have the animation
		if (currentState != _lastState && DragonBonesCharacter != null)
		{
			DragonBonesCharacter.PlayAnimation(currentState);
			_lastState = currentState;
			Console.WriteLine($"Changing animation to: {currentState}");
		}

		animationManager.Update(gameTime);

		// Base update handles DragonBones if available
		UpdateDragonBonesCharacter();
	}

	protected override void HandleCollisions(List<Rectangle> platforms, float dt)
	{
		Vector2 previousPosition = Position;

		// Move horizontally first
		Position = new Vector2(Position.X + Velocity.X * dt, Position.Y);
		Bounds = new Rectangle((int)Position.X, (int)Position.Y,
							 animationManager.Animation.FrameWidth,
							 animationManager.Animation.FrameHeight);

		// Check horizontal collisions
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				// Moving right
				if (Velocity.X > 0)
				{
					Position = new Vector2(platform.Left - Bounds.Width, Position.Y);
				}
				// Moving left
				else if (Velocity.X < 0)
				{
					Position = new Vector2(platform.Right, Position.Y);
				}
				Velocity = new Vector2(0, Velocity.Y);
				break;
			}
		}

		// Move vertically
		Position = new Vector2(Position.X, Position.Y + Velocity.Y * dt);
		Bounds = new Rectangle((int)Position.X, (int)Position.Y,
							 animationManager.Animation.FrameWidth,
							 animationManager.Animation.FrameHeight);

		// Check vertical collisions
		isOnGround = false;
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				// Moving down
				if (Velocity.Y > 0)
				{
					if (previousPosition.Y + Bounds.Height <= platform.Y + 5)
					{
						Position = new Vector2(Position.X, platform.Y - Bounds.Height);
						Velocity = new Vector2(Velocity.X, 0);
						isOnGround = true;
						canJump = true;
					}
				}
				// Moving up
				else if (Velocity.Y < 0)
				{
					Position = new Vector2(Position.X, platform.Bottom);
					Velocity = new Vector2(Velocity.X, 0);
				}
				break;
			}
		}

		// Update bounds after collision resolution
		Bounds = new Rectangle((int)Position.X, (int)Position.Y, 
		                     animationManager.Animation.FrameWidth, 
		                     animationManager.Animation.FrameHeight);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		// Only draw ONE of these options - with clear priority:
		
		// Option 1: DragonBones (Skeletal Animation)
		if (DragonBonesCharacter != null)
		{
			try
			{
				// Update the character's position to match the player
				DragonBonesCharacter.Position = Position;
				
				// Ensure FlipX is correctly set based on movement direction
				if (Velocity.X != 0)
				{
					DragonBonesCharacter.FlipX = Velocity.X < 0;
				}
				
				DragonBonesCharacter.Draw(spriteBatch);
				
				// Only draw debug info if enabled
				if (Singleton.Instance.ShowDebugInfo && _debugTexture != null)
				{
					// Just draw a simple outline for debugging
					spriteBatch.Draw(_debugTexture, Bounds, Color.Red * 0.3f);
				}
				return; // Stop here - don't render anything else!
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error drawing DragonBones: {ex.Message}");
				// Fall through to next option
			}
		}
		else
		{
			// Log that DragonBones is missing if debug is enabled
			if (Singleton.Instance.ShowDebugInfo)
			{
				Console.WriteLine("DragonBonesCharacter is null, falling back to sprite animation");
			}
		}
		
		// Option 2: Sprite Animation (Fallback)
		if (animationManager != null && animationManager.Animation != null)
		{
			bool flip = Velocity.X < 0;
			animationManager.Draw(spriteBatch, Position, flip);
			
			// Only draw debug info if enabled
			if (Singleton.Instance.ShowDebugInfo && _debugTexture != null)
			{
				spriteBatch.Draw(_debugTexture, Bounds, Color.Green * 0.3f);
			}
			return; // Stop here - don't draw debug shapes
		}
		
		// Option 3: Last resort - just draw a colored rectangle
		if (_debugTexture != null)
		{
			// Draw a larger, more visible rectangle
			Rectangle playerRect = new Rectangle(
				(int)Position.X, 
				(int)Position.Y, 
				60, 
				100
			);
			spriteBatch.Draw(_debugTexture, playerRect, Color.Magenta);
			
			// Add text to show position
			if (Singleton.Instance.Font != null)
			{
				string debugInfo = $"Player ({Position.X}, {Position.Y})";
				spriteBatch.DrawString(
					Singleton.Instance.Font,
					debugInfo,
					new Vector2(Position.X, Position.Y - 20),
					Color.White
				);
			}
		}
	}

	public bool IsJumping => !isOnGround;
	public bool IsMoving => Math.Abs(Velocity.X) > 0.1f;
	public float Direction => Velocity.X; // Positive for right, negative for left

	public void SetDebugTexture(Texture2D texture)
	{
		_debugTexture = texture;
	}

	protected override void UpdateDragonBonesCharacter()
	{
		if (DragonBonesCharacter != null)
		{
			// Add this for debug
			if (DragonBonesCharacter.Armature == null)
			{
				Console.WriteLine("WARNING: Armature is null!");
				return;
			}
			
			Console.WriteLine($"Animation names: {string.Join(", ", DragonBonesCharacter.Armature.animation.animationNames)}");
			
			// Update DragonBones character position to match the player position
			DragonBonesCharacter.Position = Position;
			
			// Only use the animations you actually have
			if (Math.Abs(Velocity.X) > 0.1f)
			{
				DragonBonesCharacter.PlayAnimation("WALK");
				DragonBonesCharacter.FlipX = Velocity.X < 0;
			}
			else
			{
				DragonBonesCharacter.PlayAnimation("IDLE");
			}
		}
	}
}
