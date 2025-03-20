using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.GameObject.Entity;

public abstract class Movable : Entity
{
	protected AnimationManager _animationManager;
	protected Dictionary<string, Animation> _animations;
	protected string _currentAnimationKey;
	protected bool isOnGround;

	public Movable(Dictionary<string, Animation> animations, Vector2 position)
		: base(position)
	{
		_animations = animations;
		_currentAnimationKey = "Idle";
		_animationManager = new AnimationManager(_animations[_currentAnimationKey]);

		if (_animations[_currentAnimationKey]?.Texture != null)
			Bounds = new Rectangle((int)position.X, (int)position.Y,
								 _animations[_currentAnimationKey].FrameWidth,
								 _animations[_currentAnimationKey].FrameHeight);
		else
			throw new System.Exception("Movable animation texture is null");
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		_animationManager.Update(gameTime);
		base.Update(gameTime, platforms);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		bool flip = Velocity.X < 0;
		_animationManager.Draw(spriteBatch, Position, flip);
		base.Draw(spriteBatch);
	}

	protected override void UpdateBounds()
	{
		Bounds = new Rectangle((int)Position.X, (int)Position.Y,
							 _animationManager.Animation.FrameWidth,
							 _animationManager.Animation.FrameHeight);
	}
}
