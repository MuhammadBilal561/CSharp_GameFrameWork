using FirstDesktopApp.Systems;
using System.Drawing;

namespace GameFrameWork
{
    public class EnemyBullet : Bullet
    {
        public int Damage { get; set; } = 15;
        private Image bulletSprite;

        public EnemyBullet(PointF position, PointF velocity)
        {
            Position = position;
            Velocity = velocity;
            Size = new SizeF(20, 20);
            Tag = "EnemyBullet";
            HasPhysics = false;
            bulletSprite = ResourceLoader.EnemyBulletSprite;
        }

        public override void Update(GameTime gameTime)
        {
            Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);

            if (Position.X > 2000 || Position.X < -50 || Position.Y > 1200 || Position.Y < -50)
                IsActive = false;
        }

        public override void Draw(Graphics g)
        {
            if (bulletSprite != null)
                g.DrawImage(bulletSprite, Position.X, Position.Y, Size.Width, Size.Height);
            else
                g.FillEllipse(Brushes.OrangeRed, Position.X, Position.Y, Size.Width, Size.Height);
        }

        public override void OnCollision(GameObject other)
        {
            if (other is ShootingPlayer)
                IsActive = false;
            else if (other.Tag == "Ground")
                IsActive = false;
        }
    }
}
