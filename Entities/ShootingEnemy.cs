using FirstDesktopApp.Systems;
using GameFramework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameFrameWork
{
    public enum EnemyType { Basic, Drone, Boss, Human, Ship }

    public class ShootingEnemy : Enemy
    {
        public const int HumanWidth = 180;
        public const int HumanHeight = 180;
        public const int ShipSize = 100;
        public const int DroneSize = 120;
        public const int BossSize = 220;
        
        public EnemyType Type { get; set; } = EnemyType.Basic;
        public int Health { get; set; } = 30;
        public int MaxHealth { get; set; } = 30;
        public int ScoreValue { get; set; } = 100;
        public bool IsDead { get; private set; } = false;
        public bool FacingRight { get; set; } = false;

        public bool CanShoot { get; set; } = true;
        public float ShootCooldown { get; set; } = 3.0f;
        public float AttackRange { get; set; } = 400f;
        
        private float shootTimer = 0;
        private float deathTimer = 0;

        private List<Image> walkFrames = new List<Image>();
        private List<Image> shootFrames = new List<Image>();
        private List<Image> deathFrames = new List<Image>();
        private bool isShooting = false;
        private float shootAnimTimer = 0;
        private bool hasEnteredScreen = false;
        private int deathFrameIndex = 0;

        public ShootingPlayer TargetPlayer { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        private Image bossSprite = null;

        public event Action<ShootingEnemy> OnDeath;
        public event Action<PointF, PointF> OnShootBullet;

        public ShootingEnemy()
        {
            Tag = "Enemy";
            Size = new SizeF(HumanWidth, HumanHeight);
        }

        public void SetBossSprite(Image sprite) => bossSprite = sprite;
        public void LoadWalkFrames(List<Image> frames)
        {
            walkFrames = frames;
            if (frames != null && frames.Count > 0)
                Animation = new AnimationSystem(frames, 0.1f);
        }
        public void LoadShootFrames(List<Image> frames) => shootFrames = frames;
        public void LoadDeathFrames(List<Image> frames) => deathFrames = frames;

        public override void Update(GameTime gameTime)
        {
            if (IsDead)
            {
                deathTimer += gameTime.DeltaTime;
                if (deathFrames.Count > 0)
                {
                    int frameIndex = (int)(deathTimer / 0.12f);
                    deathFrameIndex = frameIndex < deathFrames.Count ? frameIndex : deathFrames.Count - 1;
                }
                if (deathTimer >= 1.0f)
                    IsActive = false;
                return;
            }

            if (!hasEnteredScreen && Position.X < ScreenWidth - Size.Width)
                hasEnteredScreen = true;

            shootTimer -= gameTime.DeltaTime;

            if (isShooting)
            {
                shootAnimTimer -= gameTime.DeltaTime;
                if (shootAnimTimer <= 0)
                    isShooting = false;
            }

            UpdateFacing();
            
            // ground enemies only shoot after entering screen
            if (hasEnteredScreen && (Type == EnemyType.Basic || Type == EnemyType.Human))
                TryShoot();
            else if (Type != EnemyType.Basic && Type != EnemyType.Human)
                TryShoot();
            
            UpdateAnimation();
            Animation?.Update(gameTime);
            base.Update(gameTime);
            EnforceBounds();
        }

        private void UpdateFacing()
        {
            if (TargetPlayer != null && !TargetPlayer.IsDead)
                FacingRight = TargetPlayer.Position.X > Position.X;
            else if (Velocity.X > 0.1f)
                FacingRight = true;
            else if (Velocity.X < -0.1f)
                FacingRight = false;
        }

        private void UpdateAnimation()
        {
            if (Animation == null) return;

            bool isMoving = Math.Abs(Velocity.X) > 0.1f;

            if (Type == EnemyType.Basic || Type == EnemyType.Human)
            {
                if (isShooting && shootFrames != null && shootFrames.Count > 0)
                {
                    Animation.SetFrames(shootFrames);
                    return;
                }
                
                if (isMoving && walkFrames != null && walkFrames.Count > 0)
                {
                    Animation.SetFrames(walkFrames);
                    return;
                }
            }

            if (walkFrames != null && walkFrames.Count > 0)
                Animation.SetFrames(walkFrames);
        }

        private void TryShoot()
        {
            if (!CanShoot || TargetPlayer == null || TargetPlayer.IsDead) return;

            float dist = Math.Abs(TargetPlayer.Position.X - Position.X);
            if (dist <= AttackRange && shootTimer <= 0)
            {
                Shoot();
                shootTimer = ShootCooldown;
                isShooting = true;
                shootAnimTimer = 0.5f;
            }
        }

        private void Shoot()
        {
            if (TargetPlayer == null) return;

            float bx = FacingRight ? Position.X + Size.Width : Position.X;
            float by = Position.Y + Size.Height / 2;

            // calculate direction toward player
            float dx = TargetPlayer.Position.X + TargetPlayer.Size.Width / 2 - bx;
            float dy = TargetPlayer.Position.Y + TargetPlayer.Size.Height / 2 - by;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);

            if (len > 0)
            {
                float spd = Type == EnemyType.Boss ? 8f : 5f;
                OnShootBullet?.Invoke(new PointF(bx, by), new PointF(dx / len * spd, dy / len * spd));
                SoundManager.PlaySound("Shoot", 0.3f);
            }
        }

        private void EnforceBounds()
        {
            if (ScreenWidth <= 0 || ScreenHeight <= 0) return;

            float x = Math.Max(0, Math.Min(Position.X, ScreenWidth - Size.Width));
            float y = Math.Max(0, Position.Y);
            
            if (Type == EnemyType.Basic || Type == EnemyType.Human)
            {
                float groundY = ScreenHeight - 120 - Size.Height;
                y = Math.Min(y, groundY);
            }
            else
            {
                y = Math.Min(y, ScreenHeight - Size.Height - 100);
            }
            
            Position = new PointF(x, y);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            Health -= damage;
            if (Health <= 0)
            {
                IsDead = true;
                Velocity = PointF.Empty;
                SoundManager.PlaySound("EnemyDie", 1.0f);
                OnDeath?.Invoke(this);
            }
        }

        public override void OnCollision(GameObject other)
        {
            if (other is PlayerBullet bullet)
            {
                TakeDamage(bullet.Damage);
                bullet.IsActive = false;
            }
        }

        public override void Draw(Graphics g)
        {
            if (!IsActive) return;

            if (IsDead)
            {
                if ((Type == EnemyType.Basic || Type == EnemyType.Human) && deathFrames.Count > 0 && deathFrameIndex < deathFrames.Count)
                {
                    var img = deathFrames[deathFrameIndex];
                    if (FacingRight)
                        g.DrawImage(img, Position.X, Position.Y, Size.Width, Size.Height);
                    else
                        g.DrawImage(img, Position.X + Size.Width, Position.Y, -Size.Width, Size.Height);
                }
                return;
            }

            Image img2 = Type == EnemyType.Boss && bossSprite != null ? bossSprite : Animation?.GetCurrentFrame() ?? Sprite;

            if (img2 != null)
            {
                if (Type == EnemyType.Ship)
                {
                    // rotate ship to face downward
                    var bmp = new Bitmap(img2);
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    g.DrawImage(bmp, Position.X, Position.Y, Size.Width, Size.Height);
                    bmp.Dispose();
                }
                else if (Type == EnemyType.Drone || Type == EnemyType.Boss)
                {
                    g.DrawImage(img2, Position.X, Position.Y, Size.Width, Size.Height);
                }
                else if (FacingRight)
                {
                    g.DrawImage(img2, Position.X, Position.Y, Size.Width, Size.Height);
                }
                else
                {
                    // flip horizontally for left facing
                    g.DrawImage(img2, Position.X + Size.Width, Position.Y, -Size.Width, Size.Height);
                }
            }
            else
            {
                g.FillRectangle(Brushes.Red, Bounds);
            }

            if (!IsDead)
                DrawHealthBar(g);
        }

        private void DrawHealthBar(Graphics g)
        {
            bool showHealthBar = Type == EnemyType.Boss || Health < MaxHealth;
            if (!showHealthBar) return;

            float pct = (float)Health / MaxHealth;
            float barWidth = Size.Width;
            float barHeight = 14;
            
            g.FillRectangle(Brushes.DarkGray, Position.X, Position.Y - 20, barWidth, barHeight);

            Color col = pct <= 0.25f ? Color.Red : pct <= 0.5f ? Color.Yellow : Color.LimeGreen;

            using (var brush = new SolidBrush(col))
                g.FillRectangle(brush, Position.X, Position.Y - 20, barWidth * pct, barHeight);
            
            g.DrawRectangle(Pens.Black, Position.X, Position.Y - 20, barWidth, barHeight);
        }
    }
}
