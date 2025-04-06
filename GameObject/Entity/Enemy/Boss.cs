using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.Utils.MapManager;
using FinalProject.GameObject.Weapon;

namespace FinalProject.GameObject.Entity.Enemy
{
    public class Boss : Enemy
    {
        private const float MOVE_SPEED = 100f;
        private float attackCooldown = 2.0f;
        private float currentAttackCooldown = 0f;
        private Random random = new Random();
        private string currentAttackType = "Idle";
        
        public Boss(Vector2 position) 
            : base(Singleton.Instance.Animations["Boss"], position)
        {
            // Set boss properties
            enemyHP = 50; // Higher HP for the boss
            _currentAnimationKey = "Idle";
            _animationManager.Play(_animations["Idle"]);
        }

        public override void TakeDamage(int amount)
        {
            hit(amount);
            // You could add special effects or sounds here
        }

        public override void Update(GameTime gameTime, List<Rectangle> platforms)
        {
            //Bounds = new Rectangle((int)Position.X, (int)Position.Y, 120, 160); // หรือขนาดตาม sprite จริง

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
            _animationManager.Play(_animations[currentAttackType]);
            
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
                    break;
                case 1:
                    currentAttackType = "GunAttack";
                    break;
                case 2:
                    currentAttackType = "PickupGrenade";
                    break;
                case 3:
                    currentAttackType = "ThrowGrenade";
                    break;
                default:
                    currentAttackType = "Idle";
                    break;
            }
        }
        
        public override void Defeat()
        {
            _currentAnimationKey = "Death";
            _animationManager.Play(_animations["Death"]);
            // Add any special defeat logic here
            base.Defeat();
        }

    }
} 