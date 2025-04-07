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

	private List<Weapon> weapons;
	private Texture2D mapOverlay;
	private List<Rectangle> deadZones;
	public List<Rectangle> DeadZones => deadZones;

	public Map(string name, Texture2D texture, Vector2 position, string collisionMapPath)
		: this(name, texture, position, collisionMapPath, Singleton.Instance.MAP_WIDTH, Singleton.Instance.MAP_HEIGHT)
	{
	}

	public Map(string name, Texture2D texture, Vector2 position, string collisionMapPath, int mapWidth, int mapHeight)
	{
		this.name = name;
		deadZones = new List<Rectangle>();
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

		if (name == "Map 1")
		{
			enemies = [
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(17, 15)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(35, 15)),
			];
			weapons = [
				new Crowbar(TileToWorldPosition(7, 18)) { EntityTexture = Singleton.Instance.CrowbarTexture },
			];
		}
		else if (name == "Map 3")
		{
			enemies =
			[
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(21, 6)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(15, 6)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(10, 10)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(2, 12)),
			];
			weapons = [
				new Pistol(TileToWorldPosition(17, 18)) { EntityTexture = Singleton.Instance.PistolTexture },
			];
		}
		else if (name == "Map 4")
		{
			enemies = [
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(35, 15)),
			];
			weapons = [
				new FragGrenade(TileToWorldPosition(10, 18)) { EntityTexture = Singleton.Instance.GrenadeTexture },
			];
		}
		else if (name == "Map 5")
		{
			enemies = [
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(5, 15)),
			];
		}
		else if (name == "Map 6")
		{
			enemies = [
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(5, 15)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(8, 15)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(17, 15)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(29, 5)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(20, 5)),
			];
		}
		else if (name == "Map 7")
		{
			enemies = [
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(15, 15)),
				new SimpleEnemy(Singleton.Instance.Animations["Zombie"], TileToWorldPosition(28, 10)),
			];
			weapons = [
				new Shotgun(TileToWorldPosition(30, 18)) { EntityTexture = Singleton.Instance.ShotgunTexture },
			];
		}
		else if (name == "Boss")
		{
			enemies = [
				new Boss(TileToWorldPosition(20, 3)) // ตำแหน่ง boss ปรากฏใน map boss
			];
		}

		enemies ??= new List<Enemy>();
		weapons ??= new List<Weapon>();
		foreach (var enemy in enemies)
		{
			enemy.SetParentMap(this);
		}
	}

	public void AddOverlay(Texture2D overlayTexture)
	{
		mapOverlay = overlayTexture;
	}

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

		// plain color
		Texture2D debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
		debugTexture.SetData([Color.White]);
		// Draw collision boxes in debug mode
		if (Singleton.Instance.ShowDebugInfo)
		{
			foreach (Rectangle solidTile in SolidTiles)
			{
				if (cameraView.Intersects(solidTile))
				{
					spriteBatch.Draw(
						debugTexture,
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
						debugTexture,
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

			foreach (var deadZone in deadZones)
			{
				if (cameraView.Intersects(deadZone))
				{
					spriteBatch.Draw(
						debugTexture,
						deadZone,
						null,
						Color.Green * 0.3f
					);
				}
			}
		}
	}

	public void DrawOverlay(SpriteBatch spriteBatch, Rectangle cameraView)
	{
		if (mapOverlay != null && IsVisible(cameraView))
		{
			spriteBatch.Draw(mapOverlay, Bounds, Color.White);
		}
	}

	public void Update(GameTime gameTime, Vector2 playerPosition)
	{
		currentGameTime = gameTime.TotalGameTime;
		CheckPlayerEntry(playerPosition);
		SpawnEnemies();
		CheckMapCleared();
		CheckIntersectDeadZone(playerPosition);
	}

	public void CheckIntersectDeadZone(Vector2 playerPosition)
	{
		foreach (var deadZone in deadZones)
		{
			if (deadZone.Contains(playerPosition))
			{
				// Handle player entering the dead zone
				Console.WriteLine($"Player entered dead zone at {deadZone}");
				Singleton.Instance.CurrentGameState = GameState.GameOver;
			}
		}
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
		if ((name == "Map 3" || name == "Boss") && hasPlayerEntered && allEnemiesSpawned && !isMapCleared)
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

	// Force the map to be marked as cleared and trigger the OnMapCleared event
	public void ForceMapCleared()
	{
		if (!isMapCleared)
		{
			isMapCleared = true;
			OnMapCleared?.Invoke();
		}
	}

	public void SpawnEnemies()
	{
		if (hasPlayerEntered && !allEnemiesSpawned)
		{
			foreach (var enemy in enemies)
			{
				if (!enemy.IsSpawned)
				{
					enemy.Spawn();
				}
			}
			if (name != "Boss")
			{
				allEnemiesSpawned = true;
			}
		}
	}

	public List<Enemy> GetEnemies()
	{
		return enemies ?? [];
	}

	public List<Enemy> GetEnemiesCopy()
	{
		return new List<Enemy>(enemies ?? []);
	}

	public void AddWeapon(Weapon weapon)
	{
		weapons.Add(weapon);
	}

	public List<Weapon> GetWeapons()
	{
		return weapons ?? [];
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

	public void ReplaceDoor(int doorIndex, string newTargetMapName)
	{
		if (doorIndex >= 0 && doorIndex < DoorTiles.Count)
		{
			var door = DoorTiles[doorIndex];
			DoorTiles[doorIndex] = new DoorTile(door.Position, newTargetMapName, door.TargetSpawnX, door.TargetSpawnY);
		}
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

	public void AddDeadZone(int startTileX, int startTileY, int width, int height)
	{
		Vector2 startPosition = TileToWorldPosition(startTileX, startTileY);
		Rectangle deadZone = new Rectangle(
			(int)startPosition.X,
			(int)startPosition.Y,
			width * Singleton.Instance.TILE_WIDTH,
			height * Singleton.Instance.TILE_HEIGHT
		);
		deadZones.Add(deadZone);
	}
}
