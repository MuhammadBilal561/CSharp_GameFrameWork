using FirstDesktopApp.Systems;
using System.Drawing;

namespace GameFrameWork
{
    public class PlayerBullet : Bullet
    {
        public int Damage { get; set; } = 10;
        public FacingDirection Direction { get; set; }
        private Image bulletSprite;

        public PlayerBullet(PointF position, FacingDirection direction)
        {
            Position = position;
            Direction = direction;
            Size = new SizeF(30, 30);
            Tag = "PlayerBullet";
            HasPhysics = false;
            
            bulletSprite = ResourceLoader.PlayerBulletSprite;

            float speed = 12f;
            
            if (direction == FacingDirection.Right)
                Velocity = new PointF(speed, 0);
            else if (direction == FacingDirection.Left)
                Velocity = new PointF(-speed, 0);
            else if (direction == FacingDirection.Up)
                Velocity = new PointF(0, -speed);
        }

        public override void Update(GameTime gameTime)
        {
            Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);

            if (Position.X > 2000 || Position.X < -50 || Position.Y < -50 || Position.Y > 1200)
                IsActive = false;
        }

        public override void Draw(Graphics g)
        {
            if (bulletSprite != null)
                g.DrawImage(bulletSprite, Position.X, Position.Y, Size.Width, Size.Height);
            else
                g.FillEllipse(Brushes.Orange, Position.X, Position.Y, Size.Width, Size.Height);
        }

        public override void OnCollision(GameObject other)
        {
            if (other is ShootingEnemy enemy && !enemy.IsDead)
                IsActive = false;
            else if (other.Tag == "Ground")
                IsActive = false;
        }
    }
}
