using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using NoAlloq;
using SharpCraft.Rendering;

namespace SharpCraft.World.Rendering;

[SuppressUnmanagedCodeSecurity]
public sealed partial class ChunkBuilder : IVertexBuilder, IDisposable
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

    private bool _buildQuads;

    public void Begin(DrawMode mode)
    {
        _buildQuads = mode == DrawMode.Quads;

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

    public void Light(float light)
    {
        _hasColors = true;
        _light = (byte)(light * 255.0f);
    }

    public void Vertex(float x, float y, float z)
    {
        _vertices.Add(new ChunkVertex((Half)x, (Half)y, (Half)z, _u, _v, _light));

        if (_buildQuads && _vertices.Count % 4 != 0) return;

        _hasIndices = true;

        var start = _vertices.Count;

        var a = (ushort)(start - 4);
        var b = (ushort)(start - 3);
        var c = (ushort)(start - 2);
        var d = (ushort)(start - 1);

        // a, b, c
        // a, c, d
        _indices.Add(a);
        _indices.Add(b);
        _indices.Add(c);

        _indices.Add(a);
        _indices.Add(c);
        _indices.Add(d);
    }

    public void VertexTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
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