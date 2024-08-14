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

    public void SetLight(float light)
    {
        if (!EnableColor) return;
        Rlgl.Color4f(light, light, light, 1.0f);
    }

    public void SetColor(byte r, byte g, byte b)
    {
        Rlgl.Color4ub(r, g, b, byte.MaxValue);
    }

    public void SetUv(float u, float v)
    {
        Rlgl.TexCoord2f(u, v);
    }

    public void AddVertex(float x, float y, float z)
    {
        Rlgl.Vertex3f(x, y, z);
    }

    public void AddVertexWithUv(float x, float y, float z, float u, float v)
    {
        SetUv(u, v);
        AddVertex(x, y, z);
    }
}