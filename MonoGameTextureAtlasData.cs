using DragonBones;
using Microsoft.Xna.Framework.Graphics;

public class MonoGameTextureAtlasData : TextureAtlasData
{
    public Texture2D texture;

    protected override void _OnClear()
    {
        base._OnClear();
        texture = null;
    }

    public override TextureData CreateTexture()
    {
        return BaseObject.BorrowObject<MonoGameTextureData>();
    }
}

public class MonoGameTextureData : TextureData
{
    // Additional MonoGame-specific properties, if needed
    
    protected override void _OnClear()
    {
        base._OnClear();
        // Clear any MonoGame-specific properties
    }
}