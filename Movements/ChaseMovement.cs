using GameFrameWork;
using System;
using System.Drawing;

namespace FirstDesktopApp.Movements
{
    public class ChaseMovement : IMovement
    {
        public float Speed { get; set; }
        public float MinX { get; set; }
        public GameObject Target { get; set; }

        public ChaseMovement(float speed, float minX, GameObject target)
        {
            Speed = speed;
            MinX = minX;
            Target = target;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            if (Target == null || !Target.IsActive)
            {
                obj.Velocity = new PointF(0, obj.Velocity.Y);
                return;
            }

            float targetX = Target.Position.X;
            float currentX = obj.Position.X;
            float diff = targetX - currentX;

            float moveAmount = Speed * 0.5f;

            // only move if far enough from target
            if (Math.Abs(diff) > 50)
            {
                if (diff < 0)
                    obj.Position = new PointF(currentX - moveAmount, obj.Position.Y);
                else
                    obj.Position = new PointF(currentX + moveAmount, obj.Position.Y);

                obj.Velocity = new PointF(diff < 0 ? -moveAmount : moveAmount, obj.Velocity.Y);
            }
            else
            {
                obj.Velocity = new PointF(0, obj.Velocity.Y);
            }

            if (obj.Position.X < MinX)
                obj.Position = new PointF(MinX, obj.Position.Y);
        }
    }
}
