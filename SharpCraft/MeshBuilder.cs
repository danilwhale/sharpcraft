using System.Numerics;

namespace SharpCraft;

public class MeshBuilder : IDisposable
{
    private const int PositionsIndex = 0;
    private const int TexCoordsIndex = 1;
    private const int ColorsIndex = 3;
    
    public Mesh Mesh;
    private Mesh _oldMesh;

    private Vector2 _texCoords;
    private Color _color;
    private int _index;

    public unsafe void Begin(int triangles)
    {
        Clear();

        if (Mesh.VertexCount != 0)
        {
            _oldMesh = Mesh;
        }
        
        Mesh = new Mesh(triangles * 3, triangles);
        Mesh.AllocVertices();
        Mesh.AllocTexCoords();
        Mesh.AllocColors();
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
        Mesh.VerticesAs<Vector3>()[_index] = new Vector3(x, y, z);
        Mesh.TexCoordsAs<Vector2>()[_index] = _texCoords;
        Mesh.ColorsAs<Color>()[_index] = _color;
        
        _index++;
    }

    public void VertexWithTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }

    public void End()
    {
        if (_oldMesh.VaoId != 0) UnloadMesh(ref _oldMesh);
        
        UploadMesh(ref Mesh, false);
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
        if (Mesh.VaoId == 0) return;

        DrawMesh(Mesh, mat, Matrix4x4.Transpose(Matrix4x4.Identity));
    }

    public void Dispose()
    {
        if (_oldMesh.VaoId != 0) UnloadMesh(ref _oldMesh);
        if (Mesh.VaoId == 0) return;

        UnloadMesh(ref Mesh);
    }
}