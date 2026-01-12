using EZInput;
using GameFramework;
using System.Drawing;

namespace GameFrameWork
{
    public class ShootingPlayerMovement : IMovement
    {
        public float Speed { get; set; } = 5.0f;
        public float JumpForce { get; set; } = -14f;
        private float jumpCooldown = 0;

        public void Move(GameObject obj, GameTime gameTime)
        {
            float velocityX = 0;

            if (Keyboard.IsKeyPressed(Key.LeftArrow) || Keyboard.IsKeyPressed(Key.A))
                velocityX = -Speed;
            if (Keyboard.IsKeyPressed(Key.RightArrow) || Keyboard.IsKeyPressed(Key.D))
                velocityX = Speed;

            obj.Velocity = new PointF(velocityX, obj.Velocity.Y);

            jumpCooldown -= 0.016f;
            
            bool jumpPressed = Keyboard.IsKeyPressed(Key.Space) || Keyboard.IsKeyPressed(Key.W) || Keyboard.IsKeyPressed(Key.UpArrow);
            bool isGrounded = obj.Velocity.Y >= -0.1f && obj.Velocity.Y <= 2.0f;
            
            if (jumpPressed && isGrounded && jumpCooldown <= 0)
            {
                obj.Velocity = new PointF(obj.Velocity.X, JumpForce);
                jumpCooldown = 0.3f;
                SoundManager.PlaySound("Jump", 0.5f);
            }
        }
    }
}
