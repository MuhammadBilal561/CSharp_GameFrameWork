using FirstDesktopApp.Core;
using FirstDesktopApp.Systems;
using System.Drawing;

namespace FirstDesktopApp.Levels
{
    public class Level1Form : ShootingGameFormBase
    {
        public override int LevelNumber => 1;
        protected override Image BackgroundImage => ResourceLoader.Level1Background;

        public Level1Form(int previousScore = 0) : base(previousScore)
        {
            Text = "Level 1 - Training Ground";
        }

        protected override void CreatePlatforms()
        {
            AddGround();
            AddPlatform(600, 550, 400);
            AddPlatform(1200, 650, 350);
        }

        protected override void SetupEnemies()
        {
            totalEnemies = 9;
            enemiesRemaining = 9;
            spawnDelay = 0.6f;

            int humanY = GetHumanSpawnY();
            int screenW = ClientSize.Width;

            for (int i = 0; i < 4; i++)
            {
                spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));

                int droneX = 200 + (i * 300);
                int droneY = 100 + (i * 30);
                int capturedX = droneX;
                int capturedY = droneY;
                spawnQueue.Enqueue(() => SpawnDrone(capturedX, capturedY));
            }
            
            spawnQueue.Enqueue(() => SpawnDrone(600, 150));
        }

        protected override void OnLevelComplete()
        {
            GameDataManager.UnlockLevel(2);
            GoToNextLevel(new Level2Form(GameStateManager.CurrentScore));
        }
    }
}
