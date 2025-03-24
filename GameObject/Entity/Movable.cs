using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace FinalProject.GameObject.Entity;

public abstract class Movable : Entity
{
	protected string _currentAnimationKey;
	protected bool isOnGround;
	protected bool isFacingRight = true;
	protected float lastNonZeroVelocityX = 0;

	// Spine-related fields
	public Skeleton skeleton { get; protected set; }
	public AnimationState animationState { get; protected set; }
	public Skeleton physicsSkeleton { get; protected set; }

	public Movable(Vector2 position) : base(position)
	{
		// Initialize in child classes
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		// Update Spine animations
		if (animationState != null && skeleton != null)
		{
			animationState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			animationState.Apply(skeleton);
			skeleton.UpdateWorldTransform();
		}
		base.Update(gameTime, platforms);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		// _animationManager.Draw(spriteBatch, Position, !isFacingRight);
		base.Draw(spriteBatch);
	}

	protected void UpdateFacingDirection(float velocityX)
	{
		if (velocityX != 0)
		{
			lastNonZeroVelocityX = velocityX;
		}
		isFacingRight = lastNonZeroVelocityX >= 0;
	}

	protected override void UpdateBounds()
	{
		// Set bounds based on Spine skeleton size
		if (skeleton != null)
		{
			Bounds = new Rectangle(
				(int)Position.X,
				(int)Position.Y,
				(int)(skeleton.Data.Width * skeleton.X),
				(int)(skeleton.Data.Height * skeleton.Y)
			);
		}
	}
}
