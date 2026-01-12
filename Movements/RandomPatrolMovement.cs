using GameFrameWork;
using System;
using System.Drawing;

namespace FirstDesktopApp.Movements
{
    public class RandomPatrolMovement : IMovement
    {
        private Random rand = new Random();
        private float speed;
        private PointF target;
        private float minX, maxX, minY, maxY;
        private float waitTimer = 0;
        private float waitDuration = 0;

        public RandomPatrolMovement(float speed, float minX, float maxX, float minY, float maxY)
        {
            this.speed = speed;
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            PickNewTarget();
        }

        private void PickNewTarget()
        {
            target = new PointF(
                minX + (float)rand.NextDouble() * (maxX - minX),
                minY + (float)rand.NextDouble() * (maxY - minY)
            );
            waitDuration = 0.5f + (float)rand.NextDouble() * 1.5f;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            if (waitTimer > 0)
            {
                waitTimer -= gameTime.DeltaTime;
                obj.Velocity = PointF.Empty;
                return;
            }

            float dx = target.X - obj.Position.X;
            float dy = target.Y - obj.Position.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            if (dist < 10)
            {
                waitTimer = waitDuration;
                PickNewTarget();
                return;
            }

            float moveX = (dx / dist) * speed * gameTime.DeltaTime;
            float moveY = (dy / dist) * speed * gameTime.DeltaTime;

            obj.Position = new PointF(obj.Position.X + moveX, obj.Position.Y + moveY);
            obj.Velocity = new PointF(moveX, moveY);
        }
    }
}
