namespace GameFrameWork
{
    public interface IPhysicsObject
    {
        bool HasPhysics { get; set; }
        float? CustomGravity { get; set; }
        bool IsRigidBody { get; set; }
    }
}