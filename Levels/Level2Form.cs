using FirstDesktopApp.Core;
using FirstDesktopApp.Systems;
using System.Drawing;

namespace FirstDesktopApp.Levels
{
    public class Level2Form : ShootingGameFormBase
    {
        public override int LevelNumber => 2;
        protected override Image BackgroundImage => ResourceLoader.Level2Background;

        public Level2Form(int previousScore = 0) : base(previousScore)
        {
            Text = "Level 2 - Ship Assault";
        }

        protected override void CreatePlatforms()
        {
            AddGround();
            AddPlatform(300, 500, 350);
            AddPlatform(800, 600, 400);
            AddPlatform(1400, 550, 350);
        }

        protected override void SetupEnemies()
        {
            totalEnemies = 13;
            enemiesRemaining = 13;
            spawnDelay = 0.5f;

            int humanY = GetHumanSpawnY();
            int screenW = ClientSize.Width;

            for (int i = 0; i < 6; i++)
            {
                spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));

                if (i < 7)
                {
                    int shipX = 150 + (i * 200);
                    int shipY = 80 + (i * 25);
                    int capturedX = shipX;
                    int capturedY = shipY;
                    spawnQueue.Enqueue(() => SpawnShipEnemy(capturedX, capturedY));
                }
            }
            
            spawnQueue.Enqueue(() => SpawnShipEnemy(800, 120));
        }

        protected override void OnLevelComplete()
        {
            GameDataManager.UnlockLevel(3);
            GoToNextLevel(new Level3Form(GameStateManager.CurrentScore));
        }
    }
}
