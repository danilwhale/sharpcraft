using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using SharpCraft.Framework;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = SharpCraft.Framework.Shader;

namespace SharpCraft.Rendering;

[SuppressUnmanagedCodeSecurity]
public class MeshBuilder : IVertexBuilder, IDisposable
{
    public Mesh? Mesh;
    public Mesh? OldMesh;

    private List<Vertex> _vertices;
    private List<ushort> _indices;
    
    private Vector2 _texCoords;
    private Color _color;

    public bool SupportsIndices => true;

    public void SwapMeshes()
    {
        OldMesh = Mesh;
    }

    public void Begin(int vertices, int triangles)
    {
        Clear();

        if (Mesh is { Vao: > 0 }) SwapMeshes();

        _vertices = new List<Vertex>(vertices);
        _indices = new List<ushort>(triangles * 3);

        Mesh = new Mesh(Program.Gl);
    }

    public void TexCoords(float u, float v)
    {
        _texCoords = new Vector2(u, v);
    }

    public void Color(float r, float g, float b)
    {
        _color = new Color(r, g, b, 1.0f);
    }

    public void Vertex(float x, float y, float z)
    {
        _vertices.Add(new Vertex(x, y, z, _texCoords.X, _texCoords.Y, _color.R, _color.G, _color.B, _color.A));
    }

    public void VertexWithTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }

    public void Index(ushort index, bool relative)
    {
        _indices.Add((ushort)(relative ? _vertices.Count + index : index));
    }

    public void Indices(ushort[] indices, bool relative)
    {
        foreach (var index in indices)
        {
            Index(index, relative);
        }
    }

    public void End(Shader shader)
    {
        Mesh?.Upload(shader, CollectionsMarshal.AsSpan(_vertices), CollectionsMarshal.AsSpan(_indices));

        if (OldMesh?.Vao == 0) return;
        OldMesh?.Dispose();
    }

    private void Clear()
    {
        _color = Framework.Color.White;
        _texCoords = Vector2.Zero;
    }

    public void Draw(MatrixStack matrices, Material material, PrimitiveType mode, Matrix4x4 transform)
    {
        if ((OldMesh != null && OldMesh.Vao != 0) && 
            (Mesh == null || Mesh.Vao == 0)) 
            OldMesh?.Draw(matrices, mode, material, transform);
        if (Mesh == null || Mesh.Vao == 0) return;

        Mesh.Draw(matrices, mode, material, transform);
    }

    public void Dispose()
    {
        if (OldMesh?.Vao != 0) OldMesh?.Dispose();
        if (Mesh?.Vao == 0) return;

        Mesh?.Dispose();
    }
}