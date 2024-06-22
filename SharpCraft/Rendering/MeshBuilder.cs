using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpCraft.Rendering;

[SuppressUnmanagedCodeSecurity]
public unsafe partial class MeshBuilder : IVertexBuilder, IDisposable
{
    [LibraryImport(NativeLibName)]
    private static partial void UploadMesh(ref Mesh mesh, [MarshalAs(UnmanagedType.I1)] bool isDynamic);
    
    public Mesh Mesh;
    public Mesh OldMesh;

    private Vector2 _texCoords;
    private Color _color;
    private int _vertex;
    private int _index;

    public bool SupportsIndices => true;

    public void SwapMeshes()
    {
        OldMesh = Mesh;
    }

    public void Begin(int vertices, int triangles)
    {
        Clear();

        if (Mesh.VaoId > 0) SwapMeshes();

        Mesh = new Mesh(vertices, triangles);
        Mesh.AllocVertices();
    }

    public void TexCoords(float u, float v)
    {
        if (Mesh.TexCoords == null) Mesh.AllocTexCoords();
        
        _texCoords = new Vector2(u, v);
    }

    public void Color(float r, float g, float b)
    {
        if (Mesh.Colors == null) Mesh.AllocColors();
        
        _color = ColorFromNormalized(new Vector4(r, g, b, 1.0f));
    }

    public void Vertex(float x, float y, float z)
    {
        Mesh.Vertices[_vertex * 3] = x;
        Mesh.Vertices[_vertex * 3 + 1] = y;
        Mesh.Vertices[_vertex * 3 + 2] = z;

        if (Mesh.TexCoords != null)
        {
            Mesh.TexCoords[_vertex * 2] = _texCoords.X;
            Mesh.TexCoords[_vertex * 2 + 1] = _texCoords.Y;
        }

        if (Mesh.Colors != null)
        {
            Mesh.Colors[_vertex * 4] = _color.R;
            Mesh.Colors[_vertex * 4 + 1] = _color.G;
            Mesh.Colors[_vertex * 4 + 2] = _color.B;
            Mesh.Colors[_vertex * 4 + 3] = _color.A;
        }
        
        _vertex++;
    }

    public void VertexWithTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }

    public void Index(ushort index, bool relative)
    {
        if (Mesh.Indices == null) Mesh.AllocIndices();

        Mesh.Indices[_index++] = (ushort)(relative ? _vertex + index : index);
    }

    public void Indices(ushort[] indices, bool relative)
    {
        foreach (var index in indices)
        {
            Index(index, relative);
        }
    }

    public void End()
    {
        if (Mesh.Vertices == null) return;
        UploadMesh(ref Mesh, false);

        if (OldMesh.VaoId == 0) return;
        UnloadMesh(ref OldMesh);
        OldMesh.VaoId = 0;
    }

    private void Clear()
    {
        _vertex = 0;
        _index = 0;
        _color = Raylib_cs.Color.White;
        _texCoords = Vector2.Zero;
    }

    public void Draw(Material mat)
    {
        if (OldMesh.VaoId != 0 && Mesh.VertexCount > 0) DrawMesh(OldMesh, mat, Matrix4x4.Transpose(Matrix4x4.Identity));
        if (Mesh.VaoId == 0) return;

        DrawMesh(Mesh, mat, Matrix4x4.Transpose(Matrix4x4.Identity));
    }

    public void Dispose()
    {
        if (OldMesh.VaoId != 0) UnloadMesh(ref OldMesh);
        if (Mesh.VaoId == 0) return;

        UnloadMesh(ref Mesh);
    }
}