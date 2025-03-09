using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.Level;

public class Room
{
	public Rectangle Bounds { get; private set; }
	public List<Rectangle> Platforms { get; private set; }
	public Rectangle TriggerZone { get; private set; }
	public Room ConnectedLeft { get; set; }
	public Room ConnectedRight { get; set; }
	private Texture2D _platformTexture;
	private string _roomName;
	public bool HasPentagram { get; set; }
	public bool IsBossRoom { get; set; }
	public bool IsEndRoom { get; set; }

	public Room(string roomName, Rectangle bounds, List<Rectangle> platforms, Texture2D platformTexture)
	{
		Bounds = bounds;
		Platforms = platforms;
		_platformTexture = platformTexture;
		_roomName = roomName;

		// Create trigger zone at the edges of the room
		TriggerZone = new Rectangle(
			bounds.X + bounds.Width / 4,
			bounds.Y,
			bounds.Width / 2,
			bounds.Height
		);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		// Draw room outline for debugging
		spriteBatch.Draw(_platformTexture, Bounds, Color.White * 0.3f);

		// Draw border
		int borderThickness = 2;
		Rectangle topBorder = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderThickness);
		Rectangle bottomBorder = new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - borderThickness, Bounds.Width, borderThickness);
		Rectangle leftBorder = new Rectangle(Bounds.X, Bounds.Y, borderThickness, Bounds.Height);
		Rectangle rightBorder = new Rectangle(Bounds.X + Bounds.Width - borderThickness, Bounds.Y, borderThickness, Bounds.Height);

		spriteBatch.Draw(_platformTexture, topBorder, Color.White);
		spriteBatch.Draw(_platformTexture, bottomBorder, Color.White);
		spriteBatch.Draw(_platformTexture, leftBorder, Color.White);
		spriteBatch.Draw(_platformTexture, rightBorder, Color.White);

		// Draw room name
		Vector2 roomNamePosition = new Vector2(
			Bounds.X + 20, // Small padding from left edge
			Bounds.Y + 20  // Small padding from top edge
		);
		spriteBatch.DrawString(Singleton.Instance.Font, _roomName, roomNamePosition, Color.White);


		// Draw platforms
		foreach (var platform in Platforms)
		{
			spriteBatch.Draw(_platformTexture, platform, Color.White);
		}
	}

	public bool IsInView(Rectangle cameraBounds)
	{
		return Bounds.Intersects(cameraBounds);
	}
}
