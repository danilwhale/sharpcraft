using System.Numerics;

namespace SharpCraft.Extensions;

public static class Camera3DExtensions
{
    public static Ray GetForwardRay(this Camera3D camera)
    {
        return new Ray(camera.Position, Vector3.Normalize(camera.Target - camera.Position));
    }
}