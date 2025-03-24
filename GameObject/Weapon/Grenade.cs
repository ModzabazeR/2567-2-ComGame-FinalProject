using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FinalProject.GameObject.Weapon;

public abstract class Grenade : Weapon
{
	protected float explosionRadius;
	protected float fuseTime;
	protected float currentFuseTime;
	protected bool isThrown;
	protected bool hasExploded;

	public Grenade(Vector2 position) : base(position)
	{
		explosionRadius = 100f; // Default explosion radius
		fuseTime = 3f; // Default fuse time in seconds
		currentFuseTime = 0f;
		isThrown = false;
		hasExploded = false;
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		base.Update(gameTime, platforms);

		if (isThrown && !hasExploded)
		{
			currentFuseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (currentFuseTime >= fuseTime)
			{
				Explode();
			}
		}
	}

	protected override void PerformAttack()
	{
		if (!isThrown)
		{
			isThrown = true;
			// Implement throwing logic here
		}
	}

	protected virtual void Explode()
	{
		hasExploded = true;
		// Implement explosion logic here
	}

	public float GetExplosionRadius()
	{
		return explosionRadius;
	}

	public float GetFuseTime()
	{
		return fuseTime;
	}

	public float GetCurrentFuseTime()
	{
		return currentFuseTime;
	}

	public bool IsThrown()
	{
		return isThrown;
	}

	public bool HasExploded()
	{
		return hasExploded;
	}
}