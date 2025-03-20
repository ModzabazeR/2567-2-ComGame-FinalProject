using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.Utils.SplashScreen;

public class SplashScreenSequence
{
	private Queue<SplashScreenData> _screens;
	private SplashScreen _currentScreen;
	private bool _isComplete;

	public bool IsComplete => _isComplete;

	public SplashScreenSequence(IEnumerable<SplashScreenData> screens)
	{
		_screens = new Queue<SplashScreenData>(screens);
		_isComplete = false;
		ShowNextScreen();
	}

	public void Update(GameTime gameTime)
	{
		if (_currentScreen != null)
		{
			_currentScreen.Update(gameTime);
			if (_currentScreen.IsComplete)
			{
				ShowNextScreen();
			}
		}
	}

	public void Draw(SpriteBatch spriteBatch, SpriteFont font)
	{
		_currentScreen?.Draw(spriteBatch, font);
	}

	private void ShowNextScreen()
	{
		if (_screens.Count > 0)
		{
			_currentScreen = new SplashScreen(_screens.Dequeue());
		}
		else
		{
			_currentScreen = null;
			_isComplete = true;
		}
	}

	public void Skip()
	{
		_screens.Clear();
		_currentScreen = null;
		_isComplete = true;
	}
}