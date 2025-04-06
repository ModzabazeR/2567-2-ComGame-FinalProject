using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.GameObject.Weapon;
using System;

namespace FinalProject.GameObject.Entity;

public class Player : Movable
{
	private const float moveSpeed = 300f;
	private const float gravity = 1000f;
	private const float jumpForce = -500f;
	private bool canJump = false;
	private Weapon.Weapon _primaryWeapon;
	private Weapon.Weapon _secondaryWeapon;
	private Weapon.Weapon _currentWeapon;

	private int maxHP = 10;
	private int currentHP = 10;

	public int CurrentHP => currentHP;
	public int MaxHP => maxHP;
	public bool IsAlive => currentHP > 0;

	private float invincibilityTime = 1.0f; // ระยะเวลาอมตะหลังโดนตี (วินาที)
	private float invincibilityTimer = 0f;
	private bool isInvincible => invincibilityTimer > 0;

	//กระสุน
	private List<Bullet> bullets = new(); // เก็บกระสุนที่ยิงออกไป
	//โจมตี
	private bool isAttacking = false;
	private float attackDuration = 0.3f; // ความยาวการโจมตี
	private float attackTimer = 0f;
	public bool IsAttacking => isAttacking;
	

	public Player(Dictionary<string, Animation> animations, Vector2 position)
		: base(animations, position)
	{
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector2 previousPosition = Position;

		HandleInput();
		ApplyGravity(dt);
		HandleMovement(dt, platforms, previousPosition);

		base.Update(gameTime, platforms);

		if (invincibilityTimer > 0)
		{
			invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		// โจมตี
		if (isAttacking)
		{
			attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (attackTimer <= 0)
				isAttacking = false;
		}
	}

	private void HandleInput()
	{
		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.A))
		{
			Velocity.X = -moveSpeed;
			_animationManager.Play(_animations["Sprint"]);
		}
		else if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.D))
		{
			Velocity.X = moveSpeed;
			_animationManager.Play(_animations["Sprint"]);
		}
		else
		{
			Velocity.X = 0;
			_animationManager = new AnimationManager(_animations["Idle"]);
		}

		UpdateFacingDirection(Velocity.X);

		if (canJump && Singleton.Instance.CurrentKey.IsKeyDown(Keys.W) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.W))
		{
			Velocity.Y = jumpForce;
			canJump = false;
			isOnGround = false;
			_animationManager.Play(_animations["Walk"]);
		}

		// Handle weapon input
		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.C) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.C))
		{
			if (_currentWeapon != null && _currentWeapon is Crowbar)
			{
				isAttacking = true;
				attackTimer = attackDuration;
			}
			else
			{
				Console.WriteLine("You don't have a weapon to attack!");
			}
		}

		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.V) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.V))
		{
			if (_currentWeapon != null && _currentWeapon is Shotgun)
			{
				isAttacking = true;
				attackTimer = attackDuration;
			}
			else
			{
				Console.WriteLine("You don't have a weapon to attack!");
			}
		}

		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.K) &&
    	Singleton.Instance.PreviousKey.IsKeyUp(Keys.K))
		{
			Console.WriteLine("bullet shoot");
			Vector2 direction = isFacingRight ? Vector2.UnitX : -Vector2.UnitX;
			Vector2 spawnOffset = new Vector2(isFacingRight ? Bounds.Width : -12, 20);
			Vector2 spawnPos = Position + spawnOffset;

			var bullet = new Bullet(spawnPos, direction, speed: 700f, damage: 1f, lifetime: 2f);
			bullets.Add(bullet);
		}

	}

	private void ApplyGravity(float dt)
	{
		if (!isOnGround)
			Velocity.Y += gravity * dt;
	}

	private void HandleMovement(float dt, List<Rectangle> platforms, Vector2 previousPosition)
	{
		// Horizontal movement
		Position.X += Velocity.X * dt;
		UpdateBounds();

		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				if (Velocity.X > 0)
					Position.X = platform.Left - Bounds.Width;
				else if (Velocity.X < 0)
					Position.X = platform.Right;
				Velocity.X = 0;
				break;
			}
		}

		// Vertical movement
		Position.Y += Velocity.Y * dt;
		UpdateBounds();

		isOnGround = false;
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				if (Velocity.Y > 0)
				{
					if (previousPosition.Y + Bounds.Height <= platform.Y + 5)
					{
						Position.Y = platform.Y - Bounds.Height;
						Velocity.Y = 0;
						isOnGround = true;
						canJump = true;
					}
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
	public void PickupWeapon(Weapon.Weapon weapon)
	{
		if (_currentWeapon == null)
		{
			_currentWeapon = weapon;
			Console.WriteLine(weapon);
		}
		else if (_primaryWeapon == null)
		{
			_primaryWeapon = weapon;
			Console.WriteLine(weapon);
		}
		else if (_secondaryWeapon == null)
		{
			_secondaryWeapon = weapon;
			Console.WriteLine(weapon);
		}
		else
		{
			// ถ้ามีครบ 3 อันแล้ว ยังไม่กำหนดว่าทำไง อาจจะ drop หรือไม่เก็บ
			return;
		}

		// ตำแหน่งอาวุธจะติดตาม Player ทันที (หรือซ่อนไว้ถ้ายังไม่ใช้งาน)
		weapon.Position = this.Position;
	}

	public void TakeDamage(int amount, Vector2 knockbackDirection)
	{
		if (isInvincible || !IsAlive)
			return;

		currentHP -= amount;
		if (currentHP < 0) currentHP = 0;

		// เริ่มนับเวลาอมตะ
		invincibilityTimer = invincibilityTime;

		// กระเด้งออกจากศัตรู
		Velocity = knockbackDirection * 300f;
		Console.WriteLine($"Player HP: {currentHP}/{maxHP}");
	}

	public Rectangle GetAttackHitbox()
	{
		int width = 40;
		int height = 80;
		int offsetX = isFacingRight ? Bounds.Width : -width;
		return new Rectangle(
			(int)(Position.X + offsetX),
			(int)(Position.Y),
			width,
			height
		);
	}

	public List<Bullet> CollectBullets()
    {
        List<Bullet> output = new List<Bullet>(bullets);
        bullets.Clear();
        return output;
    }

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
	}
}
