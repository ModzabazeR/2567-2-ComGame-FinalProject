using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.GameObject.Weapon;
using System;
using FinalProject.Utils.MapManager;
using FinalProject.Utils.SFXManager;

namespace FinalProject.GameObject.Entity;

public class Player : Movable
{
	private const float moveSpeed = 180f;
	private const float gravity = 1000f;
	private const float jumpForce = -600f;
	private bool canJump = false;
	public bool IsFacingRight => isFacingRight;

	private MapManager _mapManager;

	private Weapon.Weapon _primaryWeapon;
	private Weapon.Weapon _secondaryWeapon;
	private Weapon.Weapon _currentWeapon;

	public Weapon.Weapon PrimaryWeapon => _primaryWeapon;
	public Weapon.Weapon SecondaryWeapon => _secondaryWeapon;
	public int CurrentWeapon { get; private set; } // 0 = primary, 1 = secondary
	public int GrenadeCount => _grenadeCount;
	private int _grenadeCount = 0;

	private int maxHP = 10;
	private int currentHP = 10;

	public int CurrentHP => currentHP;
	public int MaxHP => maxHP;
	public bool IsAlive => currentHP > 0;

	private float invincibilityTime = 1.0f;
	private float invincibilityTimer = 0f;
	private bool isInvincible => invincibilityTimer > 0;

	private List<Bullet> bullets = new();

	private bool isAttacking = false;
	private float attackTimer = 0f;
	private bool isAttackLocked = false;
	public bool IsAttacking => isAttacking;
	private float _animationTransitionTime = 0.1f; // Transition time in seconds

	private float attackDuration = 0.3f;

	public bool isMovingOnGround => isOnGround && (Velocity.X != 0);
	private float footstepTimer = 0f;
	private const float FOOTSTEP_INTERVAL = 2f;

	public Player(Dictionary<string, Animation> animations, Vector2 position, MapManager mapManager)
	: base(animations, position)
	{
		_mapManager = mapManager;
	}

	private string GetMovementAnimation()
	{
		if (_currentWeapon is Crowbar)
			return "Crowbar_Walk";
		else if (_currentWeapon is Pistol)
			return "Pistol_Walk";
		else if (_currentWeapon is Shotgun)
			return "Shotgun_Walk";
		else if (_currentWeapon is Grenade)
			return "Grenade_Walk";
		else
			return "Walk"; // Default unarmed sprint
	}

	private string GetIdleAnimation()
	{
		if (_currentWeapon is Crowbar)
			return "Crowbar_Idle";
		else if (_currentWeapon is Pistol)
			return "Pistol_Idle";
		else if (_currentWeapon is Shotgun)
			return "Shotgun_Idle";
		else if (_currentWeapon is Grenade)
			return "Grenade_Idle";
		else
			return "Idle"; // Default unarmed idle
	}

