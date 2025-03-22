using DragonBones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface ISlotDisplay
{
    void UpdateTransform(DragonBones.Matrix matrix);
    void UpdateFrame(TextureAtlasData textureAtlasData, TextureData textureData);
    void UpdateColor(float r, float g, float b, float a);
    void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, bool flipX, bool flipY, float scale);
} 