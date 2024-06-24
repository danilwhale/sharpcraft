using System.Numerics;
using SharpCraft.Physics;
using Silk.NET.Maths;

namespace SharpCraft.Entities;

public class Entity(Level.Level level, float halfWidth, float halfHeight)
{
    protected Vector3 LastPosition;
    protected Vector3 Position;
    protected Vector3 Direction;
    private BoundingBox _bbox;
    
    protected bool IsOnGround;
    
    protected float Yaw;
    protected float Pitch;
    
    protected void MoveToRandom()
    {
        var x = Random.Shared.NextSingle() * level.Width;
        var y = level.Height + 10;
        var z = Random.Shared.NextSingle() * level.Length;
        MoveTo(new Vector3(x, y, z));
    }

    protected void MoveTo(Vector3 newPosition)
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

        var boxes = level.GetBoxes(_bbox.Expand(direction));
        
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

        // ReSharper disable CompareOfFloatsByEqualityOperator
        IsOnGround = oldDirection.Y != direction.Y && oldDirection.Y < 0.0f;

        Direction.X = oldDirection.X != direction.X ? 0.0f : direction.X;
        Direction.Y = oldDirection.Y != direction.Y ? 0.0f : direction.Y;
        Direction.Z = oldDirection.Z != direction.Z ? 0.0f : direction.Z;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        Position = new Vector3(
            (_bbox.Min.X + _bbox.Max.X) / 2.0f,
            _bbox.Min.Y + 1.62f,
            (_bbox.Min.Z + _bbox.Max.Z) / 2.0f
        );
    }

    protected void MoveRelative(float x, float z, float speed)
    {
        var dist = x * x + z * z;

        if (dist < 0.01f) return;

        dist = speed / MathF.Sqrt(dist);

        x *= dist;
        z *= dist;

        var sin = MathF.Sin(float.DegreesToRadians(Yaw));
        var cos = MathF.Cos(float.DegreesToRadians(Yaw));

        Direction.X += x * cos + z * sin;
        Direction.Z += z * cos - x * sin;
    }
}