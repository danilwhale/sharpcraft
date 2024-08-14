using System.Runtime.InteropServices;
using SharpCraft.Rendering.Meshes;

namespace SharpCraft.World.Rendering;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ChunkVertex(float x, float y, float z, float u, float v, byte light) : IVertex
{
    public readonly float X = x, Y = y, Z = z;
    public readonly float U = u, V = v;
    public readonly byte Light = light;

    public static VertexAttribute[] Attributes { get; } =
    [
        new VertexAttribute(VertexAttributeType.Float, VertexAttributeLocation.Position, 3, false, 0),
        new VertexAttribute(VertexAttributeType.Float, VertexAttributeLocation.TexCoord, 2, false, 3 * sizeof(float)),
        new VertexAttribute(VertexAttributeType.UnsignedByte, VertexAttributeLocation.Color, 1, true, 5 * sizeof(float))
    ];
}