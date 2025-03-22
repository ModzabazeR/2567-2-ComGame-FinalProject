using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DragonBones;
using System.Collections.Generic;
using System;

namespace FinalProject.GameObject
{
    public class DragonBonesCharacter
    {
        public Armature Armature { get; private set; }
        public MonoGameArmatureDisplay ArmatureDisplay { get; private set; }
        
        // Store rotation locally
        private float _rotation = 0f;
        
        public Vector2 Position 
        { 
            get => ArmatureDisplay.Position;
            set => ArmatureDisplay.Position = value;
        }
        
        public Vector2 Scale 
        { 
            get => ArmatureDisplay.Scale;
            set => ArmatureDisplay.Scale = value;
        }
        
        public float Rotation 
        { 
            get => _rotation;
            set => _rotation = value; // Store rotation value for later use
        }
        
        public bool FlipX 
        { 
            get => ArmatureDisplay.FlipX;
            set => ArmatureDisplay.FlipX = value;
        }
        
        public Dictionary<string, string> _animationMappings = new Dictionary<string, string>();
        private string _currentAnimation = null;

        public DragonBonesCharacter(Armature armature, MonoGameArmatureDisplay armatureDisplay, Vector2 position)
        {
            Armature = armature;
            ArmatureDisplay = armatureDisplay;
            Position = position;
            Scale = Vector2.One;
        }

        public void AddAnimationMapping(string gameAction, string dragonBonesAnimation)
        {
            _animationMappings[gameAction] = dragonBonesAnimation;
        }

        public void PlayAnimation(string gameAction, int playTimes = -1)
        {
            if (_animationMappings.TryGetValue(gameAction, out string animationName))
            {
                Armature.animation.Play(animationName, playTimes);
                _currentAnimation = gameAction;
                Console.WriteLine($"Playing animation: {gameAction} ? {animationName}");
            }
            else
            {
                Console.WriteLine($"Animation mapping not found for action: {gameAction}");
            }
        }

        public void StopAnimation()
        {
            Armature.animation.Stop();
            _currentAnimation = null;
        }

        public bool IsPlayingAnimation(string action)
        {
            if (_animationMappings.TryGetValue(action, out string animationName))
            {
                return _currentAnimation == animationName && 
                       Armature.animation.lastAnimationState != null;
            }
            return false;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
            ArmatureDisplay.UpdatePosition(position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (ArmatureDisplay != null)
            {
                ArmatureDisplay.Draw(spriteBatch);
            }
        }
    }
} 