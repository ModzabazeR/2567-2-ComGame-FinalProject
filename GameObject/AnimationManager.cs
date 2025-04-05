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

        // Smooth frame advancement
        while (_timer >= _animation.FrameSpeed)
        {
            _timer -= _animation.FrameSpeed;
            _currentFrame++;

            if (_currentFrame >= _animation.FrameCount)
            {
                _currentFrame = _animation.IsLooping ? 0 : _animation.FrameCount - 1;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, bool flip = false)
    {
        SpriteEffects spriteEffects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        spriteBatch.Draw(_animation.Texture,
            position,
            new Rectangle(_currentFrame * _animation.FrameWidth, 0,
                          _animation.FrameWidth, _animation.FrameHeight),
            Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
    }
}
