using FirstDesktopApp.Core;
using FirstDesktopApp.Systems;
using System.Drawing;

namespace FirstDesktopApp.Levels
{
    public class Level3Form : ShootingGameFormBase
    {
        public override int LevelNumber => 3;
        protected override Image BackgroundImage => ResourceLoader.Level3Background;

        public Level3Form(int previousScore = 0) : base(previousScore)
        {
            Text = "Level 3 - Final Boss";
        }

        protected override void CreatePlatforms()
        {
            AddGround();
            AddPlatform(400, 480, 450);
            AddPlatform(1000, 580, 400);
            AddPlatform(1500, 520, 350);
        }

        protected override void SetupEnemies()
        {
            totalEnemies = 15;
            enemiesRemaining = 15;
            spawnDelay = 0.5f;

            int humanY = GetHumanSpawnY();
            int screenW = ClientSize.Width;

            // first boss wave
            spawnQueue.Enqueue(() => SpawnBoss(400, 150));
            spawnQueue.Enqueue(() => SpawnShipEnemy(300, 100));
            spawnQueue.Enqueue(() => SpawnDrone(600, 130));
            spawnQueue.Enqueue(() => SpawnShipEnemy(900, 110));

            spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));
            spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));

            spawnQueue.Enqueue(() => SpawnDrone(500, 140));
            spawnQueue.Enqueue(() => SpawnShipEnemy(800, 120));

            // second boss wave
            spawnQueue.Enqueue(() => SpawnBoss(1000, 150));
            spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));
            spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));
            spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));
            spawnQueue.Enqueue(() => SpawnHumanEnemy(screenW + 50, humanY));

            spawnQueue.Enqueue(() => SpawnShipEnemy(700, 100));
            spawnQueue.Enqueue(() => SpawnShipEnemy(1100, 130));
        }

        protected override void OnLevelComplete()
        {
            ShowVictory();
        }
    }
}
