namespace SharpCraft.Rendering;

public class RlglBuilder : IVertexBuilder
{
    public static readonly RlglBuilder Instance = new();
    
    public void Begin(int triangles)
    {
        Rlgl.Begin(DrawMode.Triangles);
    }

    public void TexCoords(float u, float v)
    {
        Rlgl.TexCoord2f(u, v);
    }

    public void Color(float r, float g, float b)
    {
        Rlgl.Color4f(r, g, b, 1.0f);
    }

    public void Vertex(float x, float y, float z)
    {
        Rlgl.Vertex3f(x, y, z);
    }

    public void VertexWithTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }

    public void End()
    {
        Rlgl.End();
    }
}