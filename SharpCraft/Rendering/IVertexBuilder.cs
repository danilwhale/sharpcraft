namespace SharpCraft.Rendering;

public interface IVertexBuilder
{
    void Begin(int triangles);
    void TexCoords(float u, float v);
    void Color(float r, float g, float b);
    void Vertex(float x, float y, float z);
    void VertexWithTex(float x, float y, float z, float u, float v);
    void End();
}