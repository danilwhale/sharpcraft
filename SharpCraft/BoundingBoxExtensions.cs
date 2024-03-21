using System.Numerics;

namespace SharpCraft;

/// <summary>
/// math™
/// </summary>
public static class BoundingBoxExtensions
{
    public static BoundingBox Expand(this BoundingBox bbox, float x, float y, float z)
    {
        var min = bbox.Min;
        var max = bbox.Max;

        if (x < 0.0f) min.X += x;
        if (x > 0.0f) max.X += x;

        if (y < 0.0f) min.Y += y;
        if (y > 0.0f) max.Y += y;

        if (z < 0.0f) min.Z += z;
        if (z > 0.0f) max.Z += z;

        return new BoundingBox(min, max);
    }

    public static BoundingBox Grow(this BoundingBox bbox, float x, float y, float z)
    {
        return new BoundingBox(bbox.Min - new Vector3(x, y, z), bbox.Max + new Vector3(x, y, z));
    }

    public static float ClipXCollide(this BoundingBox bbox, BoundingBox other, float x)
    {
        if (other.Max.Y <= bbox.Min.Y || other.Min.Y >= bbox.Max.Y) return x;
        if (other.Max.Z <= bbox.Min.Z || other.Min.Z >= bbox.Max.Z) return x;
        
        if (x > 0.0f && other.Max.X <= bbox.Min.X)
        {
            var max = bbox.Min.X - other.Max.X;
            if (max < x)
            {
                x = max;
            }
        }

        if (x < 0.0f && other.Min.X >= bbox.Max.X)
        {
            var max = bbox.Max.X - other.Min.X;
            if (max > x)
            {
                x = max;
            }
        }

        return x;
    }
    
    public static float ClipYCollide(this BoundingBox bbox, BoundingBox other, float y)
    {
        if (other.Max.X <= bbox.Min.X || other.Min.X >= bbox.Max.X) return y;
        if (other.Max.Z <= bbox.Min.Z || other.Min.Z >= bbox.Max.Z) return y;
        
        if (y > 0.0f && other.Max.Y <= bbox.Min.Y)
        {
            var max = bbox.Min.Y - other.Max.Y;
            if (max < y)
            {
                y = max;
            }
        }

        if (y < 0.0f && other.Min.Y >= bbox.Max.Y)
        {
            var max = bbox.Max.Y - other.Min.Y;
            if (max > y)
            {
                y = max;
            }
        }

        return y;
    }
    
    public static float ClipZCollide(this BoundingBox bbox, BoundingBox other, float z)
    {
        if(other.Max.X > bbox.Min.X && other.Min.X < bbox.Max.X) {
            if(other.Max.Y > bbox.Min.Y && other.Min.Y < bbox.Max.Y) {
                float max;
                if(z > 0.0F && other.Max.Z <= bbox.Min.Z) {
                    max = bbox.Min.Z - other.Max.Z;
                    if(max < z) {
                        z = max;
                    }
                }

                if(z < 0.0F && other.Min.Z >= bbox.Max.Z) {
                    max = bbox.Max.Z - other.Min.Z;
                    if(max > z) {
                        z = max;
                    }
                }

                return z;
            } else {
                return z;
            }
        } else {
            return z;
        }

        return z;
    }

    public static void Move(this ref BoundingBox bbox, float x, float y, float z)
    {
        bbox.Min += new Vector3(x, y, z);
        bbox.Max += new Vector3(x, y, z);
    }
}