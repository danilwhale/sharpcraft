using System.Numerics;
using SharpCraft.Entities.Models;

namespace SharpCraft.Entities;

public sealed class ZombieEntity : WalkingEntity
{
    private const float TimeOffsetSize = 1_239_831.0f;

    private readonly ZombieModel _model;

    public readonly float TimeOffset;

    public float Rotation;
    private float _rotationDelta;

    public ZombieEntity(World.World world, Vector3 position)
        : base(world, 0.6f, 1.8f)
    {
        Position = position;

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
            Jump();
        }

        TickPhysics(x, z);

        if (Position.Y < -100.0f)
        {
            Destroy();
        }
    }

    public override void Draw(float lastPartTicks)
    {
        _model.Draw(lastPartTicks);
    }

    public override void Dispose()
    {
        _model.Dispose();
    }
}