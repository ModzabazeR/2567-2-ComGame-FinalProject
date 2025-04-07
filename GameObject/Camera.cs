using Microsoft.Xna.Framework;

namespace FinalProject.GameObject;

public class Camera
{
	public Matrix Transform { get; private set; }
	public Vector2 Position { get; private set; }
	private readonly float _viewWidth;
	private readonly float _viewHeight;
	private float _zoom; // Add zoom field
	private Vector2 _desiredPosition;
	private float _followSpeed = 15f; // Adjust this value to change smoothing speed
	private const float DEBUG_ZOOM = 1.75f;
	private const float REAL_ZOOM = 1.25f;

	public Camera(float viewWidth, float viewHeight)
	{
		_viewWidth = viewWidth;
		_viewHeight = viewHeight;
		_zoom = DEBUG_ZOOM;
	}

	public void Follow(Vector2 target, Rectangle roomBounds)
	{
		// Calculate desired camera position with target at center
		_desiredPosition = target - new Vector2(_viewWidth / (2 * _zoom), (_viewHeight / (2 * _zoom)));

		// Clamp desired position to room bounds
		_desiredPosition.X = MathHelper.Clamp(_desiredPosition.X, roomBounds.Left, roomBounds.Right - _viewWidth / _zoom);
		_desiredPosition.Y = MathHelper.Clamp(_desiredPosition.Y, roomBounds.Top, roomBounds.Bottom - _viewHeight / _zoom);

		// Smoothly move current position toward desired position
		Position = Vector2.Lerp(Position, _desiredPosition, _followSpeed * (1f / 60f));

		// Create transform matrix with zoom
		Transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
				   Matrix.CreateScale(_zoom) *
				   Matrix.CreateTranslation(new Vector3(Vector2.Zero, 0));
	}

	public void SetZoom(float zoom)
	{
		_zoom = zoom;
	}
}
