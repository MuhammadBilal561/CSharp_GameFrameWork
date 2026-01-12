using GameFrameWork;
using System;

namespace FirstDesktopApp.Movements
{
    internal class ZigZagMovement : IMovement
    {
        float xSpeed = 2f;
        float ySpeed = 1.75f;
        int leftBound;
        int rightBound;
        int topBound;
        int bottomBound;

        public ZigZagMovement(int leftBound, int rightBound, int topBound, int bottomBound)
        {
            this.leftBound = leftBound;
            this.rightBound = rightBound;
            this.topBound = topBound;
            this.bottomBound = bottomBound;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            float xMoveAmount = xSpeed * gameTime.DeltaTime;
            float yMoveAmount = ySpeed * gameTime.DeltaTime;
            
            obj.Position = new PointF(obj.Position.X + xMoveAmount, obj.Position.Y + yMoveAmount);
            
            // bounce off boundaries
            if (obj.Position.Y > bottomBound)
            {
                obj.Position = new PointF(obj.Position.X, bottomBound);
                ySpeed = -Math.Abs(ySpeed);
            }
            
            if (obj.Position.Y < topBound)
            {
                obj.Position = new PointF(obj.Position.X, topBound);
                ySpeed = Math.Abs(ySpeed);
            }
            
            if (obj.Position.X < leftBound)
            {
                obj.Position = new PointF(leftBound, obj.Position.Y);
                xSpeed = Math.Abs(xSpeed);
            }
            else if (obj.Position.X > rightBound)
            {
                obj.Position = new PointF(rightBound, obj.Position.Y);
                xSpeed = -Math.Abs(xSpeed);
            }
        }
    }
}
