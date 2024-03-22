﻿using System.Numerics;

namespace SharpCraft;

public readonly struct Frustum()
{
    private static readonly Frustum FInstance = new();

    public static Frustum Instance
    {
        get
        {
            FInstance.Recalculate();
            return FInstance;
        }
    }

    private readonly Plane[] _planes = new Plane[6];
    
    private void Recalculate()
    {
        var combo = Matrix4x4.Transpose(Rlgl.GetMatrixModelview()) * Matrix4x4.Transpose(Rlgl.GetMatrixProjection());
        
        // left
        _planes[0] = Plane.Normalize(new Plane(
            combo.M14 + combo.M11,
            combo.M24 + combo.M21,
            combo.M34 + combo.M31,
            combo.M44 + combo.M41
        ));
        
        // right
        _planes[1] = Plane.Normalize(new Plane(
            combo.M14 - combo.M11,
            combo.M24 - combo.M21,
            combo.M34 - combo.M31,
            combo.M44 - combo.M41
        ));
        
        // top
        _planes[2] = Plane.Normalize(new Plane(
            combo.M14 - combo.M12,
            combo.M24 - combo.M22,
            combo.M34 - combo.M32,
            combo.M44 - combo.M42
        ));
        
        // bottom
        _planes[3] = Plane.Normalize(new Plane(
            combo.M14 + combo.M12,
            combo.M24 + combo.M22,
            combo.M34 + combo.M32,
            combo.M44 + combo.M42
        ));
        
        // near
        _planes[4] = Plane.Normalize(new Plane(
            combo.M14 + combo.M13,
            combo.M24 + combo.M23,
            combo.M34 + combo.M33,
            combo.M44 + combo.M43
        ));
        
        // far
        _planes[5] = Plane.Normalize(new Plane(
            combo.M14 - combo.M13,
            combo.M24 - combo.M23,
            combo.M34 - combo.M33,
            combo.M44 - combo.M43
        ));
    }

    private float DistanceToPoint(Plane p, float x, float y, float z) =>
        Plane.DotCoordinate(p, new Vector3(x, y, z));

    public bool IsPointVisible(float x, float y, float z)
    {
        foreach (var p in _planes)
        {
            if (DistanceToPoint(p, x, y, z) <= 0.0f) return false;
        }

        return true;
    }

    public bool IsSphereVisible(float x, float y, float z, float radius)
    {
        foreach (var p in _planes)
        {
            if (DistanceToPoint(p, x, y, z) <= -radius) return false;
        }

        return true;
    }

    public bool IsCubeFullyVisible(BoundingBox bbox)
    {
        foreach (var p in _planes)
        {
            var allOutside = true;

            for (var i = 0; i < 8; i++)
            {
                var x = (i & 1) == 0 ? bbox.Min.X : bbox.Max.X;
                var y = (i & 2) == 0 ? bbox.Min.Y : bbox.Max.Y;
                var z = (i & 4) == 0 ? bbox.Min.Z : bbox.Max.Z;

                var distance = DistanceToPoint(p, x, y, z);
                if (distance >= 0.0f)
                {
                    allOutside = false;
                    break;
                }
            }

            if (allOutside) return false;
        }

        return true;
    }
    
    public bool IsCubeVisible(BoundingBox bbox)
    {
        foreach (var p in _planes)
        {
            var allOutside = true;

            for (var i = 0; i < 8; i++)
            {
                var x = (i & 1) == 0 ? bbox.Min.X : bbox.Max.X;
                var y = (i & 2) == 0 ? bbox.Min.Y : bbox.Max.Y;
                var z = (i & 4) == 0 ? bbox.Min.Z : bbox.Max.Z;

                var distance = DistanceToPoint(p, x, y, z);
                if (distance >= 0.0f)
                {
                    allOutside = false;
                }
            }

            if (allOutside) return false;
        }

        return true;
    }
}