using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using DragonBones;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FinalProject.GameObject
{
    public abstract class GameCharacter
    {
        // Basic properties
        protected Vector2 _position;
        public Vector2 Position 
        { 
            get => _position;
            set => _position = value;
        }
        
        protected Vector2 _velocity;
        public Vector2 Velocity 
        { 
            get => _velocity;
            set => _velocity = value;
        }
        
        protected Rectangle _bounds;
        public Rectangle Bounds 
        { 
            get => _bounds;
            protected set => _bounds = value;
        }
        
        // Physics properties
        protected float gravity = 1000f;
        protected bool isOnGround;
        
        // Animation properties
        protected Dictionary<string, Animation> spriteAnimations;
        protected AnimationManager animationManager;
        
        // DragonBones character
        public DragonBonesCharacter DragonBonesCharacter { get; set; }
        protected string currentAction = "Idle";
        
        // Debug drawing
        protected Texture2D debugTexture;
        
        public GameCharacter(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
        }
        
        public virtual void Update(GameTime gameTime, List<Rectangle> platforms)
        {
            // Call this method to update the DragonBones character
            UpdateDragonBonesCharacter();
            
            // ... other update code ...
        }
        
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (DragonBonesCharacter != null)
            {
                DragonBonesCharacter.Draw(spriteBatch);
            }
            else if (animationManager != null)
            {
                bool flip = Velocity.X < 0;
                animationManager.Draw(spriteBatch, Position, flip);
            }
            
            // Draw debug visuals if enabled
            if (Singleton.Instance.ShowDebugInfo)
            {
                DrawBoundingBox(spriteBatch);
            }
        }
        
        protected virtual void UpdateDragonBonesCharacter()
        {
            if (DragonBonesCharacter != null)
            {
                // Update the dragon bones character position to match the game character
                DragonBonesCharacter.Position = Position;
            }
        }
        
        protected virtual void DrawBoundingBox(SpriteBatch spriteBatch)
        {
            // Create a 1x1 white texture that we can resize
            if (debugTexture == null)
            {
                debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                debugTexture.SetData(new[] { Color.White });
            }
            
            // Draw the outline of the bounding box
            Color boundingBoxColor = Color.Red * 0.7f; // Semi-transparent red
            
            // Top line
            spriteBatch.Draw(debugTexture, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1), boundingBoxColor);
            // Bottom line
            spriteBatch.Draw(debugTexture, new Rectangle(Bounds.X, Bounds.Y + Bounds.Height, Bounds.Width, 1), boundingBoxColor);
            // Left line
            spriteBatch.Draw(debugTexture, new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height), boundingBoxColor);
            // Right line
            spriteBatch.Draw(debugTexture, new Rectangle(Bounds.X + Bounds.Width, Bounds.Y, 1, Bounds.Height), boundingBoxColor);
        }
        
        protected virtual void HandleCollisions(List<Rectangle> platforms, float deltaTime)
        {
            // To be implemented by derived classes
        }
    }
}