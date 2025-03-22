using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DragonBones;

public class MonoGameArmatureDisplay : IArmatureProxy
{
    public Armature Armature { get; private set; }
    public Texture2D Texture { get; private set; }
    public bool FlipX { get; set; } = false;
    public bool FlipY { get; set; } = false;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Scale { get; set; } = Vector2.One;
    public float Rotation { get; set; } = 0.0f;

    public MonoGameArmatureDisplay(Armature armature, Texture2D texture)
    {
        Armature = armature;
        Texture = texture;
        
        // Set this as the display for the armature
        DBInit(armature);
        
        // Default animation
        armature.animation.Play(armature.animation.animationNames[0], -1);
    }

    public void DBClear()
    {
        // Cleanup method required by IArmatureProxy
        Armature = null;
    }

    public void DBInit(Armature armature)
    {
        // Initialization method required by IArmatureProxy
        Armature = armature;
    }

    public void Dispose(bool disposeProxy)
    {
        // Dispose method required by IArmatureProxy
        if (disposeProxy)
        {
            Armature = null;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Let the armature update itself
        if (Armature != null)
        {
            // The actual drawing is handled by slots which implement ISlotDisplay
            foreach (var slot in Armature.GetSlots())
            {
                if (slot.display is MonoGameSlotDisplay slotDisplay)
                {
                    slotDisplay.Draw(spriteBatch, Texture, Position, FlipX, FlipY, Scale.X);
                }
            }
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        Position = position;
    }

    // Implement other IArmatureProxy methods as needed
    public void DBUpdate() { }
    public Armature armature => Armature;

    // Implementing the animation property
    public DragonBones.Animation animation => Armature.animation;

    // Implementing IEventDispatcher<EventObject> methods
    public bool HasDBEventListener(string type) => false;
    public void DispatchDBEvent(string type, EventObject eventObject) { }
    public void AddDBEventListener(string type, ListenerDelegate<EventObject> listener) { }
    public void RemoveDBEventListener(string type, ListenerDelegate<EventObject> listener) { }
} 