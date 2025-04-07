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
                // bool isFacingRight = Singleton.Instance.Player.IsFacingRight;
                // Vector2 baseDirection = isFacingRight ? Vector2.UnitX : -Vector2.UnitX;
                // Vector2 spawnPos = Singleton.Instance.Player.Position +
                //                  new Vector2(isFacingRight ? 40 : -20, 20);

                // // Create spread pattern
                // for (int i = -2; i <= 2; i++)
                // {
                //     float angle = MathHelper.ToRadians(i * spread);
                //     Vector2 direction = isFacingRight ?
                //         new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) :
                //         new Vector2((float)-Math.Cos(angle), (float)Math.Sin(angle));

                //     var bullet = new Bullet(spawnPos, direction, 800f, damage, 2.0f , 12 , 12);
                //     Singleton.Instance.Bullets.Add(bullet);
                // }
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