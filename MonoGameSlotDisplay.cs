using DragonBones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using FinalProject;

public class MonoGameSlotDisplay : ISlotDisplay
{
    private Microsoft.Xna.Framework.Rectangle _sourceRect;
    private Vector2 _origin;
    private Vector2 _position;
    private float _rotation;
    private Vector2 _scale = Vector2.One;
    private Color _color = Color.White;

    public Microsoft.Xna.Framework.Rectangle TextureRegion { get; set; }
    public bool visible { get; set; } = true;

    public MonoGameSlotDisplay()
    {
        TextureRegion = Microsoft.Xna.Framework.Rectangle.Empty;
    }

    public void UpdateTransform(DragonBones.Matrix matrix)
    {
        // Extract position, rotation, and scale from the transformation matrix
        _position = new Vector2(matrix.tx, matrix.ty);
        _rotation = (float)Math.Atan2(matrix.b, matrix.a);
        _scale = new Vector2(
            (float)Math.Sqrt(matrix.a * matrix.a + matrix.b * matrix.b),
            (float)Math.Sqrt(matrix.c * matrix.c + matrix.d * matrix.d)
        );
    }

    public void UpdateFrame(TextureAtlasData textureAtlasData, TextureData textureData)
    {
        if (textureData != null)
        {
            // Set the source rectangle based on the texture data
            _sourceRect = new Microsoft.Xna.Framework.Rectangle(
                (int)textureData.region.x,
                (int)textureData.region.y,
                (int)textureData.region.width,
                (int)textureData.region.height
            );
            
            // Set the origin to the pivot point
            _origin = new Vector2(
                textureData.region.width * 0.5f,  // Center X
                textureData.region.height * 0.5f  // Center Y
            );
        }
        else
        {
            _sourceRect = Microsoft.Xna.Framework.Rectangle.Empty;
        }
    }

    public void UpdateColor(float r, float g, float b, float a)
    {
        _color = new Color(r, g, b, a);
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool flipX, bool flipY, float scale)
    {
        if (!visible || texture == null)
            return;
        
        try
        {
            // Use the _sourceRect for drawing if it's valid, otherwise use TextureRegion
            Microsoft.Xna.Framework.Rectangle sourceRect = !_sourceRect.IsEmpty ? _sourceRect : TextureRegion;
            
            // If we still don't have a valid source rectangle, draw the entire texture
            if (sourceRect.IsEmpty)
            {
                sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height);
            }
            
            // Calculate the final position: character position + slot position
            Vector2 finalPosition = position + _position;
            
            SpriteEffects effects = SpriteEffects.None;
            if (flipX) effects |= SpriteEffects.FlipHorizontally;
            if (flipY) effects |= SpriteEffects.FlipVertically;
            
            // Draw the sprite
            spriteBatch.Draw(
                texture,
                finalPosition,
                sourceRect,
                _color,
                _rotation,
                _origin,
                _scale * scale,
                effects,
                0f
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Draw: {ex.Message}");
        }
    }

    public bool containsPoint(float x, float y) => false;
    public DisplayType displayType => DisplayType.Image;
    public DragonBones.Rectangle boundingBoxData => new DragonBones.Rectangle() { x = TextureRegion.X, y = TextureRegion.Y, width = TextureRegion.Width, height = TextureRegion.Height };
    public float zOrder { get; set; }
    public Microsoft.Xna.Framework.Matrix transform { get; set; } = Microsoft.Xna.Framework.Matrix.Identity;
} 