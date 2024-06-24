using System.Numerics;
using Silk.NET.Maths;

namespace SharpCraft.Physics;

public struct BoundingBox(Vector3 min, Vector3 max)
{
    public Vector3 Min = min;
    public Vector3 Max = max;
    
    public BoundingBox Expand(float x, float y, float z)
    {
        var min = Min;
        var max = Max;

        if (x < 0.0f) min.X += x;
        if (x > 0.0f) max.X += x;

        if (y < 0.0f) min.Y += y;
        if (y > 0.0f) max.Y += y;

        if (z < 0.0f) min.Z += z;
        if (z > 0.0f) max.Z += z;

        return new BoundingBox(min, max);
    }

    public BoundingBox Expand(Vector3 expandVector)
    {
        return Expand(expandVector.X, expandVector.Y, expandVector.Z);
    }

    public BoundingBox Grow(float x, float y, float z)
    {
        return new BoundingBox(Min - new Vector3(x, y, z), Max + new Vector3(x, y, z));
    }

    public BoundingBox Grow(Vector3 growVector)
    {
        return new BoundingBox(Min - growVector, Max + growVector);
    }

    public float ClipXCollide(BoundingBox other, float x)
    {
        if (other.Max.Y <= Min.Y || other.Min.Y >= Max.Y) return x;
        if (other.Max.Z <= Min.Z || other.Min.Z >= Max.Z) return x;

        float max;
        
        if (x > 0.0f && other.Max.X <= Min.X)
        {
            max = Min.X - other.Max.X;
            if (max < x)
            {
                x = max;
            }
        }

        if (x > 0.0f || other.Min.X < Max.X) return x;
        
        max = Max.X - other.Min.X;
        if (max > x)
        {
            x = max;
        }

        return x;
    }

    public float ClipYCollide(BoundingBox other, float y)
    {
        if (other.Max.X <= Min.X || other.Min.X >= Max.X) return y;
        if (other.Max.Z <= Min.Z || other.Min.Z >= Max.Z) return y;

        float max;
        
        if (y > 0.0f && other.Max.Y <= Min.Y)
        {
            max = Min.Y - other.Max.Y;
            if (max < y)
            {
                y = max;
            }
        }

        if (y > 0.0f || other.Min.Y < Max.Y) return y;
        
        max = Max.Y - other.Min.Y;
        if (max > y)
        {
            y = max;
        }

        return y;
    }

    public float ClipZCollide(BoundingBox other, float z)
    {
        if (other.Max.X <= Min.X || other.Min.X >= Max.X) return z;
        if (other.Max.Y <= Min.Y || other.Min.Y >= Max.Y) return z;

        float max;
        
        if (z > 0.0F && other.Max.Z <= Min.Z)
        {
            max = Min.Z - other.Max.Z;
            if (max < z)
            {
                z = max;
            }
        }

        if (z > 0.0F || other.Min.Z < Max.Z) return z;
        
        max = Max.Z - other.Min.Z;
        if (max > z)
        {
            z = max;
        }

        return z;
    }

    public void Move(float x, float y, float z)
    {
        Min += new Vector3(x, y, z);
        Max += new Vector3(x, y, z);
    }

    public void Move(Vector3 moveVector)
    {
        Min += moveVector;
        Max += moveVector;
    }
}