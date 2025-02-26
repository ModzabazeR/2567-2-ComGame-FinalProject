using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using FinalProject.GameObject;

namespace FinalProject;

// Base code from KMITL Comgame Class (Tetris Exercise)
class Singleton
{
	private static Singleton instance;

	// Input
	public KeyboardState PreviousKey;
	public KeyboardState CurrentKey;

	// Random
	public Random Random { get; } = new Random();

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

}