using System.Numerics;

namespace SharpCraft.Rendering.Models;

public sealed class Boxes(int texOffsetX, int texOffsetY, float textureWidth, float textureHeight) : IDisposable
{
    public (int X, int Y) TexOffset
    {
        get => (_texOffsetX, _texOffsetY);
        set => (_texOffsetX, _texOffsetY) = value;
    }

    public Vector3 Position;
    public Vector3 Rotation;
    
    private int _texOffsetX = texOffsetX;
    private int _texOffsetY = texOffsetY;

    private readonly List<Mesh> _meshes = [];

    public void AddBox(float x, float y, float z, int xSize, int ySize, int zSize)
    {
        var x1 = x + xSize;
        var y1 = y + ySize;
        var z1 = z + zSize;

        // bottom vertices
        var b0 = new Vertex(textureWidth, textureHeight, x, y, z);
        var b1 = new Vertex(textureWidth, textureHeight, x1, y, z);
        var b2 = new Vertex(textureWidth, textureHeight, x1, y1, z);
        var b3 = new Vertex(textureWidth, textureHeight, x, y1, z);

        // top vertices
        var t0 = new Vertex(textureWidth, textureHeight, x, y, z1);
        var t1 = new Vertex(textureWidth, textureHeight, x1, y, z1);
        var t2 = new Vertex(textureWidth, textureHeight, x1, y1, z1);
        var t3 = new Vertex(textureWidth, textureHeight, x, y1, z1);

        var p0 = new Polygon(
            t1, b1, b2, t2,
            _texOffsetX + zSize + xSize, _texOffsetY + zSize,
            _texOffsetX + xSize + xSize + zSize, _texOffsetY + zSize + ySize);
        var p1 = new Polygon(
            b0, t0, t3, b3,
            _texOffsetX, _texOffsetY + zSize,
            _texOffsetX + zSize, _texOffsetY + zSize + ySize);
        var p2 = new Polygon(
            t1, t0, b0, b1,
            _texOffsetX + zSize, _texOffsetY,
            _texOffsetX + zSize + xSize, _texOffsetY + zSize);
        var p3 = new Polygon(
            b2, b3, t3, t2,
            _texOffsetX + zSize + xSize, _texOffsetY,
            _texOffsetX + zSize + xSize + xSize, _texOffsetY + zSize);
        var p4 = new Polygon(
            b1, b0, b3, b2,
            _texOffsetX + zSize, _texOffsetY + zSize,
            _texOffsetX + zSize + xSize, _texOffsetY + zSize + ySize);
        var p5 = new Polygon(
            t0, t1, t2, t3,
            _texOffsetX + zSize + xSize + zSize, _texOffsetY + zSize,
            _texOffsetX + zSize + xSize + zSize + xSize, _texOffsetY + zSize + ySize);

        var mesh = new Mesh(36, 12);
        mesh.AllocVertices();
        mesh.AllocTexCoords();
        
        p0.CopyTo(ref mesh, 0);
        p1.CopyTo(ref mesh, 6);
        p2.CopyTo(ref mesh, 12);
        p3.CopyTo(ref mesh, 18);
        p4.CopyTo(ref mesh, 24);
        p5.CopyTo(ref mesh, 30);
        
        UploadMesh(ref mesh, false);
        
        _meshes.Add(mesh);
    }

    public void Draw(Material material)
    {
        foreach (var mesh in _meshes)
        {
            DrawMesh(mesh, material, Matrix4x4.Transpose(
                Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                Matrix4x4.CreateTranslation(Position)));
        }
    }

    public void Dispose()
    {
        foreach (var mesh in _meshes)
        {
            UnloadMesh(mesh);
        }
    }
}