using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static FinalProject.GameObject.Entity.Bullet;
using FinalProject.Utils.SFXManager;

namespace FinalProject.GameObject.Weapon;

public class Crowbar : MeleeWeapon
{

	public Crowbar(Vector2 position) : base(position)
	{
		// Initialize melee weapon properties
		attackRange = 60f;
		damage = 3;
		cooldown = 1f;
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		base.Update(gameTime, platforms);
	}

	public override void PerformAttack()
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
		Console.WriteLine("Play attack sound");
		SFXManager.Instance.PlaySound("DesignedPunch1");

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
		if (EntityTexture == null)
		{
			EntityTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
			EntityTexture.SetData(new[] { Color.OrangeRed });
		}
		spriteBatch.Draw(EntityTexture, new Rectangle((int)Position.X, (int)Position.Y, EntityTexture.Width, EntityTexture.Height), Color.White);
	}
}