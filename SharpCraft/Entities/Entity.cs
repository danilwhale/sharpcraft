using System.Numerics;
using SharpCraft.Extensions;

namespace SharpCraft.Entities;

public abstract class Entity(Level.Level level, float width, float height) : IDisposable
{
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;

            var halfSize = new Vector3(Width * 0.5f, Height * 0.5f, Width * 0.5f);
            
            // center position
            BBox = new BoundingBox(
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
    protected BoundingBox BBox;
    
    protected bool IsOnGround;
    
    public float Yaw;
    public float Pitch;

    protected float HeightOffset;

    internal bool IsDestroyed;

    public EntitySystem? System;

    public Vector3 GetInterpolatedPosition(float lastPartTicks)
    {
        return _lastPosition + (Position - _lastPosition) * lastPartTicks;
    }
    
    public void SetRandomLevelPosition()
    {
        var x = Random.Shared.NextSingle() * level.Width;
        var y = level.Height + 10;
        var z = Random.Shared.NextSingle() * level.Depth;

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

        var boxes = level.GetBoxes(BBox.Expand(motion));
        
        foreach (var box in boxes)
        {
            motion.X = box.ClipXCollide(BBox, motion.X);
        }

        BBox.Move(motion.X, 0.0f, 0.0f);
        
        foreach (var box in boxes)
        {
            motion.Y = box.ClipYCollide(BBox, motion.Y);
        }

        BBox.Move(0.0f, motion.Y, 0.0f);
        
        foreach (var box in boxes)
        {
            motion.Z = box.ClipZCollide(BBox, motion.Z);
        }

        BBox.Move(0.0f, 0.0f, motion.Z);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        IsOnGround = oldMotion.Y != motion.Y && oldMotion.Y < 0.0f;

        Motion.X = oldMotion.X != motion.X ? 0.0f : motion.X;
        Motion.Y = oldMotion.Y != motion.Y ? 0.0f : motion.Y;
        Motion.Z = oldMotion.Z != motion.Z ? 0.0f : motion.Z;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        _position = new Vector3(
            (BBox.Min.X + BBox.Max.X) / 2.0f,
            BBox.Min.Y + HeightOffset,
            (BBox.Min.Z + BBox.Max.Z) / 2.0f
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

    protected bool IsLit() => level.IsLit(Position);

    protected void ApplyLighting(float baseLightValue)
    {
        var value = !IsLit() ? Level.Level.DarkValue : baseLightValue;
        Rlgl.Color3f(value, value, value);
    }
    
    public abstract void Dispose();
}