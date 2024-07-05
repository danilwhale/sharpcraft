using System.Numerics;
using SharpCraft.Entities.Models;

namespace SharpCraft.Entities;

public sealed class Zombie : Entity
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
        MoveToRandom();

        TimeOffset = Random.Shared.NextSingle() * TimeOffsetSize;
        Rotation = Random.Shared.NextSingle() * MathF.PI * 2.0f;
        _rotationDelta = (Random.Shared.NextSingle() + 1.0f) * 0.01f;

        _model = new ZombieModel(this);
    }

    public override void Tick()
    {
        base.Tick();

        Rotation += _rotationDelta;
        _rotationDelta =
            (_rotationDelta * 0.99f + (Random.Shared.NextSingle() - Random.Shared.NextSingle())) *
            Random.Shared.NextSingle() * Random.Shared.NextSingle() * 0.01f;

        var x = MathF.Sin(Rotation);
        var z = MathF.Cos(Rotation);

        if (IsOnGround && Random.Shared.NextSingle() < 0.01f)
        {
            Direction.Y = 0.12f;
        }

        MoveRelative(x, z, IsOnGround ? 0.02f : 0.005f);
        Direction.Y -= 0.005f;
        Move(Direction);

        Direction *= new Vector3(0.91f, 0.98f, 0.91f);
        
        if (Position.Y < -100.0f)
        {
            MoveToRandom();
        }

        if (IsOnGround)
        {
            Direction.X *= 0.8f;
            Direction.Z *= 0.8f;
        }
    }

    public void Draw(float lastDelta)
    {
        _model.Draw(lastDelta);
    }
}