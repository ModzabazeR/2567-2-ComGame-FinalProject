using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static FinalProject.GameObject.Entity.Bullet;

namespace FinalProject.GameObject.Weapon;

public class Crowbar : MeleeWeapon
{

	public Crowbar(Vector2 position) : base(position)
	{
		// Initialize melee weapon properties
		attackRange = 60f;
		damage = 20f;
		cooldown = 0.5f;
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		base.Update(gameTime, platforms);
	}

	protected override void PerformAttack()
	{
		var player = Singleton.Instance.Player;

		if (player == null) return;

		// Create melee hitbox relative to player's facing direction
		Rectangle attackArea = new Rectangle(
			(int)player.Position.X + (player.IsFacingRight ? 40 : -60),
			(int)player.Position.Y,
			60,
			80
		);

		// Check collisions with enemies
		var currentMap = Singleton.Instance.MapManager?.CurrentMap;
		if (currentMap != null)
		{
			foreach (var enemy in currentMap.GetEnemies())
			{
				if (attackArea.Intersects(enemy.Bounds))
				{
					enemy.TakeDamage((int)damage);
				}
			}
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		// Only draw if not being held by player
		if (texture == null)
		{
			texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
			texture.SetData(new[] { Color.OrangeRed });
		}
		spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, 40, 40), Color.White);
	}
}