using GameFrameWork;
using System;

namespace FirstDesktopApp.Movements
{
    public class DroneMovement : IMovement
    {
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float Speed { get; set; }
        public float Amplitude { get; set; }
        public float Frequency { get; set; }
        
        private float time = 0;
        private float baseY;
        private bool initialized = false;
        private bool movingRight = true;

        public DroneMovement(float minX, float maxX, float speed, float amplitude = 30f, float frequency = 2f)
        {
            MinX = Math.Max(0, minX);
            MaxX = maxX;
            Speed = speed;
            Amplitude = amplitude;
            Frequency = frequency;
        }

        public void Move(GameObject obj, GameTime gameTime)
        {
            if (!initialized)
            {
                baseY = Math.Max(50, obj.Position.Y);
                initialized = true;
            }

            time += gameTime.DeltaTime;
            
            float moveAmount = Speed * 0.4f;
            float newX = obj.Position.X;

            if (movingRight)
            {
                newX += moveAmount;
                obj.Velocity = new PointF(moveAmount, 0);
                if (newX >= MaxX) movingRight = false;
            }
            else
            {
                newX -= moveAmount;
                obj.Velocity = new PointF(-moveAmount, 0);
                if (newX <= MinX) movingRight = true;
            }

            if (newX < 0) newX = 0;
            
            // sine wave for vertical bobbing
            float newY = baseY + (float)Math.Sin(time * Frequency * 0.5f) * Amplitude;
            if (newY < 20) newY = 20;

            obj.Position = new PointF(newX, newY);
        }
    }
}
