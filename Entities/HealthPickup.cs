using FirstDesktopApp.Systems;
using System.Collections.Generic;
using System.Drawing;

namespace GameFrameWork
{
    public class HealthPickup : PowerUp
    {
        public int HealAmount { get; set; } = 25;
        private AnimationSystem animation;

        public HealthPickup(PointF position)
        {
            Position = position;
            Size = new SizeF(40, 40);
            Tag = "HealthPickup";
            
            var frames = ResourceLoader.LoadFrames("PowerUps/heart");
            if (frames.Count > 0)
                animation = new AnimationSystem(frames, 0.1f);
        }

        public override void Update(GameTime gameTime)
        {
            animation?.Update(gameTime);
        }

        public override void Draw(Graphics g)
        {
            if (!IsActive) return;

            Image img = animation?.GetCurrentFrame();
            
            if (img != null)
                g.DrawImage(img, Position.X, Position.Y, Size.Width, Size.Height);
            else
                DrawFallback(g);
        }

        private void DrawFallback(Graphics g)
        {
            // simple cross shape
            g.FillRectangle(Brushes.Green, Position.X + 15, Position.Y, 10, 40);
            g.FillRectangle(Brushes.Green, Position.X, Position.Y + 15, 40, 10);
        }
    }
}
