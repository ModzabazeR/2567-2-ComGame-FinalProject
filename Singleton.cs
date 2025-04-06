using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.GameObject;
using System.Collections.Generic;
using FinalProject.GameObject.Entity;
using FinalProject.Utils.MapManager;

namespace FinalProject;

// Base code from KMITL Comgame Class (Tetris Exercise)
public enum GameState
{
	Splash,
	Playing,
	Paused,
	Cutscene,
	GameOver,
	GameWon,
}

class Singleton
{
	private static Singleton instance;

	// Input
	public KeyboardState PreviousKey;
	public KeyboardState CurrentKey;

	// Random
	public Random Random { get; } = new Random();

	public int ScreenWidth { get; set; } = 1080;
	public int ScreenHeight { get; set; } = 720;

	// Tile size for the maps
	public int TILE_WIDTH { get; set; } = 32;
	public int TILE_HEIGHT { get; set; } = 32;

	// Map dimensions in tiles
	public int MAP_WIDTH { get; set; } = 39;
	public int MAP_HEIGHT { get; set; } = 22;

	public SpriteFont Font { get; set; }

	// Debug settings
	public bool ShowDebugInfo { get; set; } = true; // Set to true by default for testing

	// Add this property after other properties
	public GameState CurrentGameState { get; set; } = GameState.Splash;
	public Dictionary<string, Dictionary<string, Animation>> Animations { get; set; } = [];

	public List<Bullet> Bullets { get; } = new List<Bullet>();

	private Singleton() { }

	public static Singleton Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Singleton();
			}
			return instance;
		}
	}

	public Player Player { get; set; }
	public MapManager MapManager { get; set; }

	public void UpdateKeyboardState()
	{
		PreviousKey = CurrentKey;
		CurrentKey = Keyboard.GetState();
	}
}