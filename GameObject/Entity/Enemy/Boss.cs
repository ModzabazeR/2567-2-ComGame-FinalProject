using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.Utils.MapManager;
using FinalProject.GameObject.Weapon;
using FinalProject.Utils.SFXManager;

namespace FinalProject.GameObject.Entity.Enemy
{
    public class Boss : Enemy
    {
        private const float MOVE_SPEED = 100f;
        private float attackCooldown = 8.0f;
        private float currentAttackCooldown = 0f;
        private Random random = new Random();
        private string currentAttackType = "Idle";
        private Map bossMap;

        public List<Bullet> Bullets { get; set; } = new List<Bullet>();

        public Boss(Vector2 position)
            : base(Singleton.Instance.Animations["Boss"], position)
        {
            // Set boss properties
            enemyHP = 50; // Higher HP for the boss
            _currentAnimationKey = "Idle";
            _animationManager.Play(_animations["Idle"]);
            Bounds = new Rectangle(
                (int)(Position.X), // offset X เข้ากลาง
                (int)(Position.Y), // offset Y ลงมาให้ถึงขา
                Singleton.Instance.Animations["Boss"][_currentAnimationKey].FrameWidth,                     // กว้าง
                Singleton.Instance.Animations["Boss"][_currentAnimationKey].FrameHeight                      // สูง
            );
        }

        protected override void UpdateBounds()
        {
            Bounds = new Rectangle(
                (int)(Position.X), // offset X เข้ากลาง
                (int)(Position.Y), // offset Y ลงมาให้ถึงขา
                Singleton.Instance.Animations["Boss"][_currentAnimationKey].FrameWidth,                     // กว้าง
                Singleton.Instance.Animations["Boss"][_currentAnimationKey].FrameHeight                      // สูง
            );
        }

        public override void TakeDamage(int amount)
        {
            hit(amount);
            SFXManager.Instance.RandomPlayBossHurt();
            // You could add special effects or sounds here
        }

        public override void Update(GameTime gameTime, List<Rectangle> platforms)
        {

            if (!IsSpawned || IsDefeated)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle attack cooldown
            if (currentAttackCooldown > 0)
            {
                currentAttackCooldown -= dt;
            }
            else
            {
                // Choose a random attack when cooldown expires
                ChooseRandomAttack();
                currentAttackCooldown = attackCooldown;
            }

            // Update animation based on current attack
            _currentAnimationKey = currentAttackType;
            Animation animationToPlay = _animations[currentAttackType];
            _animationManager.Play(animationToPlay);

            // Call the base update which handles physics, etc.
            base.Update(gameTime, platforms);
        }

        private void ChooseRandomAttack()
        {
            // Randomly select an attack type
            int attackChoice = random.Next(4);
            switch (attackChoice)
            {
                case 0:
                    currentAttackType = "SwordAttack";
                    Console.WriteLine("swordattack");
                    FireBulletLeftdown();
                    break;
                case 1:
                    currentAttackType = "GunAttack";
                    Console.WriteLine("gunatt");
                    FireBulletLeftup();
                    break;
                case 2:
                    currentAttackType = "PickupGrenade";
                    currentAttackType = "ThrowGrenade";
                    Console.WriteLine("boomatt");
                    CreateExplosionZone();
                    break;
                case 3:
                    currentAttackType = "SwordAttack";
                    Console.WriteLine("spawnminion");
                    spawnMinions();
                    break;
                default:
                    currentAttackType = "Idle";
                    SFXManager.Instance.RandomPlayBossTakling();
                    break;
            }
        }

        public override void Defeat()
        {
            _currentAnimationKey = "Death";
            _animationManager.Play(_animations["Death"]);
            SFXManager.Instance.playBossDeath();

            // Add any special defeat logic here
            base.Defeat();

            // Ensure the boss map is marked as cleared
            if (bossMap == null)
            {
                bossMap = Singleton.Instance.MapManager.GetMap("Boss");
            }

            // Force check if the map is cleared after boss defeat
            if (bossMap != null)
            {
                // Force trigger the map cleared event
                bossMap.ForceMapCleared();
            }
        }

        private void spawnMinions()
        {
            SFXManager.Instance.playBossMelee();
            bossMap = Singleton.Instance.MapManager.GetMap("Boss");
            List<SimpleEnemy> newMinions = new List<SimpleEnemy>();
            for (int i = 0; i < 2; i++)
            {
                int randomX = random.Next(10, 30);
                int randomY = random.Next(2, 15);
                var enemy = new SimpleEnemy(Singleton.Instance.Animations["Zombie"], bossMap.TileToWorldPosition(randomX, randomY));
                enemy.SetParentMap(bossMap);
                newMinions.Add(enemy);
            }
            bossMap.GetEnemies().AddRange(newMinions);
        }

        private void FireBulletLeftup()
        {
            SFXManager.Instance.playBossGunfire();

            float baseSpeed = 300f;
            float baseDamage = 2f;
            float lifetime = 3f;

            int bulletWidth = 50;
            int bulletHeight = 50;

            // ตำแหน่งเริ่มกลางของปากปืน
            float startX = Position.X + 20;
            float startY = Position.Y + 380;

            // ยิง 3 นัด เรียงในแนว X (ห่างกันแนวนอน)
            for (int offsetX = -50; offsetX <= 50; offsetX += 50)
            {
                Vector2 spawn = new Vector2(startX + offsetX + 50, startY);
                Vector2 dir = new Vector2(-1f, 0f); // ยิงซ้ายตรง

                var bullet = new Bullet(
                    spawn,
                    dir,
                    speed: baseSpeed,
                    damage: baseDamage,
                    lifetime: lifetime,
                    widths: bulletWidth,
                    height: bulletHeight
                )
                {
                    bulletTexture = Singleton.Instance.BossSmallBulletTexture // Assign texture here
                };

                Singleton.Instance.EnemyBullets.Add(bullet);
            }

        }

        private void FireBulletLeftdown()
        {
            Vector2 spawnPos = new Vector2(Position.X, Position.Y + 400);
            Vector2 dir = new Vector2(-1f, 0f); // ยิงไปทางซ้าย

            var bullet = new Bullet(
                spawnPos,
                dir,
                speed: 300f,
                damage: 5f,
                lifetime: 3f,
                80,      // ✅ กำหนดขนาดที่ต้องการ
                80
            )
            {
                bulletTexture = Singleton.Instance.BossBigBulletTexture // Assign texture here
            }; ;

            Singleton.Instance.EnemyBullets.Add(bullet);

        }

        private void CreateExplosionZone()
        {
            SFXManager.Instance.playBossGrenade();
            Vector2 pos = new Vector2(Position.X - 550, Position.Y + 475); // ตำแหน่งพื้น
            var zone = new ExplosionZone(pos, 800, 50);
            MainScene.explosionZones.Add(zone); // ✅ ใช้ list เดียวกับ MainScene
        }

    }
}