using System.Numerics;
using SharpCraft.Entities.Models;
using SharpCraft.Utilities;

namespace SharpCraft.Entities;

public sealed class Zombie : WalkingEntity, IDisposable
{
    private const float TimeOffsetSize = 1_239_831.0f;
    private const byte DarkenColor = (byte)(0.6f * 255);

    private readonly ZombieModel _model;

    public readonly float TimeOffset;

    public float Rotation;
    private float _rotationDelta;

    public Zombie(Level.Level level, Vector3 position)
        : base(level, 0.6f, 1.8f)
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
        Assets.SetMaterialColor("char.png", MaterialMapIndex.Albedo,
            !IsLit()
                ? new Color(DarkenColor, DarkenColor, DarkenColor, (byte)255)
                : Color.White);

        _model.Draw(lastPartTicks);
    }

    public override void Dispose()
    {
        _model.Dispose();
    }
}