	private string GetAttackAnimationName()
	{
		return _currentWeapon switch
		{
			Crowbar => "Crowbar_Attack",
			Pistol => "Pistol_Shoot",
			Shotgun => "Shotgun_Shoot",
			Grenade => "Grenade_Throw",
			_ => "Idle"
		};
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		Vector2 previousPosition = Position;
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		// Smooth animation transitions
		if (!isAttacking && _animationTransitionTime > 0)
		{
			_animationTransitionTime -= dt;
			return; // Skip movement updates during transition
		}

		// Update attack state - only lock movement for non-crowbar weapons
		if (isAttackLocked && _currentWeapon != null && !(_currentWeapon is Crowbar))
		{
			attackTimer -= dt;
			if (attackTimer <= 0)
			{
				isAttackLocked = false;
				// _animationManager.Play(_animations[GetIdleAnimation()]);
			}
			return; // Lock other updates during attack
		}

		// Remove the debug Console.WriteLine calls
		if (isMovingOnGround)
		{
			// Console.WriteLine("Player is moving on ground");
			footstepTimer += 0.25f;
			// Console.WriteLine($"Footstep timer: {footstepTimer}");
			if (footstepTimer >= FOOTSTEP_INTERVAL)
			{
				// Console.WriteLine("Playing footstep sound");
				SFXManager.Instance.PlayFootSteps(); // Trigger footstep sound
				footstepTimer = 0f;
			}
		}

		HandleInput();
		ApplyGravity(dt);
		HandleMovement(dt, platforms, previousPosition);

		if (invincibilityTimer > 0)
		{
			invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		if (isAttacking)
		{
			attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (attackTimer <= 0)
				isAttacking = false;
		}

		base.Update(gameTime, platforms);
	}

	private void HandleInput()
	{
		if (isAttackLocked) return; // Block input during attacks

		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.A))
		{
			Velocity.X = -moveSpeed;
			_animationManager.Play(_animations[GetMovementAnimation()]);
		}
		else if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.D))
		{
			Velocity.X = moveSpeed;
			_animationManager.Play(_animations[GetMovementAnimation()]);
		}
		else
		{
			Velocity.X = 0;
			_animationManager.Play(_animations[GetIdleAnimation()]);
		}

		UpdateFacingDirection(Velocity.X);

		if (canJump && Singleton.Instance.CurrentKey.IsKeyDown(Keys.W) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.W))
		{
			Velocity.Y = jumpForce;
			canJump = false;
			isOnGround = false;
			_animationManager.Play(_animations["Jump"]); // Changed from "Walk"
		}

		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.Q) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.Q))
		{
			if (_primaryWeapon != null && _secondaryWeapon != null)
			{
				CurrentWeapon = (CurrentWeapon == 0) ? 1 : 0;
				_currentWeapon = (CurrentWeapon == 0) ? _primaryWeapon : _secondaryWeapon;
			}
			else if (_primaryWeapon == null && _secondaryWeapon != null)
			{
				CurrentWeapon = 1;
				_currentWeapon = _secondaryWeapon;
			}
			else if (_secondaryWeapon == null && _primaryWeapon != null)
			{
				CurrentWeapon = 0;
				_currentWeapon = _primaryWeapon;
			}
			_animationManager.Play(_animations[GetIdleAnimation()]);
		}

		// Handle weapon input
		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.C) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.C) && !isAttackLocked)
		{
			if (_currentWeapon != null && _currentWeapon is Crowbar)
			{
				Crowbar crowbar = (Crowbar)_currentWeapon;
				string attackAnim = GetAttackAnimationName();
				Animation anim = _animations[attackAnim];
				attackDuration = anim.FrameCount * anim.FrameSpeed;
				isAttacking = true;
				// Don't lock movement for crowbar attacks
				// isAttackLocked = true;
				attackTimer = attackDuration;
				_animationManager.Play(anim);
				crowbar.PerformAttack();
				// Set invincibility for just a brief moment during attack
				invincibilityTimer = 0.4f; // 0.4 seconds of invincibility
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
				Shotgun shotgun = (Shotgun)_currentWeapon;

				if (shotgun.GetCurrentAmmo() > 0)
				{
					Console.WriteLine("shotgun shoot");
					Vector2 direction = isFacingRight ? Vector2.UnitX : -Vector2.UnitX;
					Vector2 spawnOffset = new Vector2(isFacingRight ? Bounds.Width : -12, 20);
					Vector2 spawnPos = Position + spawnOffset;

					var bullet = new Bullet(spawnPos, direction, speed: 500f, damage: 1f, lifetime: 0.2f, widths: 30, height: 30);
					bullets.Add(bullet);
					// isAttacking = true;
					// attackTimer = attackDuration;
					Console.WriteLine(shotgun.GetCurrentAmmo());
					shotgun.PerformAttack();
					Console.WriteLine(shotgun.GetCurrentAmmo());
				}
			}
			else
			{
				Console.WriteLine("You don't have a weapon to attack!");
			}
		}

		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.K) &&
		Singleton.Instance.PreviousKey.IsKeyUp(Keys.K))
		{
			if (_currentWeapon != null && _currentWeapon is Pistol)
			{
				Pistol pistol = (Pistol)_currentWeapon;
				if (pistol.GetCurrentAmmo() > 0)
				{
					Console.WriteLine("bullet shoot");
					Vector2 direction = isFacingRight ? Vector2.UnitX : -Vector2.UnitX;
					Vector2 spawnOffset = new Vector2(isFacingRight ? Bounds.Width : -12, 20);
					Vector2 spawnPos = Position + spawnOffset;

					var bullet = new Bullet(spawnPos, direction, speed: 700f, damage: 1f, lifetime: 2f, widths: 12, height: 12);
					bullets.Add(bullet);

					Console.WriteLine(pistol.GetCurrentAmmo());
					pistol.PerformAttack();
					Console.WriteLine(pistol.GetCurrentAmmo());
				}
			}
			else
			{
				Console.WriteLine("You don't have a weapon to attack!");
			}
		}

		if (Singleton.Instance.CurrentKey.IsKeyDown(Keys.X) &&
			Singleton.Instance.PreviousKey.IsKeyUp(Keys.X))
		{
			DropCurrentWeapon();
		}

	}

	public void PickupGrenade()
	{
		_grenadeCount = Math.Min(_grenadeCount + 1, 3); // Max 3 grenades
	}

	private void HandleGrenadeThrow()
	{
		if (_grenadeCount > 0 && Singleton.Instance.CurrentKey.IsKeyDown(Keys.G) &&
		Singleton.Instance.PreviousKey.IsKeyUp(Keys.G))
		{
			_grenadeCount--;
			_animationManager.Play(_animations["Grenade_Throw"]);
			var grenade = new FragGrenade(Position); // Use concrete class
													 // Add to game world (modify based on your architecture)
			var currentMap = _mapManager.CurrentMap;
			currentMap?.AddWeapon(grenade);
		}
	}

	public void DropCurrentWeapon()
	{
		if (_currentWeapon == null) return;

		var droppedWeapon = _currentWeapon;

		// Calculate drop position in front of player
		float dropOffset = 50f; // Adjust this value as needed
		Vector2 dropPosition = this.Position +
			new Vector2(isFacingRight ? dropOffset : -dropOffset, 0);

		droppedWeapon.Position = dropPosition;

		// Rest of existing drop logic
		if (CurrentWeapon == 0)
		{
			_primaryWeapon = null;
		}
		else
		{
			_secondaryWeapon = null;
		}

		_currentWeapon = null;
		_mapManager.CurrentMap?.AddWeapon(droppedWeapon);
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
				if (!isOnGround)
				{
					float playerFootY = Position.Y + Bounds.Height;
					float platformMiddleY = platform.Y + platform.Height / 2;
					if (playerFootY < platformMiddleY)
					{
						// Player is above the platform
						Position.Y = platform.Y - Bounds.Height;
						Velocity.Y = 0;
						isOnGround = true;
						canJump = true;
					}
				}

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

	public bool PickupWeapon(Weapon.Weapon weapon)
	{
		if (weapon is Grenade)
		{
			PickupGrenade();
			return true;
		}
		if (weapon is PistolBulletItem)
		{
			// Only allow pickup if player has a pistol in either slot
			bool hasPistol = (_primaryWeapon is Pistol) || (_secondaryWeapon is Pistol);
			if (!hasPistol)
			{
				Console.WriteLine("You don't have a pistol to use this ammo!");
				return false;
			}

			if (PrimaryWeapon is Pistol)
			{
				((Pistol)PrimaryWeapon).Reload();
				SFXManager.Instance.PlaySound("9mm Pistol Reload 2");
			}
			else if (SecondaryWeapon is Pistol)
			{
				((Pistol)SecondaryWeapon).Reload();
				SFXManager.Instance.PlaySound("9mm Pistol Reload 2");

			}
			return true;

		}
		if (weapon is ShotgunBulletItem)
		{
			// Only allow pickup if player has a shotgun in either slot
			bool hasShotgun = (_primaryWeapon is Shotgun) || (_secondaryWeapon is Shotgun);
			if (!hasShotgun)
			{
				Console.WriteLine("You don't have a shotgun to use this ammo!");
				return false;
			}

			if (PrimaryWeapon is Shotgun)
			{
				Shotgun shotgun = (Shotgun)PrimaryWeapon;
				shotgun.Reload();
				SFXManager.Instance.PlaySound("Shotgun_Pump");
			}
			else if (SecondaryWeapon is Shotgun)
			{
				Shotgun shotgun = (Shotgun)SecondaryWeapon;
				shotgun.Reload();
				SFXManager.Instance.PlaySound("Shotgun_Pump");
			}
			return true;
		}

		if (_secondaryWeapon == null)
		{
			_secondaryWeapon = weapon;
			CurrentWeapon = 1; // Force switch to secondary slot
			_currentWeapon = _secondaryWeapon;
			return true;
		}
		else if (_primaryWeapon == null)
		{
			_primaryWeapon = weapon;
			CurrentWeapon = 0; // Force switch to primary slot
			_currentWeapon = _primaryWeapon;
			return true;
		}

		Console.WriteLine("Inventory full! Can't pickup " + weapon.GetType().Name);
		return false;
	}

	public void TakeDamage(int amount, Vector2 knockbackDirection)
	{
		if (isInvincible || !IsAlive)
			return;

		currentHP -= amount;
		SFXManager.Instance.PlaySound("playeroof");
		if (currentHP < 0) currentHP = 0;

		invincibilityTimer = invincibilityTime;

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
