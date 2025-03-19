using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		public Map(Texture2D texture, Vector2 position, string collisionMapPath)
		{
			Texture = texture;
			Position = position;
			SolidTiles = new List<Rectangle>();

			// Calculate the total map size based on tiles
			int mapWidth = Singleton.Instance.MAP_WIDTH * Singleton.Instance.TILE_WIDTH;
			int mapHeight = Singleton.Instance.MAP_HEIGHT * Singleton.Instance.TILE_HEIGHT;
			Bounds = new Rectangle((int)position.X, (int)position.Y, mapWidth, mapHeight);

			LoadCollisionData(collisionMapPath);
			InitializeCollisionTiles();
		}

		private void LoadCollisionData(string path)
		{
			string[] lines = File.ReadAllLines(path);
			collisionData = new bool[Singleton.Instance.MAP_HEIGHT, Singleton.Instance.MAP_WIDTH];

			for (int y = 0; y < lines.Length && y < Singleton.Instance.MAP_HEIGHT; y++)
			{
				string line = lines[y].Trim();
				for (int x = 0; x < line.Length && x < Singleton.Instance.MAP_WIDTH; x++)
				{
					collisionData[y, x] = line[x] == '1';
				}
			}
		}

		private void InitializeCollisionTiles()
		{
			// Add collision rectangles for solid tiles
			for (int y = 0; y < Singleton.Instance.MAP_HEIGHT; y++)
			{
				for (int x = 0; x < Singleton.Instance.MAP_WIDTH; x++)
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

		public void Draw(SpriteBatch spriteBatch)
		{
			// Draw the entire map texture scaled to fit the tile-based dimensions
			spriteBatch.Draw(Texture, Bounds, Color.White);

			// Draw collision boxes in debug mode
			if (Singleton.Instance.ShowDebugInfo)
			{
				foreach (Rectangle solidTile in SolidTiles)
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
