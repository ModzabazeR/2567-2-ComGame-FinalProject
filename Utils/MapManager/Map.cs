using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using FinalProject.GameObject.Entity.Enemy;
using FinalProject.GameObject.Weapon;

namespace FinalProject.Utils.MapManager;

public class DoorTile
{
	public Vector2 Position { get; private set; }
	public string TargetMapName { get; private set; }
	public int TargetSpawnX { get; private set; }
	public int TargetSpawnY { get; private set; }
	public DoorTile(Vector2 position, string targetMapName, int targetSpawnX, int targetSpawnY)
	{
		Position = position;
		TargetMapName = targetMapName;
		TargetSpawnX = targetSpawnX;
		TargetSpawnY = targetSpawnY;
	}
}

public class Map
{
	public Texture2D Texture { get; private set; }
	public Vector2 Position { get; set; }
	public Rectangle Bounds { get; private set; }
	public List<Rectangle> SolidTiles { get; private set; }
	public List<DoorTile> DoorTiles { get; private set; }
	public Vector2 SpawnPoint { get; private set; }
	private bool[,] collisionData;
	private int mapHeight;
	private int mapWidth;
	private string name;
	public string Name => name;
	private List<Enemy> enemies;
	private bool hasPlayerEntered;
	private TimeSpan? firstEntryTime;
	private TimeSpan currentGameTime;
	private bool isMapCleared;
	private bool allEnemiesSpawned;

	public event Action OnMapCleared;

	private List<Weapon> mapWeapons;

	public Map(string name, Texture2D texture, Vector2 position, string collisionMapPath)
		: this(name, texture, position, collisionMapPath, Singleton.Instance.MAP_WIDTH, Singleton.Instance.MAP_HEIGHT)
	{
	}

