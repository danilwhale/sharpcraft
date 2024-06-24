using System.Numerics;
using SharpCraft.Framework;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace SharpCraft.Entities;

public class PlayerEntity : Entity
{
    private const float MouseSensitivity = 0.075f;
    private const float HalfWidth = 0.3f;
    private const float HalfHeight = 0.9f;
    
    public Camera Camera;

    public PlayerEntity(Level.Level level) : base(level, HalfWidth, HalfHeight)
    {
        Camera = new Camera(Vector3.Zero, Vector3.Zero, Vector3.UnitY, 70.0f);
        MoveToRandom();
    }
    
    public void MoveCamera(float lastDelta)
    {
        Camera.Position = LastPosition + (Position - LastPosition) * lastDelta;

        var rotation = Matrix4x4.CreateFromYawPitchRoll(
            float.DegreesToRadians(Yaw),
            float.DegreesToRadians(Pitch),
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
        
        var x = Input.IsKeyDown(Key.A) || Input.IsKeyDown(Key.Left) ? 1
            : Input.IsKeyDown(Key.D) || Input.IsKeyDown(Key.Right) ? -1
            : 0;

        var z = Input.IsKeyDown(Key.W) || Input.IsKeyDown(Key.Up) ? 1
            : Input.IsKeyDown(Key.S) || Input.IsKeyDown(Key.Down) ? -1
            : 0;

        if (IsOnGround && (Input.IsKeyDown(Key.Space) || Input.IsKeyDown(Key.SuperLeft)))
        { 
            Direction.Y = 0.16f;
        }

        MoveRelative(x, z, IsOnGround ? 0.023f : 0.007f);
        Direction.Y -= 0.008f;
        Move(Direction);
        
        Direction *= new Vector3(0.91f, 0.98f, 0.91f);

        if (IsOnGround) Direction *= new Vector3(0.8f, 1.0f, 0.8f);
    }

    public void Update()
    {
        if (Input.IsKeyPressed(Key.R)) MoveToRandom();
    }
}