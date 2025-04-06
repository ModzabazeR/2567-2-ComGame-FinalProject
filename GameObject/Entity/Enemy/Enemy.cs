using System;
using System.Collections.Generic;
using FinalProject.Utils.SFXManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.GameObject.Entity.Enemy;

public abstract class Enemy : Movable
{
	protected const float gravity = 1000f;
	private bool _isSpawned = false;
	public bool IsSpawned => _isSpawned;
	private bool _isDefeated = false;
	public bool IsDefeated => _isDefeated;

	private float screamTimer = 0f;

	private const float FOOTSTEP_INTERVAL = 5f; // seconds
	



	protected Enemy(Dictionary<string, Animation> animations, Vector2 position)
		: base(animations, position)
	{
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		if (!_isSpawned)
			return;

		if (_isDefeated)
			return;

		// play sound effect when enemy is spawned repeatedly around 5 seconds
		if (screamTimer <= 0f)
		{
			SFXManager.Instance.RandomPlayZombieScream();
			screamTimer = FOOTSTEP_INTERVAL;
		}
		else
		{
			screamTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		ApplyGravity(dt);
		UpdateFacingDirection(Velocity.X);
		base.Update(gameTime, platforms);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (!_isSpawned)
			return;

		if (_isDefeated)
			return;

		base.Draw(spriteBatch);
	}

	protected void ApplyGravity(float dt)
	{
		if (!isOnGround)
			Velocity.Y += gravity * dt;
	}

	protected void HandleVerticalCollisions(List<Rectangle> platforms)
	{
		isOnGround = false;
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				if (Velocity.Y > 0)
				{
					Position.Y = platform.Y - Bounds.Height;
					Velocity.Y = 0;
					isOnGround = true;
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

	public virtual void Spawn()
	{
		Velocity = Vector2.Zero;
		_isSpawned = true;
	}

	public virtual void Despawn()
	{
		_isSpawned = false;
		Velocity = Vector2.Zero;
	}

	public virtual void Defeat()
	{
		_isDefeated = true;
		Despawn();
	}
}