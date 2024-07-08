using System.Numerics;

namespace SharpCraft.Entities;

public abstract class WalkingEntity(Level.Level level, float halfWidth, float halfHeight)
    : Entity(level, halfWidth, halfHeight)
{
    private const float GroundSpeed = 0.1f;
    private const float AirSpeed = 0.02f;
    private const float Gravity = 0.08f;
    private const float GroundFriction = 0.7f;
    private const float JumpPower = 0.5f;
    
    protected void TickPhysics(float deltaX, float deltaZ)
    {
        ApplyRelativeMotion(deltaX, deltaZ, IsOnGround ? GroundSpeed : AirSpeed);

        Motion.Y -= Gravity;
        ApplyMotion(Motion);

        Motion *= new Vector3(0.91f, 0.98f, 0.91f);

        if (!IsOnGround) return;
        Motion.X *= GroundFriction;
        Motion.Z *= GroundFriction;
    }

    protected void Jump()
    {
        Motion.Y = JumpPower;
    }
}