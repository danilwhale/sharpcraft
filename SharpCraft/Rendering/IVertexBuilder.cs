using System.Numerics;
using SharpCraft.Framework;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = SharpCraft.Framework.Shader;

namespace SharpCraft.Rendering;

public interface IVertexBuilder
{
    bool SupportsIndices { get; }

    void Begin(int vertices, int triangles);
    void TexCoords(float u, float v);
    void Color(float r, float g, float b);
    void Vertex(float x, float y, float z);
    void VertexWithTex(float x, float y, float z, float u, float v);
    void Index(ushort index, bool relative);
    void Indices(ushort[] indices, bool relative);
    void End(Shader shader);
    void Draw(MatrixStack matrices, Material material, PrimitiveType mode, Matrix4x4 transform);
}