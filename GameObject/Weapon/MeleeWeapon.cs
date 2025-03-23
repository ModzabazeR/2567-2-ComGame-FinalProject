using Microsoft.Xna.Framework;

namespace FinalProject.GameObject.Weapon;

public abstract class MeleeWeapon : Weapon
{
	protected float attackRange;
	protected float attackAngle;

	public MeleeWeapon(Vector2 position) : base(position)
	{
		attackRange = 50f; // Default range
		attackAngle = 45f; // Default angle in degrees
	}

	protected override void PerformAttack()
	{
		// Melee weapons will implement their specific attack logic here
		// This could include creating hitboxes, playing animations, etc.
	}

	public float GetAttackRange()
	{
		return attackRange;
	}

	public float GetAttackAngle()
	{
		return attackAngle;
	}
}