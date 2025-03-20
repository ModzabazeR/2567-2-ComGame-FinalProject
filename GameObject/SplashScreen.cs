using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.GameObject;

public class SplashScreenData
{
	public string[] Lines { get; }
	public float FadeSpeed { get; }
	public float DisplayTime { get; }

	public SplashScreenData(string[] lines, float fadeSpeed = 1f, float displayTime = 2f)
	{
		Lines = lines;
		FadeSpeed = fadeSpeed;
		DisplayTime = displayTime;
	}
}

public class SplashScreen
{
	private string[] _lines;
	private float _alpha;
	private float _fadeSpeed;
	private bool _isFadingIn;
	private bool _isFadingOut;
	private float _displayTime;
	private float _timer;
	private bool _isComplete;

	public bool IsComplete => _isComplete;

	public SplashScreen(string[] lines, float fadeSpeed = 1f, float displayTime = 2f)
	{
		_lines = lines;
		_fadeSpeed = fadeSpeed;
		_displayTime = displayTime;
		_alpha = 0f;
		_isFadingIn = true;
		_isFadingOut = false;
		_timer = 0f;
		_isComplete = false;
	}

	public SplashScreen(SplashScreenData data)
		: this(data.Lines, data.FadeSpeed, data.DisplayTime)
	{
	}

	public void Update(GameTime gameTime)
	{
		float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

		if (_isFadingIn)
		{
			_alpha += _fadeSpeed * deltaTime;
			if (_alpha >= 1f)
			{
				_alpha = 1f;
				_isFadingIn = false;
				_timer = 0f;
			}
		}
		else if (!_isFadingOut)
		{
			_timer += deltaTime;
			if (_timer >= _displayTime)
			{
				_isFadingOut = true;
			}
		}
		else // Fading out
		{
			_alpha -= _fadeSpeed * deltaTime;
			if (_alpha <= 0f)
			{
				_alpha = 0f;
				_isComplete = true;
			}
		}
	}

	public void Draw(SpriteBatch spriteBatch, SpriteFont font)
	{
		Vector2 screenCenter = new Vector2(
			Singleton.Instance.ScreenWidth / 2,
			Singleton.Instance.ScreenHeight / 2
		);

		// Calculate total height of all lines
		float totalHeight = (_lines.Length - 1) * font.LineSpacing;
		Vector2 currentPosition = screenCenter - new Vector2(0, totalHeight / 2);

		foreach (string line in _lines)
		{
			Vector2 textSize = font.MeasureString(line);
			Vector2 position = currentPosition - new Vector2(textSize.X / 2, 0);
			spriteBatch.DrawString(font, line, position, Color.White * _alpha);
			currentPosition.Y += font.LineSpacing;
		}
	}
}