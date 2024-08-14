using System.Runtime.InteropServices;
using SharpCraft.Rendering.Meshes;

namespace SharpCraft.Rendering;

[StructLayout(LayoutKind.Sequential)]
public readonly struct MeshVertex(float x, float y, float z, float u, float v, byte r, byte g, byte b, byte a) : IVertex
{
    public readonly float X = x, Y = y, Z = z;
    public readonly float U = u, V = v;
    public readonly byte R = r, G = g, B = b, A = a;

    public static VertexAttribute[] Attributes { get; } =
    [
        new VertexAttribute(VertexAttributeType.Float, VertexAttributeLocation.Position, 3, false, 0),
        new VertexAttribute(VertexAttributeType.Float, VertexAttributeLocation.TexCoord, 2, false, 3 * sizeof(float)),
        new VertexAttribute(VertexAttributeType.UnsignedByte, VertexAttributeLocation.Color, 4, true, 5 * sizeof(float))
    ];
}