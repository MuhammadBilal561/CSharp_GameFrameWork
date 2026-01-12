using System;

namespace FirstDesktopApp.Systems
{
    public static class GameStateManager
    {
        public static int CurrentLevel { get; set; } = 1;
        public static int CurrentScore { get; set; } = 0;

        private static int _playerHealth = 100;
        public static int PlayerHealth
        {
            get => _playerHealth;
            set => _playerHealth = Math.Clamp(value, 0, 100);
        }

        public static void ResetForLevel() => PlayerHealth = 100;

        public static void ResetForNewGame()
        {
            CurrentLevel = 1;
            CurrentScore = 0;
            PlayerHealth = 100;
        }
    }
}
