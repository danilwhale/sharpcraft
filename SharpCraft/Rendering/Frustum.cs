using System.Numerics;
using System.Runtime.CompilerServices;

namespace SharpCraft.Rendering;

public sealed class Frustum
{
    private const int Left = 0;
    private const int Right = 1;
    private const int Top = 2;
    private const int Bottom = 3;
    private const int Near = 4;
    private const int Far = 5;
    
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
        var mvp = Matrix4x4.Transpose(Raymath.MatrixMultiply(Rlgl.GetMatrixModelview(), Rlgl.GetMatrixProjection()));
        
        _planes[Left] = Plane.Normalize(new Plane(
            mvp.M14 + mvp.M11,
            mvp.M24 + mvp.M21,
            mvp.M34 + mvp.M31,
            mvp.M44 + mvp.M41
        ));
        
        _planes[Right] = Plane.Normalize(new Plane(
            mvp.M14 - mvp.M11,
            mvp.M24 - mvp.M21,
            mvp.M34 - mvp.M31,
            mvp.M44 - mvp.M41
        ));
        
        _planes[Top] = Plane.Normalize(new Plane(
            mvp.M14 - mvp.M12,
            mvp.M24 - mvp.M22,
            mvp.M34 - mvp.M32,
            mvp.M44 - mvp.M42
        ));
        
        _planes[Bottom] = Plane.Normalize(new Plane(
            mvp.M14 + mvp.M12,
            mvp.M24 + mvp.M22,
            mvp.M34 + mvp.M32,
            mvp.M44 + mvp.M42
        ));
        
        _planes[Near] = Plane.Normalize(new Plane(
            mvp.M13,
            mvp.M23,
            mvp.M33,
            mvp.M43
        ));
        
        _planes[Far] = Plane.Normalize(new Plane(
            mvp.M14 - mvp.M13,
            mvp.M24 - mvp.M23,
            mvp.M34 - mvp.M33,
            mvp.M44 - mvp.M43
        ));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float DistanceToPoint(Plane p, float x, float y, float z) =>
        Plane.DotCoordinate(p, new Vector3(x, y, z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBoxSideOutside(Plane plane, BoundingBox bbox)
    {
        var x = plane.Normal.X >= 0.0f ? bbox.Max.X : bbox.Min.X;
        var y = plane.Normal.Y >= 0.0f ? bbox.Max.Y : bbox.Min.Y;
        var z = plane.Normal.Z >= 0.0f ? bbox.Max.Z : bbox.Min.Z;
        
        return DistanceToPoint(plane, x, y, z) < 0.0f;
    }

    public bool IsBoxOutsideHorizontalPlane(BoundingBox box)
    {
        return IsBoxSideOutside(_planes[Left], box) || 
               IsBoxSideOutside(_planes[Right], box);
    }

    public bool IsBoxOutsideVerticalPlane(BoundingBox box)
    {
        return IsBoxSideOutside(_planes[Top], box) ||
               IsBoxSideOutside(_planes[Bottom], box);
    }
}