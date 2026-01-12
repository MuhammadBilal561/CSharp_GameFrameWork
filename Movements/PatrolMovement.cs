using GameFrameWork;
using System;
using System.Drawing;

namespace FirstDesktopApp.Movements
{
    public class PatrolMovement : IMovement
    {
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float Speed { get; set; }
        
        private bool movingRight = true;

        public PatrolMovement(float minX, float maxX, float speed)
        {
            MinX = minX;
            MaxX = maxX;
            Speed = speed;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            float moveAmount = Speed * 0.5f;

            if (movingRight)
            {
                obj.Position = new PointF(obj.Position.X + moveAmount, obj.Position.Y);
                obj.Velocity = new PointF(moveAmount, obj.Velocity.Y);
                
                if (obj.Position.X >= MaxX)
                    movingRight = false;
            }
            else
            {
                obj.Position = new PointF(obj.Position.X - moveAmount, obj.Position.Y);
                obj.Velocity = new PointF(-moveAmount, obj.Velocity.Y);
                
                if (obj.Position.X <= MinX)
                    movingRight = true;
            }
        }
    }
}
