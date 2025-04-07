using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using FinalProject.GameObject.Entity;

namespace FinalProject.GameObject
{
    public class ExplosionZone
    {
        public Rectangle Area { get; private set; }
        private float warningDuration = 1f;
        private float damageDuration = 0.7f;
        private float timer = 0f;
        private bool isExploded = false;
        private bool isFinished = false;

        public ExplosionZone(Vector2 position, int width = 100, int height = 100)
        {
            Area = new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (isFinished) return;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!isExploded && timer >= warningDuration)
            {
                // เริ่มระเบิด
                isExploded = true;
                timer = 0f;
            }

            // ถ้าระเบิดแล้วและยังอยู่ในช่วง damage
            if (isExploded && timer <= damageDuration)
            {
                if (Area.Intersects(player.Bounds))
                {
                    Vector2 knockback = Vector2.Normalize(player.Position - new Vector2(Area.X, Area.Y));
                    player.TakeDamage(1, knockback);
                }
            }

            // จบทุกอย่าง
            if (isExploded && timer > damageDuration)
            {
                isFinished = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isFinished) return;

            Texture2D tex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            tex.SetData(new[] { Color.White });

            Color color = isExploded ? Color.OrangeRed : Color.Red;

            spriteBatch.Draw(tex, Area, color * 0.5f);
        }

        public bool IsFinished()
        {
            return isFinished;
        }
    }
}
