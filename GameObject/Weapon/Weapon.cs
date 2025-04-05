using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FinalProject.GameObject.Weapon;

public abstract class Weapon : Entity.Entity
{
	protected float damage;
	protected float cooldown;
	protected float currentCooldown;
	protected bool isAttacking;

	public virtual Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 30, 20);

	public Weapon(Vector2 position) : base(position)
	{
		currentCooldown = 0;
		isAttacking = false;
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		base.Update(gameTime, platforms);

		if (currentCooldown > 0)
		{
			currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}
	}

	public virtual void Attack()
	{
		if (currentCooldown <= 0)
		{
			isAttacking = true;
			currentCooldown = cooldown;
			PerformAttack();
		}
	}

	protected abstract void PerformAttack();

	public virtual void StopAttack()
	{
		isAttacking = false;
	}

	public float GetDamage()
	{
		return damage;
	}
}