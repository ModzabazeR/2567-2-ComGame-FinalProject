using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalProject.GameObject.Entity;

public class Bullet : Entity
{
	private float damage;
	private float speed;
	private Vector2 direction;
	private bool isActive;
	private float lifetime;
	private float currentLifetime;

	private int bulletWidth;
	private int bulletHeight;

	public Bullet(Vector2 position, Vector2 direction, float speed, float damage, float lifetime , int widths, int height)
		: base(position)
	{
		this.direction = direction;
		this.speed = speed;
		this.damage = damage;
		this.lifetime = lifetime;
		this.currentLifetime = 0f;
		this.isActive = true;
		bulletWidth = widths;
    	bulletHeight = height;

		// initial bounds
    	Bounds = new Rectangle((int)position.X, (int)position.Y, bulletWidth, bulletHeight);

	}

	public override void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		base.Update(gameTime, platforms);

		if (!isActive) return;

		// Update position based on direction and speed
		Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

		// ✅ ตั้งขนาดกระสุนโดยไม่พึ่ง texture
		Bounds = new Rectangle((int)Position.X, (int)Position.Y, bulletWidth, bulletHeight);

		// Update lifetime
		currentLifetime += (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (currentLifetime >= lifetime)
		{
			isActive = false;
		}

		// Check for collisions with platforms
		foreach (Rectangle platform in platforms)
		{
			if (Bounds.Intersects(platform))
			{
				isActive = false;
				break;
			}
		}
	}
	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch); // วาด sprite/animation ปกติ

		// // วาด Hitbox ของ Boss
		// Texture2D hitboxTex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
		// hitboxTex.SetData(new[] { Color.Red });
		// spriteBatch.Draw(hitboxTex, Bounds, Color.Red * 0.4f);
	}


	public float GetDamage()
	{
		return damage;
	}

	public bool IsActive()
	{
		return isActive;
	}

	public void Deactivate()
	{
		isActive = false;
	}
}