using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
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
                songs[songName] = content.Load<Song>(songName);
            }
        }

        public void PlaySong(string songName, bool isRepeating = true)
        {
            if (songs.ContainsKey(songName))
            {
                MediaPlayer.IsRepeating = isRepeating;
                MediaPlayer.Play(songs[songName]);
            }
        }

        public void StopSong()
        {
            MediaPlayer.Stop();
        }

        public void SetVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }
    }
}