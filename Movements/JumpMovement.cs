using EZInput;
using GameFrameWork;

namespace FirstDesktopApp.Movements
{
    namespace FirstDesktopApp.Movements
    {
        public class JumpMovement : IMovement
        {
            public float JumpForce { get; set; } = -12f;
            public float GroundY { get; set; } = 600f; // Ground level

            public void Move(GameObject obj, GameTime gameTime)
            {
                // Check if object is at or below ground level
                if (obj.Position.Y >= GroundY)
                {
                    // On ground - apply jump force
                    obj.Velocity = new PointF(obj.Velocity.X, JumpForce);
                    obj.Position = new PointF(obj.Position.X, GroundY); // Snap to ground
                }
            }
        }
    }
}
