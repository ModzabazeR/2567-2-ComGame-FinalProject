using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace FinalProject
{
    public class Enemy
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Rectangle bounds;
        private int direction; // -1 = ซ้าย, 1 = ขวา
        private float speed;
        private float gravity = 0.5f;
        private float maxFallSpeed = 10f; // จำกัดความเร็วตก
        private float moveTime;
        private float timer;
        private bool isOnGround;

        // Debug Texture
        private static Texture2D debugTexture;
        private static bool debugTextureInitialized = false;

        public Rectangle Bounds => bounds;

        public Enemy(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            this.position = startPosition;
            this.velocity = Vector2.Zero;
            this.speed = 2f;
            this.direction = Singleton.Instance.Random.Next(0, 2) == 0 ? -1 : 1;
            this.moveTime = Singleton.Instance.Random.Next(2, 5);
            this.timer = 0;
            this.isOnGround = false;

            bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            // สร้าง Debug Texture ถ้ายังไม่มี
            if (!debugTextureInitialized)
            {
                debugTexture = new Texture2D(texture.GraphicsDevice, 1, 1);
                debugTexture.SetData(new[] { Color.Red });
                debugTextureInitialized = true;
            }
        }

        public void Update(GameTime gameTime, List<Rectangle> solidTiles)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // เปลี่ยนทิศทางแบบสุ่ม
            if (timer >= moveTime)
            {
                direction *= -1;
                timer = 0;
                moveTime = Singleton.Instance.Random.Next(2, 5);
            }

            // อัปเดตความเร็วแกน Y (แรงโน้มถ่วง)
            velocity.Y += gravity;
            if (velocity.Y > maxFallSpeed)
                velocity.Y = maxFallSpeed; // จำกัดความเร็วตก

            // อัปเดตตำแหน่ง
            position.X += speed * direction;
            position.Y += velocity.Y;

            // ตรวจสอบการชนกับพื้น
            isOnGround = false;
            foreach (var tile in solidTiles)
            {
                if (bounds.Intersects(tile))
                {
                    if (velocity.Y > 0) // ตกลงมาแล้วชนพื้น
                    {
                        position.Y = tile.Top - bounds.Height; // วางศัตรูบนพื้น
                        velocity.Y = 0; // ทำให้หยุดตกทันที
                        isOnGround = true;
                    }
                }
            }

            // ถ้าศัตรูอยู่บนพื้นให้เดินซ้าย-ขวาได้
            if (isOnGround)
            {
                position.X += speed * direction;
            }

            // ตรวจสอบว่ากำลังจะเดินออกจากพื้นไหม
            bool hasGround = false;
            foreach (var tile in solidTiles)
            {
                Rectangle groundCheck = new Rectangle(
                    (int)(position.X + (direction * speed)), // ตำแหน่งด้านหน้าศัตรู
                    (int)(position.Y + bounds.Height + 5),   // ตำแหน่งเช็คพื้น (ต่ำกว่าตัวเล็กน้อย)
                    bounds.Width, 5 // ความกว้างและความสูงของ hitbox ตรวจสอบพื้น
                );

                if (tile.Intersects(groundCheck))
                {
                    hasGround = true;
                    break;
                }
            }

            // ถ้ากำลังจะเดินออกจากพื้น ให้เปลี่ยนทิศทาง
            if (!hasGround)
            {
                direction *= -1;
            }

            // อัปเดต hitbox
            bounds.X = (int)position.X;
            bounds.Y = (int)position.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);

            // วาด Hitbox Debug ถ้าเปิดโหมด Debug
            if (Singleton.Instance.ShowDebugInfo)
            {
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X, bounds.Y, bounds.Width, 2), Color.Red); // ขอบบน
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, 2), Color.Red); // ขอบล่าง
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X, bounds.Y, 2, bounds.Height), Color.Red); // ขอบซ้าย
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X + bounds.Width, bounds.Y, 2, bounds.Height), Color.Red); // ขอบขวา
            }
        }
    }
}