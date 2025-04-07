using Microsoft.Xna.Framework;
using FinalProject.GameObject.Entity;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.Utils.SFXManager;
using System;

namespace FinalProject.GameObject.Weapon
{
    public class Shotgun : RangeWeapon
    {
        public Shotgun(Vector2 position) : base(position)
        {
            cooldown = 1.0f;
            damage = 10;
            maxAmmo = 6;
            currentAmmo = maxAmmo;
            spread = 15f;
        }

        public override void PerformAttack()
        {
            if (currentAmmo > 0 && Singleton.Instance.Player != null)
            {
                SFXManager.Instance.PlaySound("Shotgun_Shot-002");
                currentAmmo--;
            }
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