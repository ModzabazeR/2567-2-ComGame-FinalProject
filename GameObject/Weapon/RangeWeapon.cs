using FinalProject.Utils.SFXManager;
using Microsoft.Xna.Framework;

namespace FinalProject.GameObject.Weapon;

public abstract class RangeWeapon : Weapon
{
	protected float bulletSpeed;
	protected int maxAmmo;
	protected int currentAmmo;
	protected float spread;

	public RangeWeapon(Vector2 position) : base(position)
	{
		bulletSpeed = 500f; // Default bullet speed
		maxAmmo = 30; // Default max ammo
		currentAmmo = maxAmmo;
		spread = 0f; // Default spread in degrees
	}

	public override void PerformAttack()
	{
		if (currentAmmo > 0)
		{
			// Range weapons will implement their specific attack logic here
			// This could include creating bullets, playing animations, etc.
			currentAmmo--;
		}
	}

	public virtual void Reload()
	{
		currentAmmo = maxAmmo;
	}

	public int GetCurrentAmmo()
	{
		return currentAmmo;
	}

	public int GetMaxAmmo()
	{
		return maxAmmo;
	}

	public float GetBulletSpeed()
	{
		return bulletSpeed;
	}

	public float GetSpread()
	{
		return spread;
	}
}