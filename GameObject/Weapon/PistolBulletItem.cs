using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using FinalProject.GameObject.Entity.Enemy;

namespace FinalProject.GameObject.Weapon
{
    public class PistolBulletItem : Weapon
    {
        //private static Texture2D texture;

        public PistolBulletItem(Vector2 position) : base(position)
        {
            Bounds = new Rectangle((int)position.X, (int)position.Y, 40, 40);
        }

        public override void Update(GameTime gameTime, List<Rectangle> platforms)
        {
            base.Update(gameTime, platforms);
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 40, 40);
        }

        protected override void PerformAttack()
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null)
            {
                texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                texture.SetData(new[] { Color.Yellow });
            }

            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}
