using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalProject
{
	public class MapManager
	{
		private List<Map> maps;

		public MapManager()
		{
			maps = new List<Map>();
		}

		public void AddMap(Texture2D texture, Vector2 position, string collisionMapPath)
		{
			maps.Add(new Map(texture, position, collisionMapPath));
		}

		public List<Rectangle> GetAllSolidTiles()
		{
			List<Rectangle> allSolidTiles = new List<Rectangle>();
			foreach (var map in maps)
			{
				allSolidTiles.AddRange(map.SolidTiles);
			}
			return allSolidTiles;
		}

		public Rectangle GetWorldBounds()
		{
			if (maps.Count == 0) return Rectangle.Empty;

			// Start with the first map's bounds
			Rectangle bounds = maps[0].Bounds;

			// Expand bounds to include all maps
			foreach (var map in maps)
			{
				bounds = Rectangle.Union(bounds, map.Bounds);
			}

			return bounds;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var map in maps)
			{
				map.Draw(spriteBatch);
			}
		}
	}
}
