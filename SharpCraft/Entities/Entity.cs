using System.Numerics;
using SharpCraft.Physics;

namespace SharpCraft.Entities;

public abstract class Entity(Level.Level level, float halfWidth, float halfHeight)
{
    protected float EyeLevel;
    
    protected Vector3 LastPosition;
    protected Vector3 Position;
    protected Vector3 Direction;
    private BoundingBox _bbox;
    
    protected bool IsOnGround;
    
    protected float Yaw;
    protected float Pitch;

    protected readonly Level.Level Level = level;

    public void MoveTo(Vector3 newPosition)
    {
        Position = newPosition;
        _bbox = new BoundingBox(
            newPosition - new Vector3(halfWidth, halfHeight, halfWidth),
            newPosition + new Vector3(halfWidth, halfHeight, halfWidth)
        );
    }

    public virtual void Rotate(float pitch, float yaw)
    {
        Pitch += pitch;
        Yaw -= yaw;
    }

    public virtual void Tick()
    {
        LastPosition = Position;
    }
    
    protected void Move(Vector3 direction)
    {
        var oldDirection = direction;

        var boxes = Level.GetBoxes(_bbox.Expand(direction));
        
        foreach (var box in boxes)
        {
            direction.X = box.ClipXCollide(_bbox, direction.X);
        }

        _bbox.Move(direction.X, 0.0f, 0.0f);
        
        foreach (var box in boxes)
        {
            direction.Y = box.ClipYCollide(_bbox, direction.Y);
        }

        _bbox.Move(0.0f, direction.Y, 0.0f);
        
        foreach (var box in boxes)
        {
            direction.Z = box.ClipZCollide(_bbox, direction.Z);
        }

        _bbox.Move(0.0f, 0.0f, direction.Z);

        ClampBox();
        
        // ReSharper disable CompareOfFloatsByEqualityOperator
        IsOnGround = oldDirection.Y != direction.Y && oldDirection.Y < 0.0f;

        Direction.X = oldDirection.X != direction.X ? 0.0f : direction.X;
        Direction.Y = oldDirection.Y != direction.Y ? 0.0f : direction.Y;
        Direction.Z = oldDirection.Z != direction.Z ? 0.0f : direction.Z;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        Position = new Vector3(
            (_bbox.Min.X + _bbox.Max.X) / 2.0f,
            _bbox.Min.Y + EyeLevel,
            (_bbox.Min.Z + _bbox.Max.Z) / 2.0f
        );
    }

    private void ClampBox()
    {
        if (_bbox.Min.X < 0.0f)
        {
            _bbox.Min.X = 0.0f;
            _bbox.Max.X = halfWidth;
        }

        if (_bbox.Min.Z < 0.0f)
        {
            _bbox.Min.Z = 0.0f;
            _bbox.Max.Z = halfWidth;
        }
        
        if (_bbox.Max.X > Level.Width)
        {
            _bbox.Min.X = Level.Width - halfWidth;
            _bbox.Max.X = Level.Width;
        }

        if (_bbox.Max.Z > Level.Length)
        {
            _bbox.Min.Z = Level.Length - halfWidth;
            _bbox.Max.Z = Level.Length;
        }
    }

    protected void MoveRelative(float x, float z, float speed)
    {
        var dist = x * x + z * z;

        if (dist < 0.01f) return;

        dist = speed / MathF.Sqrt(dist);

        x *= dist;
        z *= dist;

        var sin = MathF.Sin(Yaw * DEG2RAD);
        var cos = MathF.Cos(Yaw * DEG2RAD);

        Direction.X += x * cos + z * sin;
        Direction.Z += z * cos - x * sin;
    }
}