using EZInput;
using GameFramework;
using System.Drawing;

namespace GameFrameWork
{
    public class KeyboardMovement : IMovement
    {
        public float Speed { get; set; } = 5f;

        public void Move(GameObject obj, GameTime gameTime)
        {
            if (Keyboard.IsKeyPressed(Key.LeftArrow))
                obj.Position = new PointF(obj.Position.X - Speed, obj.Position.Y);

            if (Keyboard.IsKeyPressed(Key.RightArrow))
                obj.Position = new PointF(obj.Position.X + Speed, obj.Position.Y);

            if (Keyboard.IsKeyPressed(Key.UpArrow))
                obj.Position = new PointF(obj.Position.X, obj.Position.Y - Speed);

            if (Keyboard.IsKeyPressed(Key.DownArrow))
                obj.Position = new PointF(obj.Position.X, obj.Position.Y + Speed);

            // jump when grounded
            if (Keyboard.IsKeyPressed(Key.Space) && obj.Velocity.Y > -0.1 && obj.Velocity.Y < 0.1)
            {
                obj.Velocity = new PointF(obj.Velocity.X, -8);
                SoundManager.PlaySound("Jump");
            }
        }
    }
}
