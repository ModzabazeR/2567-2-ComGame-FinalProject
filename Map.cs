using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace FinalProject
{
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


		public Map(string name, Texture2D texture, Vector2 position, string collisionMapPath)
		{
			this.name = name;
			Texture = texture;
			Position = position;
			SolidTiles = new List<Rectangle>();

			mapWidth = Singleton.Instance.MAP_WIDTH;
			mapHeight = Singleton.Instance.MAP_HEIGHT;

			// Calculate the total map size based on tiles
			int realMapWidth = mapWidth * Singleton.Instance.TILE_WIDTH;
			int realMapHeight = mapHeight * Singleton.Instance.TILE_HEIGHT;
			Bounds = new Rectangle((int)position.X, (int)position.Y, realMapWidth, realMapHeight);

			LoadCollisionData(collisionMapPath);
			InitializeCollisionTiles();
		}

		public Map(string name, Texture2D texture, Vector2 position, string collisionMapPath, int mapWidth, int mapHeight)
		{
			this.name = name;
			Texture = texture;
			Position = position;
			SolidTiles = new List<Rectangle>();

			this.mapWidth = mapWidth;
			this.mapHeight = mapHeight;

			// Calculate the total map size based on tiles
			int realMapWidth = this.mapWidth * Singleton.Instance.TILE_WIDTH;
			int realMapHeight = this.mapHeight * Singleton.Instance.TILE_HEIGHT;
			Bounds = new Rectangle((int)position.X, (int)position.Y, realMapWidth, realMapHeight);

			LoadCollisionData(collisionMapPath);
			InitializeCollisionTiles();
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
	}
}
