using Microsoft.Xna.Framework;
using FinalProject.GameObject.Entity;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.Utils.SFXManager;

namespace FinalProject.GameObject.Weapon
{
    public class Pistol : RangeWeapon
    {
        private Texture2D _texture;

        public Pistol(Vector2 position) : base(position)
        {
            cooldown = 0.3f;
            damage = 15f;
            maxAmmo = 12;
            currentAmmo = maxAmmo;
        }

        public override void PerformAttack()
        {
            if (currentAmmo > 0 && Singleton.Instance.Player != null)
            {
                SFXManager.Instance.PlaySound("9mm Single");

                // Get direction from player facing
                bool isFacingRight = Singleton.Instance.Player.IsFacingRight;
                Vector2 direction = isFacingRight ? Vector2.UnitX : -Vector2.UnitX;

                // Create bullet with offset from player center
                Vector2 spawnPos = Singleton.Instance.Player.Position +
                                 new Vector2(isFacingRight ? 40 : -20, 20);
                
                var bullet = new Bullet(spawnPos, direction, 800f, damage, 2.0f , 12 , 12);
                Singleton.Instance.Bullets.Add(bullet);
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