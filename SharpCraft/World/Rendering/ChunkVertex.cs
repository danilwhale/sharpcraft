using System.Numerics;
using System.Runtime.InteropServices;

namespace SharpCraft.World.Rendering;

[StructLayout(LayoutKind.Sequential, Size = 14)]
public readonly struct ChunkVertex(
    Half x,
    Half y,
    Half z,
    Half u,
    Half v,
    byte light)
{
    public readonly Half X = x, Y = y, Z = z;
    public readonly Half U = u, V = v;
    public readonly byte Light = light;

    public Vector3 Position => new((float)X, (float)Y, (float)Z);
    public Vector2 TexCoords => new((float)U, (float)V);
    public Color Color => new(Light, Light, Light, byte.MaxValue);
}