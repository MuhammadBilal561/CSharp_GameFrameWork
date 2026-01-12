using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace GameFramework
{
    public static class SoundManager
    {
        private static Dictionary<string, string> soundLibrary = new Dictionary<string, string>();
        private static MediaPlayer musicPlayer = new MediaPlayer();

        public static void LoadSound(string name, string fileName)
        {
            string fullPath = "";
            
            if (Path.IsPathRooted(fileName))
            {
                fullPath = fileName;
            }
            else
            {
                string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName);
                if (File.Exists(assetsPath))
                {
                    fullPath = assetsPath;
                }
                else
                {
                    string resourcesPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Resources", "Audio", fileName));
                    if (File.Exists(resourcesPath))
                        fullPath = resourcesPath;
                    else
                        fullPath = assetsPath;
                }
            }

            if (soundLibrary.ContainsKey(name))
                soundLibrary[name] = fullPath;
            else
                soundLibrary.Add(name, fullPath);
        }

        public static void PlayMusic(string name, float volume = 0.5f)
        {
            if (!soundLibrary.ContainsKey(name)) return;
            if (!File.Exists(soundLibrary[name])) return;

            musicPlayer.Open(new Uri(soundLibrary[name]));
            musicPlayer.Volume = volume;

            musicPlayer.MediaEnded += (sender, e) =>
            {
                musicPlayer.Position = TimeSpan.Zero;
                musicPlayer.Play();
            };

            musicPlayer.Play();
        }

        public static void PlaySound(string name, float volume = 1.0f)
        {
            if (!soundLibrary.ContainsKey(name)) return;
            if (!File.Exists(soundLibrary[name])) return;

            MediaPlayer sfx = new MediaPlayer();
            sfx.Open(new Uri(soundLibrary[name]));
            sfx.Volume = volume;
            sfx.Play();
        }

        public static void StopMusic()
        {
            musicPlayer.Stop();
        }
        
        public static void SetMusicVolume(float volume)
        {
            musicPlayer.Volume = Math.Clamp(volume, 0f, 1f);
        }
    }
}
