namespace SharpCraft.Rendering;

public sealed class RlglVertexBuilder : IVertexBuilder
{
    public static readonly RlglVertexBuilder Instance = new();

    public bool EnableColor = true;
    
    private RlglVertexBuilder() { }
    
    public void Begin(DrawMode mode)
    {
        Rlgl.Begin(mode);
    }

    public void End()
    {
        Rlgl.End();
    }

    public void Light(float light)
    {
        if (!EnableColor) return;
        Rlgl.Color4f(light, light, light, 1.0f);
    }

    public void TexCoords(float u, float v)
    {
        Rlgl.TexCoord2f(u, v);
    }

    public void Vertex(float x, float y, float z)
    {
        Rlgl.Vertex3f(x, y, z);
    }

    public void VertexTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }
}