	public Map(string name, Texture2D texture, Vector2 position, string collisionMapPath, int mapWidth, int mapHeight)
	{
		this.name = name;
		Texture = texture;
		Position = position;
		SolidTiles = [];
		DoorTiles = [];

		this.mapWidth = mapWidth;
		this.mapHeight = mapHeight;

		// Calculate the total map size based on tiles
		int realMapWidth = this.mapWidth * Singleton.Instance.TILE_WIDTH;
		int realMapHeight = this.mapHeight * Singleton.Instance.TILE_HEIGHT;
		Bounds = new Rectangle((int)position.X, (int)position.Y, realMapWidth, realMapHeight);

		LoadLCM(collisionMapPath);
		InitializeCollisionTiles();

		// Set default spawn point to the center of the map
		SpawnPoint = new Vector2(
			Position.X + (realMapWidth / 2),
			Position.Y + (realMapHeight / 2)
		);

		mapWeapons = new List<Weapon>();

		if (name == "Map 1")
		{
			enemies = [new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(6, 6))];
		}
		else if (name == "Map 3")
		{
			enemies =
			[
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(21, 6)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(18, 6)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(3, 6))
			];
		}
		else if (name == "Boss")
		{
			enemies = [
				new Boss(TileToWorldPosition(25, 5)) // ตำแหน่ง boss ปรากฏใน map boss
			];
		}

		enemies ??= new List<Enemy>();
		foreach (var enemy in enemies)
		{
			enemy.SetParentMap(this);
		}
	}

	private void LoadCollisionData(string path)
	{
		string[] lines = File.ReadAllLines(path);

		// Raise an error if line width and height doesn't match map width and height
		if (lines.Length != mapHeight || lines[0].Length != mapWidth)
		{
			throw new Exception($"Error on {path}:\nMap dimensions don't match. Expected {mapWidth}x{mapHeight}, got {lines[0].Length}x{lines.Length}");
		}

		collisionData = new bool[mapHeight, mapWidth];

		for (int y = 0; y < lines.Length && y < mapHeight; y++)
		{
			string line = lines[y].Trim();
			for (int x = 0; x < line.Length && x < mapWidth; x++)
			{
				collisionData[y, x] = line[x] == '1';
			}
		}
	}

	// TODO: 
	private void LoadLCM(string path)
	{
		using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
		{
			// Read dimensions
			int fileWidth = reader.ReadByte();
			int fileHeight = reader.ReadByte();

			// Verify dimensions match
			if (fileWidth != mapWidth || fileHeight != mapHeight)
			{
				throw new Exception($"Error on {path}: Map dimensions don't match. Expected {mapWidth}x{mapHeight}, got {fileWidth}x{fileHeight}");
			}

			collisionData = new bool[mapHeight, mapWidth];

			// Read the collision data
			for (int y = 0; y < mapHeight; y++)
			{
				int bitsToRead = mapWidth;
				int currentByte = 0;
				int bitPosition = 0;

				while (bitsToRead > 0)
				{
					if (bitPosition == 0)
					{
						currentByte = reader.ReadByte();
					}

					// Extract bit and store in collision data
					bool bitValue = ((currentByte >> bitPosition) & 1) == 1;
					collisionData[y, mapWidth - bitsToRead] = bitValue;

					bitPosition = (bitPosition + 1) % 8;
					bitsToRead--;
				}
			}
		}
	}

	private void InitializeCollisionTiles()
	{
		// Add collision rectangles for solid tiles
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				if (collisionData[y, x])
				{
					SolidTiles.Add(new Rectangle(
						(int)(Position.X + x * Singleton.Instance.TILE_WIDTH),
						(int)(Position.Y + y * Singleton.Instance.TILE_HEIGHT),
						Singleton.Instance.TILE_WIDTH,
						Singleton.Instance.TILE_HEIGHT
					));
				}
			}
		}
	}
	// vertical map 22x30

	public bool IsVisible(Rectangle cameraView)
	{
		return Bounds.Intersects(cameraView);
	}

	public void Draw(SpriteBatch spriteBatch, Rectangle cameraView)
	{
		if (!IsVisible(cameraView))
			return;

		// if (Singleton.Instance.ShowDebugInfo)
		// {
		// 	Console.WriteLine($"Drawing {name}");
		// }

		// Draw the entire map texture scaled to fit the tile-based dimensions
		spriteBatch.Draw(Texture, Bounds, Color.White);

		// Draw collision boxes in debug mode
		if (Singleton.Instance.ShowDebugInfo)
		{
			foreach (Rectangle solidTile in SolidTiles)
			{
				if (cameraView.Intersects(solidTile))
				{
					spriteBatch.Draw(
						Texture,
						solidTile,
						null,
						Color.Red * 0.3f
					);
				}
			}

			foreach (var door in DoorTiles)
			{
				if (cameraView.Intersects(new Rectangle(
					(int)door.Position.X,
					(int)door.Position.Y,
					Singleton.Instance.TILE_WIDTH,
					Singleton.Instance.TILE_HEIGHT
				)))
				{
					spriteBatch.Draw(
						Texture,
						new Rectangle(
							(int)door.Position.X,
							(int)door.Position.Y,
							Singleton.Instance.TILE_WIDTH,
							Singleton.Instance.TILE_HEIGHT
						),
						null,
						Color.Blue * 0.3f
					);
				}
			}
		}
	}

	public void Update(GameTime gameTime, Vector2 playerPosition)
	{
		currentGameTime = gameTime.TotalGameTime;
		CheckPlayerEntry(playerPosition);
		SpawnEnemies();
		CheckMapCleared();

		// debug for map 3, remove this if you want to manually clear the map
		if (name == "Map 3" && GetTimeSinceEntry()?.TotalSeconds >= 10)
		{
			foreach (var enemy in enemies)
			{
				enemy.Defeat();
			}
		}

		else if (name == "Boss")
		{
			if (hasPlayerEntered && GetTimeSinceEntry()?.TotalSeconds >= 3 && !allEnemiesSpawned)
			{
				foreach (var enemy in enemies)
				{
					if (!enemy.IsSpawned)
						enemy.Spawn();
				}
				allEnemiesSpawned = true;
			}
		}
	}

	public bool HasPlayerEntered()
	{
		return hasPlayerEntered;
	}

	public TimeSpan? GetTimeSinceEntry()
	{
		if (!hasPlayerEntered || !firstEntryTime.HasValue)
			return null;
		return currentGameTime - firstEntryTime.Value;
	}

	public void CheckPlayerEntry(Vector2 playerPosition)
	{
		if (!hasPlayerEntered && Bounds.Contains(playerPosition))
		{
			hasPlayerEntered = true;
			firstEntryTime = currentGameTime;
		}
	}

	private void CheckMapCleared()
	{
		if (name == "Map 3" && hasPlayerEntered && allEnemiesSpawned && !isMapCleared)
		{
			bool allEnemiesDefeated = true;
			foreach (var enemy in enemies)
			{
				if (enemy.IsSpawned && !enemy.IsDefeated)
				{
					allEnemiesDefeated = false;
					break;
				}
			}

			if (allEnemiesDefeated)
			{
				isMapCleared = true;
				OnMapCleared?.Invoke();
			}
		}
	}

	public void SpawnEnemies()
	{
		if (name == "Map 1")
		{
			// Map 1 spawn logic
			foreach (var enemy in enemies)
			{
				if (hasPlayerEntered && GetTimeSinceEntry()?.TotalSeconds >= 2)
				{
					if (!enemy.IsSpawned)
					{
						enemy.Spawn();
					}
				}
			}
		}
		else if (name == "Map 3")
		{
			// Map 3 spawn logic - spawn enemies after 5 seconds of entering
			if (hasPlayerEntered && GetTimeSinceEntry()?.TotalSeconds >= 5 && !allEnemiesSpawned)
			{
				// Spawn all enemies
				foreach (var enemy in enemies)
				{
					if (!enemy.IsSpawned)
					{
						enemy.Spawn();
					}
				}
				allEnemiesSpawned = true;
			}
		}
		else if (name == "Map 4")
		{
			// Map 4 spawn logic - spawn enemies after 3 seconds of entering
			if (hasPlayerEntered && GetTimeSinceEntry()?.TotalSeconds >= 3)
			{
				// Add your Map 4 specific spawn logic here
			}
		}
	}

	public List<Enemy> GetEnemies()
	{
		return enemies ?? [];
	}

	public void AddWeapon(Weapon weapon)
	{
		mapWeapons.Add(weapon);
	}

	public List<Weapon> GetWeapons()
	{
		return mapWeapons ?? [];
	}

	public Vector2 TileToWorldPosition(int tileX, int tileY)
	{
		if (tileX < 0 || tileX >= mapWidth || tileY < 0 || tileY >= mapHeight)
		{
			throw new ArgumentException($"{name}: Tile coordinates ({tileX}, {tileY}) are outside map bounds ({mapWidth}x{mapHeight})");
		}

		return new Vector2(
			Position.X + tileX * Singleton.Instance.TILE_WIDTH,
			Position.Y + tileY * Singleton.Instance.TILE_HEIGHT
		);
	}

	public void SetSpawnPoint(int tileX, int tileY)
	{
		SpawnPoint = TileToWorldPosition(tileX, tileY);
	}

	public void AddDoor(int tileX, int tileY, string targetMapName, int targetSpawnX, int targetSpawnY)
	{
		Vector2 doorPosition = TileToWorldPosition(tileX, tileY);
		// Note: The target spawn point will be calculated by the MapManager when the door is used
		DoorTiles.Add(new DoorTile(doorPosition, targetMapName, targetSpawnX, targetSpawnY));
	}

	public DoorTile GetDoorAtPosition(Vector2 position)
	{
		foreach (var door in DoorTiles)
		{
			Rectangle doorRect = new Rectangle(
				(int)door.Position.X,
				(int)door.Position.Y,
				Singleton.Instance.TILE_WIDTH,
				Singleton.Instance.TILE_HEIGHT
			);

			Rectangle playerRect = new Rectangle(
				(int)position.X,
				(int)position.Y,
				Singleton.Instance.TILE_WIDTH,
				Singleton.Instance.TILE_HEIGHT
			);
			if (doorRect.Intersects(playerRect))
			{
				return door;
			}
		}
		return null;
	}

	public List<Rectangle> GetDoorCollisionTiles()
	{
		List<Rectangle> doorCollisionTiles = new List<Rectangle>();
		foreach (var door in DoorTiles)
		{
			doorCollisionTiles.Add(new Rectangle(
				(int)door.Position.X,
				(int)door.Position.Y,
				Singleton.Instance.TILE_WIDTH,
				Singleton.Instance.TILE_HEIGHT
			));
		}
		return doorCollisionTiles;
	}
}
