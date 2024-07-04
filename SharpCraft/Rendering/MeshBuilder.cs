using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpCraft.Rendering;

[SuppressUnmanagedCodeSecurity]
public sealed partial class MeshBuilder : IDisposable
{
    [LibraryImport(NativeLibName)]
    private static partial void UploadMesh(ref Mesh mesh, [MarshalAs(UnmanagedType.I1)] bool isDynamic);
    
    private Mesh _mesh;
    private Mesh _oldMesh;

    private Vector2 _texCoords;
    private Color _color;
    private int _index;

    public void Begin(int triangles)
    {
        Clear();

        if (_mesh.VaoId != 0)
        {
            _oldMesh = _mesh;
        }
        
        _mesh = new Mesh(triangles * 3, triangles);
        _mesh.AllocVertices();
        _mesh.AllocTexCoords();
        _mesh.AllocColors();
    }

    public void TexCoords(float u, float v)
    {
        _texCoords = new Vector2(u, v);
    }

    public void Color(float r, float g, float b)
    {
        _color = ColorFromNormalized(new Vector4(r, g, b, 1.0f));
    }

    public void Vertex(float x, float y, float z)
    {
        _mesh.VerticesAs<Vector3>()[_index] = new Vector3(x, y, z);
        _mesh.TexCoordsAs<Vector2>()[_index] = _texCoords;
        _mesh.ColorsAs<Color>()[_index] = _color;
        
        _index++;
    }

    public void VertexWithTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }

    public void End()
    {
        UploadMesh(ref _mesh, false);

        if (_oldMesh.VaoId == 0) return;
        UnloadMesh(_oldMesh);
        _oldMesh.VaoId = 0;
    }

    private void Clear()
    {
        _index = 0;
        _color = Raylib_cs.Color.White;
        _texCoords = Vector2.Zero;
    }

    public void Draw(Material mat)
    {
        if (_oldMesh.VaoId != 0) DrawMesh(_oldMesh, mat, Matrix4x4.Transpose(Matrix4x4.Identity));
        if (_mesh.VaoId == 0) return;

        DrawMesh(_mesh, mat, Matrix4x4.Transpose(Matrix4x4.Identity));
    }

    public void Dispose()
    {
        if (_oldMesh.VaoId != 0) UnloadMesh(_oldMesh);
        if (_mesh.VaoId == 0) return;

        UnloadMesh(_mesh);
    }
}