using System.Runtime.InteropServices;
using SharpCraft.Rendering.Meshes;

namespace SharpCraft.Rendering;

public sealed class MeshBuilder : IVertexBuilder, IDisposable
{
    public GenericMesh<MeshVertex> Mesh;

    private List<MeshVertex> _vertices = [];
    private List<ushort> _indices = [];
    
    private float _u, _v;
    private byte _r, _g, _b;
    
    private bool _buildQuads;
    
    public void Begin(DrawMode mode)
    {
        _buildQuads = mode == DrawMode.Quads;
        Clear();
    }

    private void Clear()
    {
        _vertices = [];
        _indices = [];

        _u = _v = 0.0f;
        _r = _g = _b = 0;
    }

    public void End()
    {
        if (Mesh.Vao != 0) Mesh.Dispose();
        Mesh = new GenericMesh<MeshVertex>(CollectionsMarshal.AsSpan(_vertices), CollectionsMarshal.AsSpan(_indices));
    }

    public void SetLight(float light)
    {
        _r = _g = _b = (byte)(light * 255.0f);
    }

    public void SetColor(byte r, byte g, byte b)
    {
        (_r, _g, _b) = (r, g, b);
    }

    public void SetUv(float u, float v)
    {
        (_u, _v) = (u, v);
    }

    public void AddVertex(float x, float y, float z)
    {
        _vertices.Add(new MeshVertex(x, y, z, _u, _v, _r, _g, _b, 255));
        
        if (!_buildQuads || _vertices.Count % 4 != 0) return;
        
        var a = (ushort)(_vertices.Count - 4);
        var b = (ushort)(_vertices.Count - 3);
        var c = (ushort)(_vertices.Count - 2);
        var d = (ushort)(_vertices.Count - 1);
        
        _indices.Add(a);
        _indices.Add(b);
        _indices.Add(c);
        
        _indices.Add(a);
        _indices.Add(c);
        _indices.Add(d);
    }

    public void AddVertexWithUv(float x, float y, float z, float u, float v)
    {
        SetUv(u, v);
        AddVertex(x, y, z);
    }

    public void Dispose()
    {
        Mesh.Dispose();
    }
}