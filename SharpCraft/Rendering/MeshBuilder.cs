using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using NoAlloq;

namespace SharpCraft.Rendering;

[SuppressUnmanagedCodeSecurity]
public sealed partial class MeshBuilder : IDisposable
{
    private readonly struct VertexPrivate(Vector3 position, Vector2 texCoords, Color color)
    {
        public readonly Vector3 Position = position;
        public readonly Vector2 TexCoords = texCoords;
        public readonly Color Color = color;
    }
    
    [LibraryImport(NativeLibName)]
    private static partial void UploadMesh(ref Mesh mesh, [MarshalAs(UnmanagedType.I1)] bool isDynamic);
    
    private Mesh _mesh;
    private Mesh _oldMesh;

    private Vector2 _texCoords;
    private Color _color;

    private readonly List<VertexPrivate> _vertices = [];
    private readonly List<ushort> _indices = [];

    private bool _hasTexCoords;
    private bool _hasColors;
    private bool _hasIndices;

    public void Begin()
    {
        Clear();

        if (_mesh.VaoId != 0)
        {
            _oldMesh = _mesh;
        }
    }

    public void TexCoords(float u, float v)
    {
        _hasTexCoords = true;
        _texCoords = new Vector2(u, v);
    }

    public void Color(float r, float g, float b)
    {
        _hasColors = true;
        _color = ColorFromNormalized(new Vector4(r, g, b, 1.0f));
    }

    public void Vertex(float x, float y, float z)
    {
        _vertices.Add(new VertexPrivate(new Vector3(x, y, z), _texCoords, _color));
    }

    public void VertexTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }

    public void Index(ushort i)
    {
        _hasIndices = true;
        _indices.Add(i);
    }

    public void Triangle(ushort relativeA, ushort relativeB, ushort relativeC)
    {
        Index((ushort)(_vertices.Count + relativeA));
        Index((ushort)(_vertices.Count + relativeB));
        Index((ushort)(_vertices.Count + relativeC));
    }

    public void Quad()
    {
        Triangle(0, 1, 2);
        Triangle(0, 2, 3);
    }

    public void End()
    {
        _mesh = new Mesh(_vertices.Count, _hasIndices ? _indices.Count / 3 : _vertices.Count / 3);
        
        var vertices = CollectionsMarshal.AsSpan(_vertices)[.._vertices.Count];
        
        _mesh.AllocVertices();
        vertices
            .Select(v => v.Position)
            .CopyInto(_mesh.VerticesAs<Vector3>());
        
        if (_hasTexCoords)
        {
            _mesh.AllocTexCoords();
            vertices
                .Select(v => v.TexCoords)
                .CopyInto(_mesh.TexCoordsAs<Vector2>());
        }

        if (_hasColors)
        {
            _mesh.AllocColors();
            vertices
                .Select(v => v.Color)
                .CopyInto(_mesh.ColorsAs<Color>());
        }

        if (_hasIndices)
        {
            _mesh.AllocIndices();
            CollectionsMarshal.AsSpan(_indices)[.._indices.Count]
                .CopyTo(_mesh.IndicesAs<ushort>());
        }
        
        UploadMesh(ref _mesh, false);

        if (_oldMesh.VaoId == 0) return;
        UnloadMesh(_oldMesh);
        _oldMesh.VaoId = 0;
    }

    private void Clear()
    {
        _vertices.Clear();
        _indices.Clear();
        
        _color = Raylib_cs.Color.White;
        _texCoords = Vector2.Zero;

        _hasTexCoords = false;
        _hasColors = false;
        _hasIndices = false;
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