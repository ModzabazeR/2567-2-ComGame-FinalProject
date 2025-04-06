using Microsoft.Xna.Framework;
using FinalProject.GameObject.Entity;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FinalProject.GameObject.Weapon
{
    public class Shotgun : RangeWeapon
    {
        private Texture2D _texture;

        //public override Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 40, 20);

        public Shotgun(Vector2 position) : base(position)
        {
            cooldown = 1.0f;
            damage = 30f;
            maxAmmo = 6;
            currentAmmo = maxAmmo;
            spread = 15f;
        }

        public override void PerformAttack()
        {
            if (currentAmmo > 0 && Singleton.Instance.Player != null)
            {
                bool isFacingRight = Singleton.Instance.Player.IsFacingRight;
                Vector2 baseDirection = isFacingRight ? Vector2.UnitX : -Vector2.UnitX;
                Vector2 spawnPos = Singleton.Instance.Player.Position + 
                                 new Vector2(isFacingRight ? 40 : -20, 20);

                // Create spread pattern
                for (int i = -2; i <= 2; i++)
                {
                    float angle = MathHelper.ToRadians(i * spread);
                    Vector2 direction = isFacingRight ? 
                        new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) :
                        new Vector2((float)-Math.Cos(angle), (float)Math.Sin(angle));

                    var bullet = new Bullet(spawnPos, direction, 800f, damage, 2.0f);
                    Singleton.Instance.Bullets.Add(bullet);
                }
                currentAmmo--;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _texture.SetData(new[] { Color.DarkSlateGray });
                texture = _texture;
            }
            spriteBatch.Draw(_texture, 
                new Rectangle((int)Position.X, (int)Position.Y, 40, 20), 
                Color.White);
        }
    }
}