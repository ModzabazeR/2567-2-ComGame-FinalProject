// FragGrenade.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalProject.GameObject.Weapon
{
    public class FragGrenade : Grenade
    {
        //public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 20, 20);

        public FragGrenade(Vector2 position) : base(position)
        {
            explosionRadius = 150f;
            fuseTime = 2.5f;
        }

        protected override void Explode()
        {
            // Implement explosion logic
            base.Explode();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Add drawing logic
            if (texture == null)
            {
                texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                texture.SetData(new[] { Color.DarkGreen });
            }
            spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, 20, 20), Color.White);
        }
    }
}