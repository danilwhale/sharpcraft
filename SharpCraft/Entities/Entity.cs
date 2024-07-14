using System.Numerics;
using SharpCraft.Extensions;

namespace SharpCraft.Entities;

public abstract class Entity(World.World world, float width, float height) : IDisposable
{
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;

            var halfSize = new Vector3(Width * 0.5f, Height * 0.5f, Width * 0.5f);

            // center position
            Box = new BoundingBox(
                value - halfSize,
                value + halfSize
            );
        }
    }

    protected float Width = width;
    protected float Height = height;

    private Vector3 _position;
    private Vector3 _lastPosition;
    protected Vector3 Motion;
    public BoundingBox Box;

    protected bool IsOnGround;

    public float Yaw;
    public float Pitch;

    protected float HeightOffset;

    public bool IsDestroyed;

    public EntitySystem? System;
    public readonly World.World World = world;

    public Vector3 GetInterpolatedPosition(float lastPartTicks)
    {
        return _lastPosition + (Position - _lastPosition) * lastPartTicks;
    }

    public void SetRandomLevelPosition()
    {
        var x = Random.Shared.NextSingle() * World.Width;
        var y = World.Height + 10;
        var z = Random.Shared.NextSingle() * World.Depth;

        Position = new Vector3(x, y, z);
    }

    public virtual void Rotate(float pitch, float yaw)
    {
        Pitch += pitch;
        Yaw += yaw;
    }

    public virtual void Tick()
    {
        _lastPosition = Position;
    }

    public abstract void Draw(float lastPartTicks);

    public void Destroy()
    {
        IsDestroyed = true;
    }

    protected void ApplyMotion(Vector3 motion)
    {
        var oldMotion = motion;

        var boxes = World.GetBoxes(Box.Expand(motion));

        foreach (var box in boxes)
        {
            motion.X = box.ClipXCollide(Box, motion.X);
        }

        Box.Move(motion.X, 0.0f, 0.0f);

        foreach (var box in boxes)
        {
            motion.Y = box.ClipYCollide(Box, motion.Y);
        }

        Box.Move(0.0f, motion.Y, 0.0f);

        foreach (var box in boxes)
        {
            motion.Z = box.ClipZCollide(Box, motion.Z);
        }

        Box.Move(0.0f, 0.0f, motion.Z);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        IsOnGround = oldMotion.Y != motion.Y && oldMotion.Y < 0.0f;

        Motion.X = oldMotion.X != motion.X ? 0.0f : motion.X;
        Motion.Y = oldMotion.Y != motion.Y ? 0.0f : motion.Y;
        Motion.Z = oldMotion.Z != motion.Z ? 0.0f : motion.Z;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        _position = new Vector3(
            (Box.Min.X + Box.Max.X) / 2.0f,
            Box.Min.Y + HeightOffset,
            (Box.Min.Z + Box.Max.Z) / 2.0f
        );
    }

    protected void ApplyRelativeMotion(float x, float z, float speed)
    {
        var dist = x * x + z * z;

        if (dist < 0.01f) return;

        dist = speed / MathF.Sqrt(dist);

        x *= dist;
        z *= dist;

        var sin = MathF.Sin(Yaw * DEG2RAD);
        var cos = MathF.Cos(Yaw * DEG2RAD);

        Motion.X += x * cos + z * sin;
        Motion.Z += z * cos - x * sin;
    }

    public bool IsLit() => World.IsLit(Position);

    public abstract void Dispose();
}