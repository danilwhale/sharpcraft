using System.Numerics;
using System.Runtime.Intrinsics;
using SharpCraft.Framework;
using SharpCraft.Physics;

namespace SharpCraft.Rendering;

public readonly struct Frustum()
{
    private static readonly Frustum FInstance = new();

    public static Frustum GetInstance(MatrixStack matrices)
    {
        FInstance.Recalculate(matrices);
        return FInstance;
    }

    private readonly Plane[] _planes = new Plane[6];
    
    private void Recalculate(MatrixStack matrices)
    {
        var mvp = matrices.ModelView * matrices.Projection;
        
        // left
        _planes[0] = Plane.Normalize(new Plane(
            mvp.M14 + mvp.M11,
            mvp.M24 + mvp.M21,
            mvp.M34 + mvp.M31,
            mvp.M44 + mvp.M41
        ));
        
        // right
        _planes[1] = Plane.Normalize(new Plane(
            mvp.M14 - mvp.M11,
            mvp.M24 - mvp.M21,
            mvp.M34 - mvp.M31,
            mvp.M44 - mvp.M41
        ));
        
        // top
        _planes[2] = Plane.Normalize(new Plane(
            mvp.M14 - mvp.M12,
            mvp.M24 - mvp.M22,
            mvp.M34 - mvp.M32,
            mvp.M44 - mvp.M42
        ));
        
        // bottom
        _planes[3] = Plane.Normalize(new Plane(
            mvp.M14 + mvp.M12,
            mvp.M24 + mvp.M22,
            mvp.M34 + mvp.M32,
            mvp.M44 + mvp.M42
        ));
        
        // near
        _planes[4] = Plane.Normalize(new Plane(
            mvp.M14 + mvp.M13,
            mvp.M24 + mvp.M23,
            mvp.M34 + mvp.M33,
            mvp.M44 + mvp.M43
        ));
        
        // far
        _planes[5] = Plane.Normalize(new Plane(
            mvp.M14 - mvp.M13,
            mvp.M24 - mvp.M23,
            mvp.M34 - mvp.M33,
            mvp.M44 - mvp.M43
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
    
    public bool IsCubeVisible(BoundingBox bbox)
    {
        var x0 = bbox.Min.X;
        var y0 = bbox.Min.Y;
        var z0 = bbox.Min.Z;
        
        var x1 = bbox.Max.X;
        var y1 = bbox.Max.Y;
        var z1 = bbox.Max.Z;
        
        foreach (var p in _planes)
        {
            if (DistanceToPoint(p, x1, y1, z1) < 0.0f &&
                DistanceToPoint(p, x0, y1, z1) < 0.0f &&
                DistanceToPoint(p, x1, y0, z1) < 0.0f &&
                DistanceToPoint(p, x0, y0, z1) < 0.0f &&
                DistanceToPoint(p, x1, y1, z0) < 0.0f &&
                DistanceToPoint(p, x0, y1, z0) < 0.0f &&
                DistanceToPoint(p, x1, y0, z0) < 0.0f &&
                DistanceToPoint(p, x0, y0, z0) < 0.0f)
                return false;
        }

        return true;
    }
}