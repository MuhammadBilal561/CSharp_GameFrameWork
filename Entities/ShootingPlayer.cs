using EZInput;
using FirstDesktopApp.Systems;
using GameFramework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameFrameWork
{
    public enum FacingDirection { Right, Left, Up }

    public class ShootingPlayer : Player
    {
        public const int EntityWidth = 180;
        public const int EntityHeight = 180;
        
        public FacingDirection Facing { get; set; } = FacingDirection.Right;
        public float ShootCooldown { get; set; } = 0.2f;
        
        private float shootTimer = 0;
        private float invincibilityTimer = 0;
        private const float InvincibilityDuration = 1.5f;

        private List<Image> idleFramesRight = new List<Image>();
        private List<Image> idleFramesLeft = new List<Image>();
        private List<Image> runFramesRight = new List<Image>();
        private List<Image> runFramesLeft = new List<Image>();
        private List<Image> shootFramesRight = new List<Image>();
        private List<Image> shootFramesLeft = new List<Image>();
        private List<Image> deathFrames = new List<Image>();

        private bool isShooting = false;
        private float shootAnimTimer = 0;

        public bool IsDead { get; private set; } = false;
        public bool IsInvincible => invincibilityTimer > 0;
        private float deathTimer = 0;
        private int deathFrameIndex = 0;

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public event Action<PointF, FacingDirection> OnShoot;

        public ShootingPlayer(int screenWidth, int screenHeight)
        {
            Tag = "Player";
            Health = 100;
            Size = new SizeF(EntityWidth, EntityHeight);
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            LoadSprites();
        }

        private void LoadSprites()
        {
            idleFramesRight = ResourceLoader.LoadFrames("NewPLayer/Idle/sprites");
            idleFramesLeft = ResourceLoader.FlipFrames(idleFramesRight);
            runFramesRight = ResourceLoader.LoadFrames("NewPLayer/run");
            runFramesLeft = ResourceLoader.FlipFrames(runFramesRight);
            shootFramesRight = ResourceLoader.LoadFrames("NewPLayer/shoot");
            shootFramesLeft = ResourceLoader.FlipFrames(shootFramesRight);
            deathFrames = ResourceLoader.LoadFrames("NewPLayer/death");

            if (idleFramesRight.Count > 0)
                Animation = new AnimationSystem(idleFramesRight, 0.1f);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDead)
            {
                deathTimer += gameTime.DeltaTime;
                if (deathFrames.Count > 0)
                {
                    int frameIndex = (int)(deathTimer / 0.15f);
                    deathFrameIndex = frameIndex < deathFrames.Count ? frameIndex : deathFrames.Count - 1;
                }
                return;
            }

            shootTimer -= gameTime.DeltaTime;
            invincibilityTimer -= gameTime.DeltaTime;

            if (isShooting)
            {
                shootAnimTimer -= gameTime.DeltaTime;
                if (shootAnimTimer <= 0)
                    isShooting = false;
            }

            UpdateFacing();
            UpdateShooting();
            UpdateAnimation();
            
            base.Update(gameTime);
            EnforceBounds();
        }

        private void UpdateFacing()
        {
            if (Keyboard.IsKeyPressed(Key.RightArrow) || Keyboard.IsKeyPressed(Key.D))
                Facing = FacingDirection.Right;
            else if (Keyboard.IsKeyPressed(Key.LeftArrow) || Keyboard.IsKeyPressed(Key.A))
                Facing = FacingDirection.Left;
        }

        private void UpdateShooting()
        {
            bool shootHorizontal = Keyboard.IsKeyPressed(Key.X) || Keyboard.IsKeyPressed(Key.Shift);
            bool shootUp = Keyboard.IsKeyPressed(Key.Q);
            
            if ((shootHorizontal || shootUp) && shootTimer <= 0)
            {
                if (shootUp)
                    ShootUp();
                else
                    Shoot();
                    
                shootTimer = ShootCooldown;
                isShooting = true;
                shootAnimTimer = 0.25f;
            }
        }

        private void UpdateAnimation()
        {
            if (Animation == null) return;

            bool isMoving = Math.Abs(Velocity.X) > 0.1f;

            if (isShooting && shootFramesRight.Count > 0)
            {
                Animation.SetFrames(Facing == FacingDirection.Right ? shootFramesRight : shootFramesLeft);
                return;
            }

            if (isMoving)
                Animation.SetFrames(Velocity.X > 0 ? runFramesRight : runFramesLeft);
            else
                Animation.SetFrames(Facing == FacingDirection.Right ? idleFramesRight : idleFramesLeft);
        }

        private void EnforceBounds()
        {
            float x = Math.Max(0, Math.Min(Position.X, ScreenWidth - Size.Width));
            float y = Math.Max(0, Position.Y);
            float groundY = ScreenHeight - 120 - Size.Height;
            y = Math.Min(y, groundY);
            Position = new PointF(x, y);
        }

        private void Shoot()
        {
            SoundManager.PlaySound("Shoot", 0.5f);

            float bulletY = Position.Y + Size.Height / 2 - 12;
            float bulletX = Facing == FacingDirection.Right 
                ? Position.X + Size.Width + 5 
                : Position.X - 35;

            OnShoot?.Invoke(new PointF(bulletX, bulletY), Facing);
        }

        private void ShootUp()
        {
            SoundManager.PlaySound("Shoot", 0.5f);
            float bulletX = Position.X + Size.Width / 2 - 12;
            float bulletY = Position.Y - 30;
            OnShoot?.Invoke(new PointF(bulletX, bulletY), FacingDirection.Up);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead || IsInvincible) return;

            Health -= damage;
            GameStateManager.PlayerHealth = Health;
            invincibilityTimer = InvincibilityDuration;

            if (Health <= 0)
            {
                Health = 0;
                IsDead = true;
                Velocity = PointF.Empty;
                SoundManager.PlaySound("EnemyDie", 1.0f);
            }
        }

        public void Heal(int amount)
        {
            Health = Math.Min(100, Health + amount);
            GameStateManager.PlayerHealth = Health;
        }

        public override void OnCollision(GameObject other)
        {
            if (IsDead || IsInvincible) return;

            if (other is ShootingEnemy enemy && !enemy.IsDead)
            {
                TakeDamage(15);
                return;
            }

            if (other is EnemyBullet bullet)
            {
                TakeDamage(bullet.Damage);
                bullet.IsActive = false;
                return;
            }

            if (other is HealthPickup pickup)
            {
                Heal(25);
                pickup.IsActive = false;
                SoundManager.PlaySound("Pickup", 0.7f);
            }
        }

        public override void Draw(Graphics g)
        {
            if (!IsActive) return;

            if (IsDead)
            {
                if (deathFrames.Count > 0 && deathFrameIndex < deathFrames.Count)
                    g.DrawImage(deathFrames[deathFrameIndex], Position.X, Position.Y, Size.Width, Size.Height);
                return;
            }

            // blink effect when invincible
            if (IsInvincible && ((int)(invincibilityTimer * 8) % 2 == 0))
                return;

            Image imageToDraw = Animation?.GetCurrentFrame() ?? Sprite;

            if (imageToDraw != null)
                g.DrawImage(imageToDraw, Position.X, Position.Y, Size.Width, Size.Height);
            else
                g.FillRectangle(Brushes.Blue, Bounds);
        }
    }
}
