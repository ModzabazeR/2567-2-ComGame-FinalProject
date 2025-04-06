// FragGrenade.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using FinalProject.Utils.SFXManager;

namespace FinalProject.GameObject.Weapon
{
    public class FragGrenade : Grenade
    {

        public FragGrenade(Vector2 position) : base(position)
        {
            explosionRadius = 150f;
            fuseTime = 2.5f;
        }

        protected override void Explode()
        {
            // Implement explosion logic
            SFXManager.Instance.PlaySound("bomb");
            base.Explode();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (EntityTexture != null)
            {
                spriteBatch.Draw(EntityTexture,
                    new Rectangle((int)Position.X, (int)Position.Y, EntityTexture.Width, EntityTexture.Height),
                    Color.White);
            }
            else
            {
                // Fallback code
                base.Draw(spriteBatch);
            }
        }
    }
}