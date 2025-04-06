using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.Utils.MapManager;
using FinalProject.GameObject.Weapon;

namespace FinalProject.GameObject.Entity.Enemy;

public abstract class Enemy : Movable
{
	protected const float gravity = 1000f;
	private bool _isSpawned = false;
	public bool IsSpawned => _isSpawned;
	private bool _isDefeated = false;
	public bool IsDefeated => _isDefeated;

	public int enemyHP = 5;

	private Map parentMap;

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
		Console.WriteLine("enemy Defeat ");
			// ✅ Drop item
		if (parentMap != null)
		{
			Random rng = new Random();
			int dropType = rng.Next(2); // 0 = pistol, 1 = shotgun
			Console.WriteLine(dropType);

			Weapon.Weapon drop;
			if (dropType == 0)
				drop = new PistolBulletItem(Position);
			else
				drop = new ShotgunBulletItem(Position);

			parentMap.GetWeapons().Add(drop);
		}
	}

	public virtual void hit(int i)
	{
		enemyHP = enemyHP - i;
		if (enemyHP <= 0){
			Defeat();
		}
	}

	public void SetParentMap(Map map)
	{
		this.parentMap = map;
	}
}