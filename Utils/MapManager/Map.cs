using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using FinalProject.GameObject.Entity.Enemy;

namespace FinalProject.Utils.MapManager;

public class Map
{
	public Texture2D Texture { get; private set; }
	public Vector2 Position { get; set; }
	public Rectangle Bounds { get; private set; }
	public List<Rectangle> SolidTiles { get; private set; }
	private bool[,] collisionData;
	private int mapHeight;
	private int mapWidth;
	private string name;
	private List<Enemy> enemies;


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

		this.mapWidth = mapWidth;
		this.mapHeight = mapHeight;

		// Calculate the total map size based on tiles
		int realMapWidth = this.mapWidth * Singleton.Instance.TILE_WIDTH;
		int realMapHeight = this.mapHeight * Singleton.Instance.TILE_HEIGHT;
		Bounds = new Rectangle((int)position.X, (int)position.Y, realMapWidth, realMapHeight);

		LoadLCM(collisionMapPath);
		InitializeCollisionTiles();

		if (name == "Map 1")
		{
			enemies = [new SimpleEnemy(Singleton.Instance.EntityAnimations["Player"], new Vector2(200, 700))];
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
		}
	}

	public void SpawnEnemies(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

		if (name == "Map 1")
		{
			// Map 1 spawn logic
			foreach (var enemy in enemies)
			{
				if (!enemy.IsSpawned)
				{
					enemy.Spawn();
				}
			}
		}
		else if (name == "Map 2")
		{
			// Map 2 spawn logic
			// Add your Map 2 specific spawn logic here
		}
		else if (name == "Map 3")
		{
			// Map 3 spawn logic
			// Add your Map 3 specific spawn logic here
		}
	}

	public List<Enemy> GetEnemies()
	{
		return enemies ?? [];
	}
}
