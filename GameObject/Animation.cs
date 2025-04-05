    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;

    namespace FinalProject.GameObject;

    public class Animation
    {
        public Texture2D Texture { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public int FrameCount { get; private set; }
        public float FrameSpeed { get; private set; }
        public bool IsLooping { get; private set; }

        public Animation(Texture2D texture, int frameWidth, int frameHeight, int frameCount, float frameSpeed, bool isLooping = true)
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCount = frameCount;
            FrameSpeed = frameSpeed;
            IsLooping = isLooping;
        }
    }
