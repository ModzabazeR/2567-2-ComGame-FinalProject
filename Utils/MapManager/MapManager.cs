using FinalProject.GameObject.Weapon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using FinalProject.GameObject.Entity;

namespace FinalProject.Utils.MapManager;
public class MapManager
{
	private Dictionary<string, Map> maps;
	private Player player;
	private float doorTransitionCooldown = 1.0f; // 1 second cooldown
	private float currentCooldown = 0f;
	private Map currentMap;

	public bool IsDoorCooldownActive => currentCooldown > 0;
	public string CurrentMap => currentMap.Name;

	public MapManager()
	{
		maps = new Dictionary<string, Map>();
	}

	public void SetPlayer(Player player)
	{
		this.player = player;
		UpdateCurrentMap();
	}

	public void UpdateCurrentMap()
	{
		if (player == null) return;

		foreach (var map in maps.Values)
		{
			if (map.Bounds.Contains(player.Position))
			{
				if (currentMap != map)
				{
					currentMap = map;
					// You can add any additional logic here when the player changes maps
					// For example, triggering events or updating UI
				}
				break;
			}
		}
	}

	public void AddMap(string name, Texture2D texture, Vector2 position, string collisionMapPath)
	{
		maps[name] = new Map(name, texture, position, collisionMapPath);
	}

	public void AddMap(string name, Texture2D texture, Vector2 position, string collisionMapPath, int mapWidth, int mapHeight)
	{
		maps[name] = new Map(name, texture, position, collisionMapPath, mapWidth, mapHeight);
	}

	public void SetMapSpawnPoint(string mapName, int tileX, int tileY)
	{
		if (maps.TryGetValue(mapName, out var map))
		{
			map.SetSpawnPoint(tileX, tileY);
		}
	}

	public void AddMapDoor(string sourceMapName, int sourceTileX, int sourceTileY,
						 string targetMapName, int targetSpawnX, int targetSpawnY)
	{
		if (maps.TryGetValue(sourceMapName, out var sourceMap))
		{
			sourceMap.AddDoor(sourceTileX, sourceTileY, targetMapName, targetSpawnX, targetSpawnY);
		}
	}

	public void ReplaceMapDoor(string mapName, int doorIndex, string newTargetMapName)
	{
		if (maps.TryGetValue(mapName, out var map))
		{
			map.ReplaceDoor(doorIndex, newTargetMapName);
		}
	}

	public void CheckMapTransitions(GameTime gameTime)
	{
		if (player == null) return;

		// Update cooldown
		if (currentCooldown > 0)
		{
			currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			return;
		}

		foreach (var map in maps.Values)
		{
			var door = map.GetDoorAtPosition(player.Position);
			if (door != null && maps.TryGetValue(door.TargetMapName, out var targetMap))
			{
				// Convert tile coordinates to world coordinates for the target map
				Vector2 targetPosition = targetMap.TileToWorldPosition(door.TargetSpawnX, door.TargetSpawnY);
				player.Position = targetPosition;
				currentCooldown = doorTransitionCooldown;
				UpdateCurrentMap(); // Update current map after teleporting
				break;
			}
		}
	}

	public List<Rectangle> GetAllSolidTiles()
	{
		List<Rectangle> allSolidTiles = new List<Rectangle>();
		foreach (var map in maps.Values)
		{
			allSolidTiles.AddRange(map.SolidTiles);
			if (IsDoorCooldownActive)
			{
				allSolidTiles.AddRange(map.GetDoorCollisionTiles());
			}
		}
		return allSolidTiles;
	}

	public Rectangle GetWorldBounds()
	{
		if (maps.Count == 0) return Rectangle.Empty;

		return currentMap.Bounds;
	}

	public void Draw(SpriteBatch spriteBatch, Rectangle cameraView)
	{
		foreach (var map in maps.Values)
		{
			map.Draw(spriteBatch, cameraView);
		}
	}

	public void DrawOverlays(SpriteBatch spriteBatch, Rectangle cameraView)
	{
		foreach (var map in maps.Values)
		{
			map.DrawOverlay(spriteBatch, cameraView);
		}
	}

	public Dictionary<string, Map> GetMaps()
	{
		return maps;
	}

	public Map GetMap(string name)
	{
		return maps.TryGetValue(name, out var map) ? map : null;
	}

	public void AddOverlay(string mapName, Texture2D overlayTexture)
	{
		if (maps.TryGetValue(mapName, out var map))
		{
			map.AddOverlay(overlayTexture);
		}
	}

	public void AddMapDeadZone(string mapName, int startTileX, int startTileY, int width, int height)
	{
		if (maps.TryGetValue(mapName, out var map))
		{
			map.AddDeadZone(startTileX, startTileY, width, height);
		}
	}
}

