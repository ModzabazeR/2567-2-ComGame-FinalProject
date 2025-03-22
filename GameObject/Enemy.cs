using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FinalProject.GameObject
{
    public class Enemy : GameCharacter
    {
        private float direction = 1f;
        private float speed = 100f;
        private float maxFallSpeed = 500f;
        private float moveTime = 0f;
        private float timer = 0f;
        private Texture2D texture;

        public Enemy(Texture2D texture, Vector2 position) : base(position)
        {
            // Store the texture
            this.texture = texture;
            
            // Initialize bounds based on texture size
            Bounds = new Rectangle(
                (int)position.X,
                (int)position.Y,
                texture.Width,
                texture.Height
            );
        }

        public override void Update(GameTime gameTime, List<Rectangle> platforms)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Apply gravity
            var newVelocity = Velocity;
            newVelocity.Y += gravity * deltaTime;
            newVelocity.Y = Math.Min(newVelocity.Y, maxFallSpeed);
            Velocity = newVelocity;
            
            // Move horizontally
            var newPosition = Position;
            newPosition.X += direction * speed * deltaTime;
            Position = newPosition;
            
            // Update bounds
            var newBounds = Bounds;
            newBounds.X = (int)Position.X;
            newBounds.Y = (int)Position.Y;
            Bounds = newBounds;
            
            // Handle collisions
            HandleCollisions(platforms, deltaTime);
            
            // Update DragonBones character
            UpdateDragonBonesCharacter();
        }

        protected override void HandleCollisions(List<Rectangle> platforms, float deltaTime)
        {
            // Horizontal collision
            Rectangle horizontalBounds = new Rectangle(
                (int)(Position.X + direction * speed * deltaTime),
                (int)Position.Y,
                Bounds.Width,
                Bounds.Height
            );

            foreach (var platform in platforms)
            {
                if (horizontalBounds.Intersects(platform))
                {
                    direction *= -1;
                    break;
                }
            }

            // Vertical collision
            Rectangle verticalBounds = new Rectangle(
                (int)Position.X,
                (int)(Position.Y + Velocity.Y * deltaTime),
                Bounds.Width,
                Bounds.Height
            );

            foreach (var platform in platforms)
            {
                if (verticalBounds.Intersects(platform))
                {
                    if (Velocity.Y > 0)
                    {
                        var newPosition = Position;
                        newPosition.Y = platform.Top - Bounds.Height;
                        Position = newPosition;
                        
                        var newVelocity = Velocity;
                        newVelocity.Y = 0;
                        Velocity = newVelocity;
                        
                        isOnGround = true;
                    }
                    else
                    {
                        var newPosition = Position;
                        newPosition.Y = platform.Bottom;
                        Position = newPosition;
                        
                        var newVelocity = Velocity;
                        newVelocity.Y = 0;
                        Velocity = newVelocity;
                    }
                    break;
                }
            }
        }

        protected override void UpdateDragonBonesCharacter()
        {
            if (DragonBonesCharacter == null)
                return;
                
            // Update position
            DragonBonesCharacter.Position = Position;
            
            // Update animation based on current state
            DragonBonesCharacter.PlayAnimation(currentAction);
            
            // Set horizontal flip based on movement direction
            DragonBonesCharacter.FlipX = direction < 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the main texture, no DragonBones character for now
            bool flip = Velocity.X < 0;
            if (animationManager != null && animationManager.Animation != null)
            {
                animationManager.Draw(spriteBatch, Position, flip);
            }
            else if (texture != null)
            {
                // Fallback to simple texture if no animation
                spriteBatch.Draw(
                    texture, 
                    new Rectangle((int)Position.X, (int)Position.Y, 64, 64), 
                    null, 
                    Color.White, 
                    0f, 
                    Vector2.Zero, 
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 
                    0f
                );
            }
            
            // Draw debug info only if requested
            if (Singleton.Instance.ShowDebugInfo && debugTexture != null)
            {
                spriteBatch.Draw(debugTexture, Bounds, Color.Yellow * 0.3f);
            }
        }
    }
} 