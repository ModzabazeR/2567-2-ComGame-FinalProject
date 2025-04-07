using System;
using System.Collections.Generic;
using FinalProject.Utils.SFXManager;
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

	public int enemyHP = 10;

	public float invincibilityTime = 1.0f;

	public float invincibilityTimer = 0f;
	public bool isInvincible => invincibilityTimer > 0;

	private Map parentMap;

	private float screamTimer = 0f;

	private const float FOOTSTEP_INTERVAL = 5f; // seconds

	protected Enemy(Dictionary<string, Animation> animations, Vector2 position)
		: base(animations, position)
	{
	}

	public abstract void TakeDamage(int amount);

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		if (!_isSpawned)
			return;

		if (_isDefeated)
			return;

		if (invincibilityTimer > 0)
		{
			invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

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

		// if (this is Boss)
		// {
		// 	Texture2D hitboxTex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
		// 	hitboxTex.SetData(new[] { Color.Red });

		// 	// วาด hitbox ที่ปรับให้ตรง sprite จริง
		// 	var boss = this as Boss;

		// 	Rectangle hitbox = new Rectangle(
		// 		(int)(Position.X + 100), // offset X เข้ากลาง
		// 		(int)(Position.Y), // offset Y ลงมาให้ถึงขา
		// 		400,                     // กว้าง
		// 		500                      // สูง
		// 	);

		// 	spriteBatch.Draw(hitboxTex, hitbox, Color.Red * 0.4f);
		// }
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
				drop = new PistolBulletItem(Position) { EntityTexture = Singleton.Instance.PistolAmmoTexture };
			else
				drop = new ShotgunBulletItem(Position) { EntityTexture = Singleton.Instance.ShotgunAmmoTexture };

			parentMap.GetWeapons().Add(drop);
		}
	}

	public virtual void hit(int i)
	{
		enemyHP = enemyHP - i;
		Console.WriteLine("this is enemyHP : " + enemyHP);
		if (enemyHP <= 0)
		{
			Defeat();
		}
		invincibilityTimer = invincibilityTime;
	}

	public void SetParentMap(Map map)
	{
		this.parentMap = map;
	}
}