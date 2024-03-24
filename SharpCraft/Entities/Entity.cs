using System.Numerics;
using SharpCraft.Physics;

namespace SharpCraft.Entities;

public class Entity(Level.Level level, float halfWidth, float halfHeight)
{
    protected Vector3 LastPosition;
    public Vector3 Position;
    protected Vector3 Direction;
    protected BoundingBox BBox;
    
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
        BBox = new BoundingBox(
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

        var boxes = level.GetBoxes(BBox.Expand(direction));
        
        foreach (var box in boxes)
        {
            direction.X = box.ClipXCollide(BBox, direction.X);
        }

        BBox.Move(direction.X, 0.0f, 0.0f);
        
        foreach (var box in boxes)
        {
            direction.Y = box.ClipYCollide(BBox, direction.Y);
        }

        BBox.Move(0.0f, direction.Y, 0.0f);
        
        foreach (var box in boxes)
        {
            direction.Z = box.ClipZCollide(BBox, direction.Z);
        }

        BBox.Move(0.0f, 0.0f, direction.Z);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        IsOnGround = oldDirection.Y != direction.Y && oldDirection.Y < 0.0f;

        Direction.X = oldDirection.X != direction.X ? 0.0f : direction.X;
        Direction.Y = oldDirection.Y != direction.Y ? 0.0f : direction.Y;
        Direction.Z = oldDirection.Z != direction.Z ? 0.0f : direction.Z;
        // ReSharper restore CompareOfFloatsByEqualityOperator

        Position = new Vector3(
            (BBox.Min.X + BBox.Max.X) / 2.0f,
            BBox.Min.Y + 1.62f,
            (BBox.Min.Z + BBox.Max.Z) / 2.0f
        );
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