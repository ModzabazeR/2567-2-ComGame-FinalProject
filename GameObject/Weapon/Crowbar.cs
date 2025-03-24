using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
// using FinalProject.GameObject.Entity.Enemy;

namespace FinalProject.GameObject.Weapon;

public class Crowbar : MeleeWeapon
{
	public Crowbar(Vector2 position) : base(position)
	{
	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		base.Update(gameTime, platforms);
	}

	protected override void PerformAttack()
	{

	}

	public override void Draw(SpriteBatch spriteBatch)
	{

	}
}