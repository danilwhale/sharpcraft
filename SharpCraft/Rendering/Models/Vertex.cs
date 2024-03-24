using System.Numerics;

namespace SharpCraft.Rendering.Models;

public readonly struct Vertex(Vector3 position, Vector2 texCoords)
{
    public readonly Vector3 Position = position;
    public readonly Vector2 TexCoords = texCoords;

    public Vertex(float x, float y, float z, float u, float v)
        : this(new Vector3(x, y, z), new Vector2(u, v)) { }

    public void Add(Mesh mesh, int index)
    {
        mesh.VerticesAs<Vector3>()[index] = Position;
        mesh.TexCoordsAs<Vector2>()[index] = TexCoords;
    }

    public Vertex Move(float x, float y, float z)
    {
        return new Vertex(Position + new Vector3(x, y, z), TexCoords);
    }
}