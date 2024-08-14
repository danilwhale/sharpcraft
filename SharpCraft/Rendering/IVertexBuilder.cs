namespace SharpCraft.Rendering;

public interface IVertexBuilder
{
    void Begin(DrawMode mode);
    void End();
    
    void SetLight(float light);
    void SetColor(byte r, byte g, byte b);
    void SetUv(float u, float v);
    void AddVertex(float x, float y, float z);
    void AddVertexWithUv(float x, float y, float z, float u, float v);
}