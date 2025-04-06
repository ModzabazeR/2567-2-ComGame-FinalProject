using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.GameObject.Entity;

public abstract class Entity
{
	public Vector2 Position;
	public Vector2 Velocity;
	public Rectangle Bounds;
	public Texture2D EntityTexture { get; set; }

	public Entity(Vector2 position)
	{
		Position = position;
		Velocity = Vector2.Zero;
	}

	public virtual void Update(GameTime gameTime, List<Rectangle> platforms)
	{
		UpdateBounds();
	}

	public virtual void Draw(SpriteBatch spriteBatch)
	{
		DrawBoundingBox(spriteBatch);
	}

	protected virtual void UpdateBounds()
	{
		if (EntityTexture != null)
			Bounds = new Rectangle((int)Position.X, (int)Position.Y, EntityTexture.Width, EntityTexture.Height);
	}

	protected virtual void DrawBoundingBox(SpriteBatch spriteBatch)
	{
		if (!Singleton.Instance.ShowDebugInfo)
			return;

		if (EntityTexture == null)
		{
			EntityTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
			EntityTexture.SetData(new[] { Color.White });
		}

		Color boundingBoxColor = Color.Red * 0.7f;

		spriteBatch.Draw(EntityTexture, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1), boundingBoxColor);
		spriteBatch.Draw(EntityTexture, new Rectangle(Bounds.X, Bounds.Y + Bounds.Height, Bounds.Width, 1), boundingBoxColor);
		spriteBatch.Draw(EntityTexture, new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height), boundingBoxColor);
		spriteBatch.Draw(EntityTexture, new Rectangle(Bounds.X + Bounds.Width, Bounds.Y, 1, Bounds.Height), boundingBoxColor);
	}
}
