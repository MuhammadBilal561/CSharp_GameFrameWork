using FirstDesktopApp.Systems;
using GameFrameWork;
using System.Drawing;

namespace FirstDesktopApp.Entities
{
    public class Platform : GameObject
    {
        public bool IsVisible { get; set; } = true;
        public bool IsGround { get; set; } = false;
        
        private static Image platformSprite;
        
        public Platform(int x, int y)
        {
            Position = new PointF(x, y);
            Tag = "Ground";
            IsRigidBody = true;
            Size = new Size(50, 50);
            
            if (platformSprite == null)
                platformSprite = ResourceLoader.PlatformSprite;
        }

        public override void Draw(Graphics g)
        {
            if (!IsVisible) return;
            
            if (IsGround)
            {
                g.FillRectangle(Brushes.SaddleBrown, Bounds);
                g.DrawRectangle(Pens.DarkGray, Position.X, Position.Y, Size.Width, Size.Height);
            }
            else if (platformSprite != null)
            {
                g.DrawImage(platformSprite, Position.X, Position.Y, Size.Width, Size.Height);
            }
            else
            {
                g.FillRectangle(Brushes.SaddleBrown, Bounds);
            }
        }
    }
}
