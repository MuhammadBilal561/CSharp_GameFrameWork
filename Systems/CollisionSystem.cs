using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameFrameWork
{
    public class CollisionSystem
    {
        public void Check(List<GameObject> objects)
        {
            var collidables = objects.OfType<ICollidable>().ToList();

            for (int i = 0; i < collidables.Count; i++)
            {
                for (int j = i + 1; j < collidables.Count; j++)
                {
                    if (collidables[i].Bounds.IntersectsWith(collidables[j].Bounds))
                    {
                        var a = (GameObject)collidables[i];
                        var b = (GameObject)collidables[j];

                        var overlap = RectangleF.Intersect(a.Bounds, b.Bounds);
                        
                        if (overlap.Width > 0 && overlap.Height > 0)
                        {
                            // push non-rigid body out of rigid body
                            if (a.IsRigidBody && !b.IsRigidBody)
                            {
                                PushOut(b, a, overlap);
                                b.Velocity = PointF.Empty;
                            }
                            else if (b.IsRigidBody && !a.IsRigidBody)
                            {
                                PushOut(a, b, overlap);
                                a.Velocity = PointF.Empty;
                            }
                            else
                            {
                                // both can move - push apart equally
                                PushApart(a, b, overlap);
                            }

                            if (a.IsRigidBody)
                            {
                                a.Velocity = PointF.Empty;
                                a.HasPhysics = false;
                            }
                            if (b.IsRigidBody)
                            {
                                b.Velocity = PointF.Empty;
                                b.HasPhysics = false;
                            }
                        }

                        collidables[i].OnCollision((GameObject)collidables[j]);
                        collidables[j].OnCollision((GameObject)collidables[i]);
                    }
                }
            }
        }

        private void PushOut(GameObject movable, GameObject rigid, RectangleF overlap)
        {
            if (overlap.Width < overlap.Height)
            {
                if (rigid.Position.X < movable.Position.X)
                    movable.Position = new PointF(movable.Position.X + overlap.Width, movable.Position.Y);
                else
                    movable.Position = new PointF(movable.Position.X - overlap.Width, movable.Position.Y);
            }
            else
            {
                if (rigid.Position.Y < movable.Position.Y)
                    movable.Position = new PointF(movable.Position.X, movable.Position.Y + overlap.Height);
                else
                    movable.Position = new PointF(movable.Position.X, movable.Position.Y - overlap.Height);
            }
        }

        private void PushApart(GameObject a, GameObject b, RectangleF overlap)
        {
            if (overlap.Width < overlap.Height)
            {
                float sep = overlap.Width / 2f;
                if (a.Position.X < b.Position.X)
                {
                    a.Position = new PointF(a.Position.X - sep, a.Position.Y);
                    b.Position = new PointF(b.Position.X + sep, b.Position.Y);
                }
                else
                {
                    a.Position = new PointF(a.Position.X + sep, a.Position.Y);
                    b.Position = new PointF(b.Position.X - sep, b.Position.Y);
                }
            }
            else
            {
                float sep = overlap.Height / 2f;
                if (a.Position.Y < b.Position.Y)
                {
                    a.Position = new PointF(a.Position.X, a.Position.Y - sep);
                    b.Position = new PointF(b.Position.X, b.Position.Y + sep);
                }
                else
                {
                    a.Position = new PointF(a.Position.X, a.Position.Y + sep);
                    b.Position = new PointF(b.Position.X, b.Position.Y - sep);
                }
            }
        }
    }
}
