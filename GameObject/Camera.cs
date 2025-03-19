using Microsoft.Xna.Framework;

namespace FinalProject.GameObject;

public class Camera
{
	public Matrix Transform { get; private set; }
	public Vector2 Position { get; private set; }
	private readonly float _viewWidth;
	private readonly float _viewHeight;
	private readonly float _zoom; // Add zoom field

	public Camera(float viewWidth, float viewHeight)
	{
		_viewWidth = viewWidth;
		_viewHeight = viewHeight;
		_zoom = 1.0f; // Set default zoom level (greater than 1 means more zoomed in)
	}

	public void Follow(Vector2 target, Rectangle roomBounds)
	{
		// Calculate camera position with target at center and adjusted for zoom
		var position = target - new Vector2(_viewWidth / (2 * _zoom), _viewHeight / (2 * _zoom));

		// Clamp camera to room bounds, accounting for zoom
		position.X = MathHelper.Clamp(position.X, roomBounds.Left, roomBounds.Right - _viewWidth / _zoom);
		position.Y = MathHelper.Clamp(position.Y, roomBounds.Top, roomBounds.Bottom - _viewHeight / _zoom);

		Position = position;

		// Create transform matrix with zoom
		Transform = Matrix.CreateTranslation(new Vector3(-position, 0)) *
				   Matrix.CreateScale(_zoom) * // Apply zoom
				   Matrix.CreateTranslation(new Vector3(Vector2.Zero, 0));
	}
}
