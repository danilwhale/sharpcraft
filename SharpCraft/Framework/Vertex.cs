using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.Maths;

namespace SharpCraft.Framework;

[StructLayout(LayoutKind.Sequential, Size = 32)]
public readonly struct Vertex(Vector3 position, Vector2 texCoord, Color color)
{
    public readonly Vector3 Position = position;
    public readonly Vector2 TexCoord = texCoord;
    public readonly Color Color = color;

    public Vertex(float x, float y, float z, float u, float v, byte r, byte g, byte b, byte a)
        : this(new Vector3(x, y, z), new Vector2(u, v), new Color(r, g, b, a))
    {
    }
    
    public Vertex(float x, float y, float z, float u, float v, float r, float g, float b, float a)
        : this(new Vector3(x, y, z), new Vector2(u, v), new Color(r, g, b, a))
    {
    }
}