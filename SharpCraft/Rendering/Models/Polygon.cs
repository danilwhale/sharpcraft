namespace SharpCraft.Rendering.Models;

public readonly struct Polygon(Vertex a, Vertex b, Vertex c, Vertex d) : IMeshModel
{
    public Polygon(Vertex a, Vertex b, Vertex c, Vertex d, float u0, float v0, float u1, float v1)
        : this(a.WithTexCoords(u1, v0), b.WithTexCoords(u0, v0), c.WithTexCoords(u0, v1), d.WithTexCoords(u1, v1))
    {
    }
    
    public void CopyTo(ref Mesh mesh, int vertexOffset)
    {
        d.CopyTo(ref mesh, vertexOffset);
        c.CopyTo(ref mesh, vertexOffset + 1);
        b.CopyTo(ref mesh, vertexOffset + 2);
        
        d.CopyTo(ref mesh, vertexOffset + 3);
        b.CopyTo(ref mesh, vertexOffset + 4);
        a.CopyTo(ref mesh, vertexOffset + 5);
    }
}