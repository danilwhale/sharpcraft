namespace SharpCraft.Rendering.Models;

public readonly struct Polygon(Vertex a, Vertex b, Vertex c, Vertex d)
{
    public readonly Vertex A = a;
    public readonly Vertex B = b;
    public readonly Vertex C = c;
    public readonly Vertex D = d;

    public void Draw()
    {
        A.Draw();
        B.Draw();
        C.Draw();
        
        A.Draw();
        C.Draw();
        D.Draw();
    }

    public Polygon Move(float x, float y, float z)
    {
        return new Polygon(A.Move(x, y, z), B.Move(x, y, z), C.Move(x, y, z), D.Move(x, y, z));
    }
}