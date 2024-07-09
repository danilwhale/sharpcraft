using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using NoAlloq;
using SharpCraft.Rendering.Numerics;

namespace SharpCraft.Rendering;

[SuppressUnmanagedCodeSecurity]
public sealed partial class ChunkBuilder : IDisposable
{
    [LibraryImport(NativeLibName)]
    private static partial void UploadMesh(ref Mesh mesh, [MarshalAs(UnmanagedType.I1)] bool isDynamic);
    
    private Mesh _mesh;
    private Mesh _oldMesh;

    private Half _u, _v;
    private byte _light;

    private List<ChunkVertex> _vertices = [];
    private List<ushort> _indices = [];

    private bool _hasTexCoords;
    private bool _hasColors;
    private bool _hasIndices;

    public void Begin()
    {
        if (_mesh.VaoId != 0)
        {
            _oldMesh = _mesh;
        }
    }

    public void TexCoords(float u, float v)
    {
        _hasTexCoords = true;
        _u = (Half)u;
        _v = (Half)v;
    }

    public void Light(float tint)
    {
        _hasColors = true;
        _light = (byte)(tint * 255.0f);
    }

    public void Vertex(float x, float y, float z)
    {
        _vertices.Add(new ChunkVertex((Half)x, (Half)y, (Half)z, _u, _v, _light));
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
        
        Clear();

        if (_oldMesh.VaoId == 0) return;
        UnloadMesh(_oldMesh);
        _oldMesh.VaoId = 0;
    }

    private void Clear()
    {
        _vertices = [];
        _indices = [];
        
        _light = 255;
        _u = Half.Zero;
        _v = Half.Zero;

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