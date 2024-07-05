using System.Numerics;

namespace SharpCraft.Rendering.Models;

public readonly struct Vertex(float textureWidth, float textureHeight, float x, float y, float z, float u = 0.0f, float v = 0.0f) : IMeshModel
{
    public Vertex WithTexCoords(float u0, float v0)
    {
        return new Vertex(textureWidth, textureHeight, x, y, z, u0, v0);
    }
    
    public unsafe void CopyTo(ref Mesh mesh, int vertexOffset)
    {
        mesh.Vertices[vertexOffset * 3] = x;
        mesh.Vertices[vertexOffset * 3 + 1] = y;
        mesh.Vertices[vertexOffset * 3 + 2] = z;

        mesh.TexCoords[vertexOffset * 2] = u / textureWidth;
        mesh.TexCoords[vertexOffset * 2 + 1] = v / textureHeight;
    }
}