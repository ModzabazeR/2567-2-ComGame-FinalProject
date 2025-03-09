using Microsoft.Xna.Framework;

namespace FinalProject.GameObject;

public class Camera
{
	public Matrix Transform { get; private set; }
	public Vector2 Position { get; private set; }
	private readonly float _viewWidth;
	private readonly float _viewHeight;

	public Camera(float viewWidth, float viewHeight)
	{
		_viewWidth = viewWidth;
		_viewHeight = viewHeight;
	}

	public void Follow(Vector2 target, Rectangle roomBounds)
	{
		// Calculate camera position with target at center
		var position = target - new Vector2(_viewWidth / 2f, _viewHeight / 2f);

		// Clamp camera to room bounds
		position.X = MathHelper.Clamp(position.X, roomBounds.Left, roomBounds.Right - _viewWidth);
		position.Y = MathHelper.Clamp(position.Y, roomBounds.Top, roomBounds.Bottom - _viewHeight);

		Position = position;
		Transform = Matrix.CreateTranslation(new Vector3(-position, 0));
	}
}
