namespace SharpCraft.Rendering.Models;

public readonly struct Polygon(Vertex a, Vertex b, Vertex c, Vertex d)
{
    public readonly Vertex A = a;
    public readonly Vertex B = b;
    public readonly Vertex C = c;
    public readonly Vertex D = d;

    public void Add(MeshBuilder builder)
    {
        A.Add(builder);
        B.Add(builder);
        C.Add(builder);
        
        A.Add(builder);
        C.Add(builder);
        D.Add(builder);
    }
}