using Microsoft.Xna.Framework;
using System;

namespace FinalProject.GameObject;

public class Camera
{
	public Matrix Transform { get; private set; }
	public Vector2 Position { get; private set; }
	private readonly float _viewWidth;
	private readonly float _viewHeight;
	private readonly float _zoom; // Add zoom field
	private Vector2 _desiredPosition;
	private float _followSpeed = 15f; // Adjust this value to change smoothing speed
	private const float DEBUG_ZOOM = 1f;
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
		_desiredPosition = new Vector2(
			target.X - (_viewWidth / (2 * _zoom)),
			target.Y - (_viewHeight / (2 * _zoom))
		);
		
		// Log to verify camera is actually following player
		Console.WriteLine($"Camera following - Target: {target}, Desired: {_desiredPosition}");
		
		// Clamp position to room bounds to prevent seeing outside the level
		_desiredPosition.X = MathHelper.Clamp(_desiredPosition.X, 
											Math.Max(0, roomBounds.Left), 
											Math.Max(roomBounds.Right - _viewWidth / _zoom, roomBounds.Left));
		_desiredPosition.Y = MathHelper.Clamp(_desiredPosition.Y, 
											Math.Max(0, roomBounds.Top), 
											Math.Max(roomBounds.Bottom - _viewHeight / _zoom, roomBounds.Top));
		
		// Smoothly move current position toward desired position
		Position = Vector2.Lerp(Position, _desiredPosition, _followSpeed * (1f / 60f));
		
		// Create transform matrix with zoom
		Transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
				   Matrix.CreateScale(_zoom) *
				   Matrix.CreateTranslation(new Vector3(Vector2.Zero, 0));
	}

	public void SetPosition(Vector2 position)
	{
		Position = position;
		// Update the transform too
		Transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
				   Matrix.CreateScale(_zoom) *
				   Matrix.CreateTranslation(new Vector3(Vector2.Zero, 0));
	}
}
