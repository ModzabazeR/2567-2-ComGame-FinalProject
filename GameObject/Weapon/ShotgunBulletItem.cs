using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalProject.GameObject.Weapon
{
    public class ShotgunBulletItem : Weapon
    {
        //private static Texture2D texture;

        public ShotgunBulletItem(Vector2 position) : base(position)
        {
            Bounds = new Rectangle((int)position.X, (int)position.Y, 40, 40);
        }

        public override void Update(GameTime gameTime, List<Rectangle> platforms)
        {
            base.Update(gameTime, platforms);
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 40, 40);
        }

        public override void PerformAttack()
        {

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
