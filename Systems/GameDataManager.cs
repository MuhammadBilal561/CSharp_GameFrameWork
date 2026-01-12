using System;
using System.IO;

namespace FirstDesktopApp.Systems
{
    public static class GameDataManager
    {
        private static readonly string SaveDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SpaceDefender");
        
        private static readonly string SaveFile = Path.Combine(SaveDirectory, "gamedata.txt");

        public static GameSaveData CurrentData { get; private set; } = new GameSaveData();

        static GameDataManager()
        {
            Load();
        }

        public static void Save()
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            using (StreamWriter writer = new StreamWriter(SaveFile))
            {
                writer.WriteLine("HighScore=" + CurrentData.HighScore);
                writer.WriteLine("MaxLevelUnlocked=" + CurrentData.MaxLevelUnlocked);
                writer.WriteLine("MusicVolume=" + CurrentData.MusicVolume);
                writer.WriteLine("SfxVolume=" + CurrentData.SfxVolume);
                writer.WriteLine("PlayerName=" + CurrentData.PlayerName);
                writer.WriteLine("TotalEnemiesKilled=" + CurrentData.TotalEnemiesKilled);
                writer.WriteLine("TotalPlayTime=" + CurrentData.TotalPlayTime);
            }
        }

        public static void Load()
        {
            if (!File.Exists(SaveFile))
            {
                CurrentData = new GameSaveData();
                return;
            }

            string[] lines = File.ReadAllLines(SaveFile);
            
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                    
                string[] parts = line.Split('=');
                if (parts.Length != 2) continue;
                
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                
                switch (key)
                {
                    case "HighScore":
                        CurrentData.HighScore = int.Parse(value);
                        break;
                    case "MaxLevelUnlocked":
                        CurrentData.MaxLevelUnlocked = int.Parse(value);
                        break;
                    case "MusicVolume":
                        CurrentData.MusicVolume = float.Parse(value);
                        break;
                    case "SfxVolume":
                        CurrentData.SfxVolume = float.Parse(value);
                        break;
                    case "PlayerName":
                        CurrentData.PlayerName = value;
                        break;
                    case "TotalEnemiesKilled":
                        CurrentData.TotalEnemiesKilled = int.Parse(value);
                        break;
                    case "TotalPlayTime":
                        CurrentData.TotalPlayTime = int.Parse(value);
                        break;
                }
            }
        }

        public static void UpdateHighScore(int score)
        {
            if (score > CurrentData.HighScore)
            {
                CurrentData.HighScore = score;
                Save();
            }
        }

        public static void UnlockLevel(int level)
        {
            if (level > CurrentData.MaxLevelUnlocked)
            {
                CurrentData.MaxLevelUnlocked = level;
                Save();
            }
        }

        public static void ResetProgress()
        {
            CurrentData = new GameSaveData();
            Save();
        }
    }

    public class GameSaveData
    {
        public int HighScore { get; set; } = 0;
        public int MaxLevelUnlocked { get; set; } = 1;
        public float MusicVolume { get; set; } = 0.5f;
        public float SfxVolume { get; set; } = 1.0f;
        public string PlayerName { get; set; } = "Player";
        public int TotalEnemiesKilled { get; set; } = 0;
        public int TotalPlayTime { get; set; } = 0;
    }
}
