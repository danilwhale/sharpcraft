using System.Runtime.InteropServices;
using SharpCraft.Rendering;
using SharpCraft.Rendering.Meshes;

namespace SharpCraft.World.Rendering;

public sealed class ChunkBuilder : IVertexBuilder, IDisposable
{
    private GenericMesh<ChunkVertex> _current;
    private GenericMesh<ChunkVertex> _previous;
    private bool _drawPrevious;

    private List<ChunkVertex> _vertices = null!;
    private List<ushort> _indices = null!;

    private float _u, _v;
    private byte _light;

    private bool _buildQuads;

    public void Begin(DrawMode mode)
    {
        _buildQuads = mode == DrawMode.Quads;

        if (_current.Vao != 0)
        {
            _previous = _current;
            _drawPrevious = true;
        }

        Clear();
    }

    private void Clear()
    {
        _vertices = [];
        _indices = [];

        _u = _v = 0.0f;
        _light = 0;

        _drawPrevious = false;
    }

    public void SetColor(byte r, byte g, byte b)
    {
        
    }

    public void SetUv(float u, float v)
    {
        (_u, _v) = (u, v);
    }

    public void SetLight(float light)
    {
        _light = (byte)(light * 255.0f);
    }

    public void AddVertex(float x, float y, float z)
    {
        _vertices.Add(new ChunkVertex(x, y, z, _u, _v, _light));

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

    public void End()
    {
        _current = new GenericMesh<ChunkVertex>(
            CollectionsMarshal.AsSpan(_vertices),
            CollectionsMarshal.AsSpan(_indices)
        );

        _drawPrevious = false;
        if (_previous.Vao != 0) _previous.Dispose();
    }

    public void Draw(Material material)
    {
        if (_drawPrevious) _previous.Draw(material);
        else _current.Draw(material);
    }

    public void Dispose()
    {
        if (_drawPrevious) _previous.Dispose();
        _current.Dispose();
    }
}