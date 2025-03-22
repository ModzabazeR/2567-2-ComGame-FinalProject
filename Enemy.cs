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
        private int direction; // -1 = ����, 1 = ���
        private float speed;
        private float gravity = 0.5f;
        private float maxFallSpeed = 10f; // �ӡѴ�������ǵ�
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

            // ���ҧ Debug Texture ����ѧ�����
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

            // ����¹��ȷҧẺ����
            if (timer >= moveTime)
            {
                direction *= -1;
                timer = 0;
                moveTime = Singleton.Instance.Random.Next(2, 5);
            }

            // �ѻവ��������᡹ Y (�ç�����ǧ)
            velocity.Y += gravity;
            if (velocity.Y > maxFallSpeed)
                velocity.Y = maxFallSpeed; // �ӡѴ�������ǵ�

            // �ѻവ���˹�
            position.X += speed * direction;
            position.Y += velocity.Y;

            // ��Ǩ�ͺ��ê��Ѻ���
            isOnGround = false;
            foreach (var tile in solidTiles)
            {
                if (bounds.Intersects(tile))
                {
                    if (velocity.Y > 0) // ��ŧ�����Ǫ����
                    {
                        position.Y = tile.Top - bounds.Height; // �ҧ�ѵ�ٺ����
                        velocity.Y = 0; // �������ش���ѹ��
                        isOnGround = true;
                    }
                }
            }

            // ����ѵ�����躹�������Թ����-�����
            if (isOnGround)
            {
                position.X += speed * direction;
            }

            // ��Ǩ�ͺ��ҡ��ѧ���Թ�͡�ҡ������
            bool hasGround = false;
            foreach (var tile in solidTiles)
            {
                Rectangle groundCheck = new Rectangle(
                    (int)(position.X + (direction * speed)), // ���˹觴�ҹ˹���ѵ��
                    (int)(position.Y + bounds.Height + 5),   // ���˹��社�� (��ӡ��ҵ����硹���)
                    bounds.Width, 5 // �������ҧ��Ф����٧�ͧ hitbox ��Ǩ�ͺ���
                );

                if (tile.Intersects(groundCheck))
                {
                    hasGround = true;
                    break;
                }
            }

            // ��ҡ��ѧ���Թ�͡�ҡ��� �������¹��ȷҧ
            if (!hasGround)
            {
                direction *= -1;
            }

            // �ѻവ hitbox
            bounds.X = (int)position.X;
            bounds.Y = (int)position.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);

            // �Ҵ Hitbox Debug ����Դ���� Debug
            if (Singleton.Instance.ShowDebugInfo)
            {
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X, bounds.Y, bounds.Width, 2), Color.Red); // �ͺ��
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, 2), Color.Red); // �ͺ��ҧ
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X, bounds.Y, 2, bounds.Height), Color.Red); // �ͺ����
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X + bounds.Width, bounds.Y, 2, bounds.Height), Color.Red); // �ͺ���
            }
        }
    }
}