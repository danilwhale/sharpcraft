namespace SharpCraft.Rendering.Models;

public readonly struct Polygon(Vertex a, Vertex b, Vertex c, Vertex d)
{
    public readonly Vertex A = a;
    public readonly Vertex B = b;
    public readonly Vertex C = c;
    public readonly Vertex D = d;

    public void Add(Mesh mesh, int index)
    {
        A.Add(mesh, index);
        B.Add(mesh, index + 1);
        C.Add(mesh, index + 2);
        
        A.Add(mesh, index + 3);
        C.Add(mesh, index + 4);
        D.Add(mesh, index + 5);
    }

    public Polygon Move(float x, float y, float z)
    {
        return new Polygon(A.Move(x, y, z), B.Move(x, y, z), C.Move(x, y, z), D.Move(x, y, z));
    }
}