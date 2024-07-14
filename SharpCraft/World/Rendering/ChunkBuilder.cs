using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using NoAlloq;
using SharpCraft.Rendering;

namespace SharpCraft.World.Rendering;

public sealed class ChunkBuilder : IVertexBuilder, IDisposable
{
    private ChunkMesh _current;
    private ChunkMesh _previous;
    private bool _drawPrevious;

    private List<ChunkVertex> _vertices;
    private List<ushort> _indices;

    private float _u, _v;
    private float _light;

    private bool _drawQuads;

    public void Begin(DrawMode mode)
    {
        _drawQuads = mode == DrawMode.Quads;

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

        _u = _v = _light = 0.0f;

        _drawPrevious = false;
    }

    public void TexCoords(float u, float v)
    {
        (_u, _v) = (u, v);
    }

    public void Light(float light)
    {
        _light = light;
    }

    public void Vertex(float x, float y, float z)
    {
        _vertices.Add(new ChunkVertex(x, y, z, _u, _v, _light));

        if (!_drawQuads || _vertices.Count == 0 || _vertices.Count % 4 != 0) return;
        
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

    public void VertexTex(float x, float y, float z, float u, float v)
    {
        TexCoords(u, v);
        Vertex(x, y, z);
    }

    public void End()
    {
        _current = new ChunkMesh(
            CollectionsMarshal.AsSpan(_vertices),
            CollectionsMarshal.AsSpan(_indices)
        );

        _drawPrevious = false;
        _previous.Dispose();
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