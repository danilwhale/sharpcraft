using System.Numerics;

namespace SharpCraft.Entities;

public sealed class Player : Entity
{
    private const float MouseSensitivity = 0.075f;
    private const float HalfWidth = 0.3f;
    private const float HalfHeight = 0.9f;
    
    public Camera3D Camera;

    public Player(Level.Level level) : base(level, HalfWidth, HalfHeight)
    {
        Camera = new Camera3D(Vector3.Zero, Vector3.Zero, Vector3.UnitY, 70.0f, CameraProjection.Perspective);
        MoveToRandom();
    }
    
    public void MoveCamera(float lastDelta)
    {
        Camera.Position = LastPosition + (Position - LastPosition) * lastDelta;

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
        Pitch = Math.Clamp(Pitch, -89.0f, 89.0f);
    }

    public override void Tick()
    {
        base.Tick();
        
        if (IsKeyDown(KeyboardKey.R)) MoveToRandom();

        var x = IsKeyDown(KeyboardKey.A) || IsKeyDown(KeyboardKey.Left) ? 1
            : IsKeyDown(KeyboardKey.D) || IsKeyDown(KeyboardKey.Right) ? -1
            : 0;

        var z = IsKeyDown(KeyboardKey.W) || IsKeyDown(KeyboardKey.Up) ? 1
            : IsKeyDown(KeyboardKey.S) || IsKeyDown(KeyboardKey.Down) ? -1
            : 0;

        if (IsOnGround && (IsKeyDown(KeyboardKey.Space) || IsKeyDown(KeyboardKey.LeftSuper)))
        {
            Direction.Y = 0.12f;
        }

        MoveRelative(x, z, IsOnGround ? 0.02f : 0.005f);
        Direction.Y -= 0.005f;
        Move(Direction);
        
        Direction *= new Vector3(0.91f, 0.98f, 0.91f);

        if (IsOnGround) Direction *= new Vector3(0.8f, 1.0f, 0.8f);
    }
}