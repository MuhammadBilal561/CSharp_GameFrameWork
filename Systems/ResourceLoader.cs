using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstDesktopApp.Systems
{
    public static class ResourceLoader
    {
        private static readonly string ResourcePath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Resources"));
        
        private static readonly Dictionary<string, Image> ImageCache = new Dictionary<string, Image>();
        private static readonly Dictionary<string, List<Image>> FrameCache = new Dictionary<string, List<Image>>();

        public static Image LoadImage(string relativePath)
        {
            if (ImageCache.TryGetValue(relativePath, out var cached))
                return cached;

            string fullPath = Path.Combine(ResourcePath, relativePath);
            if (File.Exists(fullPath))
            {
                var img = Image.FromFile(fullPath);
                ImageCache[relativePath] = img;
                return img;
            }
            return null;
        }

        public static List<Image> LoadFrames(string folderPath)
        {
            if (FrameCache.TryGetValue(folderPath, out var cached))
                return cached;

            var frames = new List<Image>();
            string fullPath = Path.Combine(ResourcePath, folderPath);

            if (Directory.Exists(fullPath))
            {
                var files = Directory.GetFiles(fullPath, "*.png")
                    .OrderBy(f => GetFrameNumber(f))
                    .ToList();

                foreach (var file in files)
                {
                    var img = Image.FromFile(file);
                    frames.Add(img);
                }
            }

            FrameCache[folderPath] = frames;
            return frames;
        }

        private static int GetFrameNumber(string filePath)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            
            // Try to extract number from filename (handles: 1.png, frame1.png, idle1.png, run-with-gun1.png)
            var match = Regex.Match(name, @"(\d+)");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);
            
            return 0;
        }

        public static List<Image> FlipFrames(List<Image> frames)
        {
            var flipped = new List<Image>();
            foreach (var frame in frames)
            {
                var bmp = new Bitmap(frame);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                flipped.Add(bmp);
            }
            return flipped;
        }

        public static string GetAudioPath(string relativePath)
        {
            return Path.Combine(ResourcePath, "Audio", relativePath);
        }

        // Backgrounds for each level
        public static Image Level1Background => LoadImage("Backgrounds/lvl3_bg.jpg");
        public static Image Level2Background => LoadImage("Backgrounds/level2.jpg");
        public static Image Level3Background => LoadImage("Backgrounds/level3.jpg");
        public static Image MenuBackground1 => LoadImage("Backgrounds/mainbackground.png");
        
        // Platform
        public static Image PlatformSprite => LoadImage("Environment/PLATFORM.png");
        
        // UI
        public static Image MenuBackground3 => LoadImage("UI/menubg3.png");
        public static Image Heart => LoadImage("UI/heart.png");
        
        // Boss
        public static Image BossSprite => LoadImage("Enemy/Bdrone.png");
        
        // Bullets
        public static Image PlayerBulletSprite => LoadImage("bullet/bullet1.png");
        public static Image EnemyBulletSprite => LoadImage("bullet/bullet2.png");

        public static void ClearCache()
        {
            foreach (var img in ImageCache.Values)
                img?.Dispose();
            ImageCache.Clear();

            foreach (var list in FrameCache.Values)
                foreach (var img in list)
                    img?.Dispose();
            FrameCache.Clear();
        }
    }
}
