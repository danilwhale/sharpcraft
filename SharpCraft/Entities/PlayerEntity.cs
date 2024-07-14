using System.Numerics;

namespace SharpCraft.Entities;

public sealed class PlayerEntity : WalkingEntity
{
    private const float MouseSensitivity = 0.1f;
    
    public Camera3D Camera;

    public PlayerEntity(World.World world) 
        : base(world, 0.6f, 1.8f)
    {
        HeightOffset = 1.62f;
        SetRandomLevelPosition();
        
        Camera = new Camera3D(Vector3.Zero, Vector3.Zero, Vector3.UnitY, 70.0f, CameraProjection.Perspective);
    }
    
    public void MoveCamera(float lastPartTicks)
    {
        Camera.Position = GetInterpolatedPosition(lastPartTicks);

        var rotation = Matrix4x4.CreateFromYawPitchRoll(
            Yaw * DEG2RAD,
            Pitch * DEG2RAD,
            0.0f
        );

        var forward = Vector3.Transform(Vector3.UnitZ, rotation);

        Camera.Target = Camera.Position + forward;
    }

    public override void Rotate(float pitch, float yaw)
    {
        base.Rotate(pitch * MouseSensitivity, yaw * MouseSensitivity);
        Pitch = Math.Clamp(Pitch, -89.9f, 89.9f);
    }

    public override void Tick()
    {
        base.Tick();
        
        if (IsKeyDown(KeyboardKey.R)) SetRandomLevelPosition();

        var x = IsKeyDown(KeyboardKey.A) || IsKeyDown(KeyboardKey.Left) ? 1
            : IsKeyDown(KeyboardKey.D) || IsKeyDown(KeyboardKey.Right) ? -1
            : 0;

        var z = IsKeyDown(KeyboardKey.W) || IsKeyDown(KeyboardKey.Up) ? 1
            : IsKeyDown(KeyboardKey.S) || IsKeyDown(KeyboardKey.Down) ? -1
            : 0;

        if (IsOnGround && (IsKeyDown(KeyboardKey.Space) || IsKeyDown(KeyboardKey.LeftSuper)))
        {
            Jump();
        }

        TickPhysics(x, z);
    }

    public override void Draw(float lastPartTicks)
    {
        
    }

    public override void Dispose()
    {
        
    }
}