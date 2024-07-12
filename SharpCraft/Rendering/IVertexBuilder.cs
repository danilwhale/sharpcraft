namespace SharpCraft.Rendering;

public interface IVertexBuilder
{
    void Begin(DrawMode mode);
    void End();
    
    void Light(float light);
    void TexCoords(float u, float v);
    void Vertex(float x, float y, float z);
    void VertexTex(float x, float y, float z, float u, float v);
}