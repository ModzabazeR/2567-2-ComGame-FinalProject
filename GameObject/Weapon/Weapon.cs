using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.GameObject.Weapon;

public abstract class Weapon : Entity.Entity
{
	protected float damage;
	protected float cooldown;
	protected float currentCooldown;
	protected bool isAttacking;

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

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (EntityTexture != null)
		{
			spriteBatch.Draw(EntityTexture, new Rectangle((int)Position.X, (int)Position.Y, 40, 40), Color.White);
			DrawBoundingBox(spriteBatch);
		}
		else
		{
			// Fallback to solid color if texture isn't set
			base.Draw(spriteBatch);
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