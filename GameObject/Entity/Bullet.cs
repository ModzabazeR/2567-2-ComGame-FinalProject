using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalProject.GameObject.Entity;

public class Bullet : Entity
{
	private float damage;
	private float speed;
	private Vector2 direction;
	private bool isActive;
	private float lifetime;
	private float currentLifetime;

	public Bullet(Vector2 position, Vector2 direction, float speed, float damage, float lifetime)
		: base(position)
	{
		this.direction = direction;
		this.speed = speed;
		this.damage = damage;
		this.lifetime = lifetime;
		this.currentLifetime = 0f;
		this.isActive = true;
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		base.Update(gameTime, platforms);

		if (!isActive) return;

		// Update position based on direction and speed
		Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

		// Update lifetime
		currentLifetime += (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (currentLifetime >= lifetime)
		{
			isActive = false;
		}

		// Check for collisions with platforms
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				isActive = false;
				break;
			}
		}
	}

	public float GetDamage()
	{
		return damage;
	}

	public bool IsActive()
	{
		return isActive;
	}

	public void Deactivate()
	{
		isActive = false;
	}
}