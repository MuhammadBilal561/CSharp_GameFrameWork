using FirstDesktopApp.Systems;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameFrameWork
{
    public class GameObject : IDrawable, IUpdatable, IMovable, ICollidable, IPhysicsObject
    {
        public string Tag { get; set; } = "Default";
        public PointF Position { get; set; }
        public SizeF Size { get; set; }
        public PointF Velocity { get; set; } = PointF.Empty;
        public bool IsActive { get; set; } = true;
        public bool HasPhysics { get; set; } = false;
        public float? CustomGravity { get; set; } = null;
        public bool IsRigidBody { get; set; } = false;
        public Image? Sprite { get; set; } = null;
        public RectangleF Bounds => new RectangleF(Position, Size);
        public AnimationSystem? Animation { get; set; }

        public virtual void Update(GameTime gameTime)
        {
            Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);
            Animation?.Update(gameTime);
        }

        public virtual void Draw(Graphics graphics)
        {
            Image imageToDraw = Animation?.GetCurrentFrame() ?? Sprite;
            
            if (imageToDraw != null)
                graphics.DrawImage(imageToDraw, Position.X, Position.Y, Size.Width, Size.Height);
        }

        public virtual void OnCollision(GameObject other) { }
    }
}