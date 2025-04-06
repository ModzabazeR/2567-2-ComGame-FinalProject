using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace FinalProject.Utils.BGMManager
{
    public class BGMManager
    {
        private static BGMManager instance;
        private Dictionary<string, Song> songs;
        private ContentManager content;

        public static BGMManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new BGMManager();
                return instance;
            }
        }

        private BGMManager()
        {
            songs = new Dictionary<string, Song>();
        }

        public void Initialize(ContentManager content)
        {
            this.content = content;
        }

        public void LoadSong(string songName)
        {
            if (!songs.ContainsKey(songName))
            {
                Console.WriteLine($"Loading song: {songName}");
                songs[songName] = content.Load<Song>(songName);
            }
        }

        public void PlaySong(string songName, bool isRepeating = true)
        {
            if (songs.ContainsKey(songName))
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = isRepeating;
                MediaPlayer.Play(songs[songName]);
            }
            else 
            {
                Console.WriteLine($"Song not found: {songName}");
            }
        }

        public void PlayMainTheme1(){
            StopSong();
            LoadSong("BGM/main theme1");
            PlaySong("BGM/main theme1", true);
            //Lake Jupiter - John Patitucci
            SetVolume(0.8f);
        }

        public void PlayMainTheme2(){
            StopSong();
            LoadSong("BGM/main theme 2 - Frightmare - Jimena Contreras");
            PlaySong("BGM/main theme 2 - Frightmare - Jimena Contreras", true);
            SetVolume(0.5f);
            
        }

        public void PlayBossTheme(){
            LoadSong("BGM/boss theme - Deep Space Sector 9 - Ezra Lipp");
            PlaySong("BGM/boss theme - Deep Space Sector 9 - Ezra Lipp", true);
            SetVolume(0.5f);

        }

        public void PlayWinningTheme(){
            LoadSong("BGM/[winning theme]Cosmic nightmare and praise");
            PlaySong("BGM/[winning theme]Cosmic nightmare and praise", true);
            SetVolume(0.5f);
        }
//         I am the queue's chosen servant. I am not gay. Why are you gae, player? You are gae, Player. Why can't you talk to me?
// I need more bullets! I need more bullet!
// Take my nut!
// How good is my new arm?
// Oh mommy queue, I'm hurt! Mama help me!
// Dedi? Dedi am koming uhhhhh

        public void StopSong()
        {
            MediaPlayer.Stop();
        }

        public void SetVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        public void test()
        {
            Console.WriteLine("BGMManager test method called.");
        }
    }
}
