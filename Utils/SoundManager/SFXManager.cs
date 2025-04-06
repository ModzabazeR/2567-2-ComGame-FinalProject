using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;

namespace FinalProject.Utils.SFXManager
{
    public class SFXManager
    {
        private static SFXManager instance;
        private ContentManager content;


        public static SFXManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SFXManager();
                return instance;
            }
        }

        // Removed duplicate constructor

        public void Initialize(ContentManager content)
        {
            this.content = content;
        }



        public void SetVolume(float volume)
        {
            SoundEffect.MasterVolume = volume;
        }

        public void PlaySound(string soundName)
        {
            soundName = "SFX/" + soundName;
            SoundEffect soundEffect = content.Load<SoundEffect>(soundName);
            soundEffect.Play();
        }

        public void StopSound(string soundName)
        {
            SoundEffect soundEffect = content.Load<SoundEffect>(soundName);
            SoundEffectInstance instance = soundEffect.CreateInstance();
            instance.Stop(true);
        }

        public void RandomPlayBossTakling()
        {
            string[] soundNames = ["I am the queue's closest servant", "You are gay, player", "Why are you gay, player", "Why can't yout talk to me"];
            Random random = new();
            int randomIndex = random.Next(0, 4); // Assuming you have 4 sound effects
            string soundName = soundNames[randomIndex];
            PlaySound(soundName);
        }

        public void PlayFootSteps()
        {
            // Uncomment and use randomized footsteps for variation
            string[] steps = {
                "FootStepsConcrete1",
                "FootStepsConcrete2",
                "FootStepsConcrete3",
                "FootStepsConcrete4"
    };
            Random rand = new Random();
            PlaySound(steps[rand.Next(0, steps.Length)]);
        }

        public void RandomPlayBossHurt()
        {
            string[] soundNames = ["itaiahh", "oh mommy queue i'm hurt", "yametekudasai", "mama help me"];
            Random random = new();
            int randomIndex = random.Next(0, 4); // Assuming you have 4 sound effects
            Console.WriteLine(soundNames[randomIndex]);
            string soundName = soundNames[randomIndex];
            PlaySound(soundName);
        }

        public void RandomPlayZombieScream()
        {
            string[] soundNames = ["Zombie_6", "Zombie_8", "Zombie"];
            Random random = new();
            int randomIndex = random.Next(0, 3); // Assuming you have 3 sound effects
            string soundName = soundNames[randomIndex];
            PlaySound(soundName);
        }

        public void playDoorLock()
        {
            PlaySound("Door_handle_jiggle_checking if locked");
        }

        public void playDoorOpen()
        {
            PlaySound("Door_squeaky_2");
        }

        public void playBossDeath()
        {
            PlaySound("dedi dedi amkaming ohhh");
        }

        public void playBossGunfire()
        {
            PlaySound("I need more bulletx2");
            for (int i = 0; i < 3; i++)
            {
                PlaySound("762x39 Spray MP3");
            }
        }

        public void playBossGrenade()
        {
            PlaySound("take my nut");
            for (int i = 0; i < 3; i++)
            {
                PlaySound("762x39 Spray MP3");
            }
        }

        public void playBossGrenadeExplosion()
        {
            PlaySound("bomb");
        }

        public void playBossMelee()
        {
            PlaySound("how good is my new arm");
        }

        //         GraphicsDeviceManager graphics;
        //         SpriteBatch spriteBatch;
        //         List<SoundEffect> soundEffects;
        // 
        //         public Game1()
        //         {
        //             graphics = new GraphicsDeviceManager(this);
        //             Content.RootDirectory = "Content";
        //             soundEffects = new List<SoundEffect>();
        //         }
        // 
        //         protected override void Initialize()
        //         {
        //             base.Initialize();
        //         }
        // 
        //         protected override void LoadContent()
        //         {
        //             // Create a new SpriteBatch, which can be used to draw 
        //             textures.
        //             spriteBatch = new SpriteBatch(GraphicsDevice);
        // 
        //             soundEffects.Add(Content.Load<SoundEffect>("airlockclose"))
        //                              ;
        //             soundEffects.Add(Content.Load<SoundEffect>("ak47"));
        //             soundEffects.Add(Content.Load<SoundEffect>("icecream"));
        //             soundEffects.Add(Content.Load<SoundEffect>("sneeze"));
        // 
        //             // Fire and forget play
        //             soundEffects[0].Play();
        //             
        //             // Play that can be manipulated after the fact
        //             var instance = soundEffects[0].CreateInstance();
        //             instance.IsLooped = true;
        //             instance.Play();
        //         }
        // 
        // 
        //         protected override void Update(GameTime gameTime)
        //         {
        //             if (GamePad.GetState(PlayerIndex.One).Buttons.Back == 
        //                 ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
        //                 Keys.Escape))
        //                 Exit();
        // 
        //             if (Keyboard.GetState().IsKeyDown(Keys.D1))
        //                 soundEffects[0].CreateInstance().Play();
        //             if (Keyboard.GetState().IsKeyDown(Keys.D2))
        //                 soundEffects[1].CreateInstance().Play();
        //             if (Keyboard.GetState().IsKeyDown(Keys.D3))
        //                 soundEffects[2].CreateInstance().Play();
        //             if (Keyboard.GetState().IsKeyDown(Keys.D4))
        //                 soundEffects[3].CreateInstance().Play();
        // 
        // 
        //             if (Keyboard.GetState().IsKeyDown(Keys.Space))
        //             {
        //                 if (SoundEffect.MasterVolume == 0.0f)
        //                     SoundEffect.MasterVolume = 1.0f;
        //                 else
        //                     SoundEffect.MasterVolume = 0.0f;
        //             }
        //             base.Update(gameTime);
        //         }
        // 
        //         protected override void Draw(GameTime gameTime)
        //         {
        //             GraphicsDevice.Clear(Color.CornflowerBlue);
        // 
        //             base.Draw(gameTime);
        //         }
    }
}