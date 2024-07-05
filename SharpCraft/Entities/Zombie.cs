using System.Numerics;
using SharpCraft.Entities.Models;

namespace SharpCraft.Entities;

public sealed class Zombie : Entity, IDisposable
{
    private const float HalfWidth = 0.3f;
    private const float HalfHeight = 0.9f;
    private const float TimeOffsetSize = 1_239_831.0f;

    private readonly ZombieModel _model;

    public readonly float TimeOffset;

    public float Rotation;
    private float _rotationDelta;

    public Zombie(Level.Level level)
        : base(level, HalfWidth, HalfHeight)
    {
        ResetToRandomPosition();

        TimeOffset = Random.Shared.NextSingle() * TimeOffsetSize;
        Rotation = Random.Shared.NextSingle() * MathF.PI * 2.0f;
        _rotationDelta = (Random.Shared.NextSingle() + 1.0f) * 0.01f;

        _model = new ZombieModel(this);
    }

    public override void Tick()
    {
        base.Tick();

        Rotation += _rotationDelta;
        
        _rotationDelta *= 0.99f;
        _rotationDelta +=
            (Random.Shared.NextSingle() - Random.Shared.NextSingle()) *
            Random.Shared.NextSingle() * Random.Shared.NextSingle() * 0.01f;

        // great ai™
        var x = MathF.Sin(Rotation);
        var z = MathF.Cos(Rotation);

        if (IsOnGround && Random.Shared.NextSingle() < 0.01f)
        {
            Motion.Y = 0.12f;
        }

        ApplyRelativeMotion(x, z, IsOnGround ? 0.02f : 0.005f);
        Motion.Y -= 0.005f;
        ApplyMotion(Motion);

        Motion *= new Vector3(0.91f, 0.98f, 0.91f);
        
        if (Position.Y < -100.0f)
        {
            ResetToRandomPosition();
        }

        if (IsOnGround)
        {
            Motion.X *= 0.8f;
            Motion.Z *= 0.8f;
        }
    }

    public void Draw(float lastDelta)
    {
        _model.Draw(lastDelta);
    }

    public void Dispose()
    {
        _model.Dispose();
    }
}