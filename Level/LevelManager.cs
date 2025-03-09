using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.Level;

public class LevelManager
{
	private List<Room> _rooms;
	private Room _currentRoom;

	public LevelManager(GraphicsDevice graphicsDevice)
	{
		InitializeRooms(graphicsDevice);
	}

	private void InitializeRooms(GraphicsDevice graphicsDevice)
	{
		// Create platform texture
		Texture2D platformTexture = new Texture2D(graphicsDevice, 200, 32);
		Color[] platformData = new Color[200 * 32];
		for (int i = 0; i < platformData.Length; i++)
			platformData[i] = Color.Green;
		platformTexture.SetData(platformData);

		// Create special platform textures
		Texture2D redPlatformTexture = new Texture2D(graphicsDevice, 200, 32);
		Color[] redPlatformData = new Color[200 * 32];
		for (int i = 0; i < redPlatformData.Length; i++)
			redPlatformData[i] = Color.Red;
		redPlatformTexture.SetData(redPlatformData);

		Texture2D bluePlatformTexture = new Texture2D(graphicsDevice, 200, 32);
		Color[] bluePlatformData = new Color[200 * 32];
		for (int i = 0; i < bluePlatformData.Length; i++)
			bluePlatformData[i] = Color.Blue;
		bluePlatformTexture.SetData(bluePlatformData);

		Texture2D purplePlatformTexture = new Texture2D(graphicsDevice, 200, 32);
		Color[] purplePlatformData = new Color[200 * 32];
		for (int i = 0; i < purplePlatformData.Length; i++)
			purplePlatformData[i] = Color.Purple;
		purplePlatformTexture.SetData(purplePlatformData);

		Texture2D yellowPlatformTexture = new Texture2D(graphicsDevice, 200, 32);
		Color[] yellowPlatformData = new Color[200 * 32];
		for (int i = 0; i < yellowPlatformData.Length; i++)
			yellowPlatformData[i] = Color.Yellow;
		yellowPlatformTexture.SetData(yellowPlatformData);

		// Create special object textures
		Texture2D pentagramTexture = new Texture2D(graphicsDevice, 100, 100);
		Color[] pentagramData = new Color[100 * 100];
		for (int i = 0; i < pentagramData.Length; i++)
			pentagramData[i] = Color.White;
		pentagramTexture.SetData(pentagramData);

		_rooms = new List<Room>();

		// Room 1 (Starting Room)
		var startRoom = new Room(
			"Start Room",
			new Rectangle(0, 0, 800, 600),
			new List<Rectangle> {
				new Rectangle(0, 500, 800, 32),    // Ground
                new Rectangle(100, 400, 200, 32),  // Platform
            },
			platformTexture
		);

		// Room 2 (Game Room - Top Left)
		var gameRoom1 = new Room(
			"Room 1",
			new Rectangle(800, 0, 800, 600),
			new List<Rectangle> {
				new Rectangle(800, 500, 600, 32),     // Ground
                new Rectangle(1000, 350, 200, 32),    // Platform 1
                new Rectangle(1300, 200, 200, 32)     // Platform 2
            },
			platformTexture
		);

		// Room 3 (Game Room - Bottom Left)
		var gameRoom2 = new Room(
			"Room 2",
			new Rectangle(800, 600, 800, 600),
			new List<Rectangle> {
				new Rectangle(800, 1100, 800, 32),    // Ground
                new Rectangle(900, 900, 200, 32),    // Platform 1
                new Rectangle(1200, 1000, 200, 32)   // Platform 2
            },
			platformTexture
		);

		// Room 4 (Game Room - Bottom Right)
		var gameRoom3 = new Room(
			"Room 3",
			new Rectangle(1600, 600, 800, 600),
			new List<Rectangle> {
				new Rectangle(1600, 1100, 800, 32),    // Ground
                new Rectangle(1700, 900, 200, 32),    // Platform 1
                new Rectangle(2000, 1000, 200, 32)    // Platform 2
            },
			platformTexture
		);

		// Room 5 (Game Room - Top Right with pentagram)
		var gameRoom4 = new Room(
			"Room 4",
			new Rectangle(1600, 0, 800, 600),
			new List<Rectangle> {
				new Rectangle(1600, 500, 800, 32),    // Ground
                new Rectangle(1800, 350, 200, 32),    // Platform 1
                new Rectangle(2000, 200, 200, 32)     // Platform 2
            },
			platformTexture
		);

		// Room 6 (Boss Room)
		var bossRoom = new Room(
			"Boss Room",
			new Rectangle(2400, 0, 800, 600),
			new List<Rectangle> {
				new Rectangle(2400, 500, 800, 32),    // Ground
                new Rectangle(2600, 350, 200, 32),    // Platform 1
            },
			redPlatformTexture // Boss room uses red platforms
		);

		// Room 7 (End Room)
		var endRoom = new Room(
			"End Room",
			new Rectangle(3200, 0, 800, 600),
			new List<Rectangle> {
				new Rectangle(3200, 500, 800, 32),    // Ground
                new Rectangle(3500, 350, 200, 32),    // Platform
            },
			yellowPlatformTexture // End room uses yellow platforms
		);

		// Connect rooms based on the layout in the sketch
		startRoom.ConnectedRight = gameRoom1;

		gameRoom1.ConnectedLeft = startRoom;
		gameRoom1.ConnectedRight = gameRoom4;

		gameRoom2.ConnectedRight = gameRoom3;

		gameRoom3.ConnectedLeft = gameRoom2;

		gameRoom4.ConnectedLeft = gameRoom1;
		gameRoom4.ConnectedRight = bossRoom;

		bossRoom.ConnectedLeft = gameRoom4;
		bossRoom.ConnectedRight = endRoom;

		endRoom.ConnectedLeft = bossRoom;

		// Add all rooms to the list
		_rooms.Add(startRoom);
		_rooms.Add(gameRoom1);
		_rooms.Add(gameRoom2);
		_rooms.Add(gameRoom3);
		_rooms.Add(gameRoom4);
		_rooms.Add(bossRoom);
		_rooms.Add(endRoom);

		_currentRoom = startRoom;
	}

	public List<Rectangle> GetVisiblePlatforms(Rectangle cameraBounds)
	{
		List<Rectangle> visiblePlatforms = new List<Rectangle>();

		foreach (var room in _rooms)
		{
			if (room.IsInView(cameraBounds))
			{
				visiblePlatforms.AddRange(room.Platforms);
			}
		}

		return visiblePlatforms;
	}

	public void Draw(SpriteBatch spriteBatch, Rectangle cameraBounds)
	{
		foreach (var room in _rooms)
		{
			if (room.IsInView(cameraBounds))
			{
				room.Draw(spriteBatch);
			}
		}
	}
}
