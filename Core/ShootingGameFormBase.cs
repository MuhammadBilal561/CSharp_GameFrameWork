using EZInput;
using FirstDesktopApp.Entities;
using FirstDesktopApp.Movements;
using FirstDesktopApp.Movements.FirstDesktopApp.Movements;
using FirstDesktopApp.Systems;
using FirstDesktopApp.UI;
using GameFramework;
using GameFrameWork;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FirstDesktopApp.Core
{
    public abstract class ShootingGameFormBase : Form
    {
        protected Game game = new Game();
        protected PhysicsSystem physics = new PhysicsSystem();
        protected CollisionSystem collisions = new CollisionSystem();
        protected System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        protected UIRenderer uiRenderer = new UIRenderer();

        protected ShootingPlayer player;
        protected ScrollingBackground scrollingBg;

        protected Queue<Action> spawnQueue = new Queue<Action>();
        protected float spawnTimer = 0;
        protected float spawnDelay = 0.6f;
        protected int enemiesRemaining = 0;
        protected int totalEnemies = 0;
        protected bool levelCompleteTriggered = false;

        protected List<Image> humanWalkFrames;
        protected List<Image> humanShootFrames;
        protected List<Image> humanDeathFrames;
        protected List<Image> shipFrames;
        protected List<Image> droneFrames;

        protected bool escPressed = false;
        protected bool isPaused = false;
        
        private Random random = new Random();

        public abstract int LevelNumber { get; }
        protected abstract new Image BackgroundImage { get; }
        protected abstract void SetupEnemies();
        protected abstract void CreatePlatforms();
        protected abstract void OnLevelComplete();

        protected ShootingGameFormBase(int previousScore = 0)
        {
            SetupForm();
            LoadSprites();
            GameStateManager.CurrentScore = previousScore;
            GameStateManager.CurrentLevel = LevelNumber;
        }

        private void SetupForm()
        {
            Text = "Space Defender - Level " + LevelNumber;
            ClientSize = new Size(1920, 1080);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;
            KeyUp += OnKeyUp;
        }

        private void LoadSprites()
        {
            humanWalkFrames = ResourceLoader.LoadFrames("Enemy/HumanEnemy/walk");
            humanShootFrames = ResourceLoader.LoadFrames("Enemy/HumanEnemy/shoot");
            humanDeathFrames = ResourceLoader.LoadFrames("Enemy/HumanEnemy/death");
            shipFrames = ResourceLoader.LoadFrames("Enemy/Ship ENemy/ShipandThrust");
            droneFrames = ResourceLoader.LoadFrames("Enemy/DroneEnemy");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            scrollingBg = new ScrollingBackground(ClientSize.Width, ClientSize.Height);
            scrollingBg.SetBackground(BackgroundImage);

            GameStateManager.ResetForLevel();
            
            CreatePlayer();
            game.ProcessPendingObjects();
            
            CreatePlatforms();
            game.ProcessPendingObjects();
            
            SetupEnemies();
            game.ProcessPendingObjects();

            SoundManager.StopMusic();
            SoundManager.PlayMusic("LevelMusic", GameDataManager.CurrentData.MusicVolume);

            gameTimer.Start();
        }

        protected virtual void CreatePlayer()
        {
            int groundY = ClientSize.Height - 120;
            int playerY = groundY - ShootingPlayer.EntityHeight;

            player = new ShootingPlayer(ClientSize.Width, ClientSize.Height)
            {
                Position = new PointF(100, playerY),
                HasPhysics = true,
                Movement = new ShootingPlayerMovement()
            };

            player.OnShoot += SpawnPlayerBullet;
            game.AddObject(player);
        }

        protected void AddGround()
        {
            var ground = new Platform(0, ClientSize.Height - 60);
            ground.Size = new Size(ClientSize.Width, 60);
            ground.IsGround = true;
            game.AddObject(ground);
        }

        protected void AddPlatform(int x, int y, int width)
        {
            var plat = new Platform(x, y);
            plat.Size = new Size(width, 40);
            game.AddObject(plat);
        }

        private void SpawnPlayerBullet(PointF position, FacingDirection direction)
        {
            var bullet = new PlayerBullet(position, direction);
            game.AddObject(bullet);
        }

        protected void SpawnEnemyBullet(PointF position, PointF velocity)
        {
            var bullet = new EnemyBullet(position, velocity);
            game.AddObject(bullet);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            HandleInput();

            if (!isPaused)
                UpdateGame();

            Invalidate();
        }

        private void UpdateGame()
        {
            var gameTime = new GameTime { DeltaTime = 0.016f };

            if (spawnQueue.Count > 0)
            {
                spawnTimer += 0.016f;
                if (spawnTimer >= spawnDelay)
                {
                    spawnTimer = 0;
                    spawnQueue.Dequeue()();
                }
            }

            game.Update(gameTime);
            physics.Apply(game.Objects);
            collisions.Check(game.Objects);
            game.Cleanup();

            if (player != null)
            {
                scrollingBg?.Update(player.Velocity.X);

                if (player.IsDead)
                {
                    gameTimer.Stop();
                    ShowGameOver();
                    return;
                }
            }

            if (enemiesRemaining <= 0 && spawnQueue.Count == 0 && !levelCompleteTriggered)
            {
                levelCompleteTriggered = true;
                gameTimer.Stop();
                OnLevelComplete();
            }
        }

        private void HandleInput()
        {
            if (Keyboard.IsKeyPressed(Key.Escape) && !escPressed)
            {
                escPressed = true;
                isPaused = !isPaused;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                escPressed = false;
        }

        protected void GoToNextLevel(Form nextForm)
        {
            GameDataManager.UnlockLevel(LevelNumber + 1);
            gameTimer.Stop();
            Hide();
            nextForm.Show();
        }

        protected void ShowVictory()
        {
            GameDataManager.UpdateHighScore(GameStateManager.CurrentScore);
            MessageBox.Show("Victory! Final Score: " + GameStateManager.CurrentScore, "You Win!");
            ReturnToMenu();
        }

        protected void ShowGameOver()
        {
            MessageBox.Show("Game Over! Score: " + GameStateManager.CurrentScore, "Game Over");
            ReturnToMenu();
        }

        protected void ReturnToMenu()
        {
            gameTimer.Stop();
            SoundManager.StopMusic();
            SoundManager.PlayMusic("MenuMusic", GameDataManager.CurrentData.MusicVolume);
            
            var menu = new MainMenuForm();
            Hide();
            menu.Show();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            scrollingBg?.Draw(g);
            game.Draw(g);
            uiRenderer.DrawGameUI(g, player?.Health ?? 0, GameStateManager.CurrentScore, LevelNumber, enemiesRemaining);

            if (isPaused)
                uiRenderer.DrawPausedOverlay(g, ClientSize.Width, ClientSize.Height);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameTimer.Stop();
            SoundManager.StopMusic();
            GameDataManager.Save();
            base.OnFormClosing(e);
            
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        protected int GetHumanSpawnY()
        {
            return ClientSize.Height - 60 - ShootingEnemy.HumanHeight;
        }

        protected void SpawnHumanEnemy(float x, float y)
        {
            var enemy = new ShootingEnemy
            {
                Position = new PointF(x, y),
                Size = new SizeF(ShootingEnemy.HumanWidth, ShootingEnemy.HumanHeight),
                Type = EnemyType.Human,
                Health = 50,
                MaxHealth = 50,
                ScoreValue = 100,
                HasPhysics = true,
                CanShoot = true,
                ShootCooldown = 1.5f,
                AttackRange = 600f,
                TargetPlayer = player,
                ScreenWidth = ClientSize.Width,
                ScreenHeight = ClientSize.Height,
                FacingRight = false,
                Movement = new ChaseMovement(1.8f, 120, player)
            };
            
            enemy.LoadWalkFrames(humanWalkFrames);
            enemy.LoadShootFrames(humanShootFrames);
            enemy.LoadDeathFrames(humanDeathFrames);
            enemy.OnDeath += OnEnemyDeath;
            enemy.OnShootBullet += SpawnEnemyBullet;
            game.AddObject(enemy);
        }

        protected void SpawnShipEnemy(float x, float y)
        {
            var ship = new ShootingEnemy
            {
                Position = new PointF(x, y),
                Size = new SizeF(ShootingEnemy.ShipSize, ShootingEnemy.ShipSize),
                Type = EnemyType.Ship,
                Health = 40,
                MaxHealth = 40,
                ScoreValue = 150,
                HasPhysics = false,
                CanShoot = true,
                ShootCooldown = 2.0f,
                AttackRange = 500f,
                TargetPlayer = player,
                ScreenWidth = ClientSize.Width,
                ScreenHeight = ClientSize.Height,
                Movement = new DroneMovement(50, ClientSize.Width - 150, 0.8f, 30f, 1.0f)
            };
            
            ship.LoadWalkFrames(shipFrames);
            ship.OnDeath += OnEnemyDeath;
            ship.OnShootBullet += SpawnEnemyBullet;
            game.AddObject(ship);
        }

        protected void SpawnDrone(float x, float y)
        {
            var drone = new ShootingEnemy
            {
                Position = new PointF(x, y),
                Size = new SizeF(ShootingEnemy.DroneSize, ShootingEnemy.DroneSize),
                Type = EnemyType.Drone,
                Health = 30,
                MaxHealth = 30,
                ScoreValue = 150,
                HasPhysics = false,
                CanShoot = true,
                ShootCooldown = 2.0f,
                AttackRange = 500f,
                TargetPlayer = player,
                ScreenWidth = ClientSize.Width,
                ScreenHeight = ClientSize.Height,
                Movement = new DroneMovement(50, ClientSize.Width - 150, 0.8f, 30f, 1.0f)
            };
            
            drone.LoadWalkFrames(droneFrames);
            drone.OnDeath += OnEnemyDeath;
            drone.OnShootBullet += SpawnEnemyBullet;
            game.AddObject(drone);
        }

        protected void SpawnBoss(float x, float y)
        {
            var boss = new ShootingEnemy
            {
                Position = new PointF(x, y),
                Size = new SizeF(ShootingEnemy.BossSize, ShootingEnemy.BossSize),
                Type = EnemyType.Boss,
                Health = 400,
                MaxHealth = 400,
                ScoreValue = 1000,
                HasPhysics = false,
                CanShoot = true,
                ShootCooldown = 1.0f,
                AttackRange = 800f,
                TargetPlayer = player,
                ScreenWidth = ClientSize.Width,
                ScreenHeight = ClientSize.Height,
                Movement = new PatrolMovement(100, ClientSize.Width - 300, 1.0f),
                Sprite = ResourceLoader.BossSprite
            };
            
            boss.OnDeath += OnEnemyDeath;
            boss.OnShootBullet += SpawnEnemyBullet;
            game.AddObject(boss);
        }

        private void OnEnemyDeath(ShootingEnemy enemy)
        {
            enemiesRemaining--;
            GameStateManager.CurrentScore += enemy.ScoreValue;
            
            if (random.NextDouble() < 0.4)
            {
                var pickup = new HealthPickup(new PointF(enemy.Position.X + 30, enemy.Position.Y + 30));
                game.AddObject(pickup);
            }
        }
    }
}
