using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameFrameWork
{
    public class PhysicsSystem
    {
        public float Gravity { get; set; } = 0.5f;

        public void Apply(List<GameObject> objects)
        {
            foreach (var obj in objects.OfType<IPhysicsObject>().Where(o => o.HasPhysics))
            {
                if (obj is IMovable movable)
                {
                    float appliedGravity = obj.CustomGravity ?? Gravity;

                    movable.Velocity = new PointF(
                        movable.Velocity.X,
                        movable.Velocity.Y + appliedGravity
                    );

                    if (obj is GameObject gameObject)
                    {
                        gameObject.Position = new PointF(
                            gameObject.Position.X + movable.Velocity.X,
                            gameObject.Position.Y + movable.Velocity.Y
                        );
                    }
                }
            }
        }
    }
}