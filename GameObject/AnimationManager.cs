using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject.GameObject;

public class AnimationManager
{
    private Animation _animation;
    private float _timer;
    private int _currentFrame;

    public Animation Animation => _animation;

    public AnimationManager(Animation animation)
    {
        _animation = animation;
        _currentFrame = 0;
        _timer = 0f;
    }

    public void Play(Animation animation)
    {
        if (_animation == animation)
            return;

        _animation = animation;
        _currentFrame = 0;
        _timer = 0f;
    }

    public void Stop()
    {
        _timer = 0;
        _currentFrame = 0;
    }

    public void Update(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= _animation.FrameSpeed)
        {
            _timer = 0f;
            _currentFrame++;

            if (_currentFrame >= _animation.FrameCount)
            {
                if (_animation.IsLooping)
                    _currentFrame = 0;
                else
                    _currentFrame = _animation.FrameCount - 1;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, bool flip = false)
    {
        if (_animation == null)
        {
            Console.WriteLine("Cannot draw: Animation is null");
            return;
        }

        if (_animation.Texture == null)
        {
            Console.WriteLine("Cannot draw: Animation texture is null");
            return;
        }

        try
        {
            SpriteEffects spriteEffects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            // Get the source rectangle for the current frame
            Rectangle sourceRect = new Rectangle(
                _currentFrame * _animation.FrameWidth, 
                0,
                _animation.FrameWidth, 
                _animation.FrameHeight
            );
            
            // Check if the source rectangle is valid
            if (sourceRect.X + sourceRect.Width > _animation.Texture.Width ||
                sourceRect.Y + sourceRect.Height > _animation.Texture.Height)
            {
                Console.WriteLine($"Warning: Source rectangle ({sourceRect}) exceeds texture bounds ({_animation.Texture.Width}x{_animation.Texture.Height})");
                
                // Adjust source rectangle to fit within texture bounds
                sourceRect.Width = Math.Min(sourceRect.Width, _animation.Texture.Width - sourceRect.X);
                sourceRect.Height = Math.Min(sourceRect.Height, _animation.Texture.Height);
            }
            
            spriteBatch.Draw(
                _animation.Texture,
                position,
                sourceRect,
                Color.White, 
                0f, 
                Vector2.Zero, 
                1f, 
                spriteEffects, 
                0f
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error drawing animation: {ex.Message}");
        }
    }

    public void DrawWithTint(SpriteBatch spriteBatch, Vector2 position, Color tint, bool flip = false)
    {
        if (_animation == null)
        {
            Console.WriteLine("Cannot draw: Animation is null");
            return;
        }

        if (_animation.Texture == null)
        {
            Console.WriteLine("Cannot draw: Animation texture is null");
            return;
        }

        try
        {
            SpriteEffects spriteEffects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            // Get the source rectangle for the current frame
            Rectangle sourceRect = new Rectangle(
                _currentFrame * _animation.FrameWidth, 
                0,
                _animation.FrameWidth, 
                _animation.FrameHeight
            );
            
            // Check if the source rectangle is valid
            if (sourceRect.X + sourceRect.Width > _animation.Texture.Width ||
                sourceRect.Y + sourceRect.Height > _animation.Texture.Height)
            {
                Console.WriteLine($"Warning: Source rectangle exceeds texture bounds");
                
                // Adjust source rectangle to fit within texture bounds
                sourceRect.Width = Math.Min(sourceRect.Width, _animation.Texture.Width - sourceRect.X);
                sourceRect.Height = Math.Min(sourceRect.Height, _animation.Texture.Height);
            }
            
            spriteBatch.Draw(
                _animation.Texture,
                position,
                sourceRect,
                tint,  // Use the tint color parameter
                0f, 
                Vector2.Zero, 
                1f, 
                spriteEffects, 
                0f
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error drawing animation with tint: {ex.Message}");
        }
    }
}
