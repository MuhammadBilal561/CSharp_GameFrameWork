using GameFrameWork;
using System;

namespace FirstDesktopApp.Movements
{
    internal class VerticalMovement : IMovement
    {
        int topBound;
        int bottomBound;
        float speed = 2f;

        public VerticalMovement(int topBound, int bottomBound)
        {
            this.topBound = topBound;
            this.bottomBound = bottomBound;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            float moveAmount = speed * gameTime.DeltaTime;
            obj.Position = new PointF(obj.Position.X, obj.Position.Y + moveAmount);

            if (obj.Position.Y > bottomBound)
            {
                obj.Position = new PointF(obj.Position.X, bottomBound);
                speed = -Math.Abs(speed);
            }
            if (obj.Position.Y < topBound)
            {
                obj.Position = new PointF(obj.Position.X, topBound);
                speed = Math.Abs(speed);
            }
        }
    }
}
