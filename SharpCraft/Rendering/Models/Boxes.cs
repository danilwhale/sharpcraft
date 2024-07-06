using System.Numerics;

namespace SharpCraft.Rendering.Models;

public sealed class Boxes(int texOffsetX, int texOffsetY, float textureWidth, float textureHeight) : IDisposable
{
    public Vector3 Position;
    public Vector3 Rotation;

    private readonly List<Mesh> _meshes = [];

    public void AddBox(float x, float y, float z, int width, int height, int depth)
    {
        var x1 = x + width;
        var y1 = y + height;
        var z1 = z + depth;

        // back vertices
        var b0 = new Vertex(textureWidth, textureHeight, x, y, z);
        var b1 = new Vertex(textureWidth, textureHeight, x1, y, z);
        var b2 = new Vertex(textureWidth, textureHeight, x1, y1, z);
        var b3 = new Vertex(textureWidth, textureHeight, x, y1, z);

        // front vertices
        var f0 = new Vertex(textureWidth, textureHeight, x, y, z1);
        var f1 = new Vertex(textureWidth, textureHeight, x1, y, z1);
        var f2 = new Vertex(textureWidth, textureHeight, x1, y1, z1);
        var f3 = new Vertex(textureWidth, textureHeight, x, y1, z1);

        var p0 = new Polygon(
            f1, b1, b2, f2,
            texOffsetX + depth + width, texOffsetY + depth,
            texOffsetX + width + width + depth, texOffsetY + depth + height);
        var p1 = new Polygon(
            b0, f0, f3, b3,
            texOffsetX, texOffsetY + depth,
            texOffsetX + depth, texOffsetY + depth + height);
        var p2 = new Polygon(
            f1, f0, b0, b1,
            texOffsetX + depth, texOffsetY,
            texOffsetX + depth + width, texOffsetY + depth);
        var p3 = new Polygon(
            b2, b3, f3, f2,
            texOffsetX + depth + width, texOffsetY,
            texOffsetX + depth + width + width, texOffsetY + depth);
        var p4 = new Polygon(
            b1, b0, b3, b2,
            texOffsetX + depth, texOffsetY + depth,
            texOffsetX + depth + width, texOffsetY + depth + height);
        var p5 = new Polygon(
            f0, f1, f2, f3,
            texOffsetX + depth + width + depth, texOffsetY + depth,
            texOffsetX + depth + width + depth + width, texOffsetY + depth + height);

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
        var transform =
            Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
            Matrix4x4.CreateTranslation(Position);
        
        transform = Matrix4x4.Transpose(transform);

        foreach (var mesh in _meshes)
        {
            DrawMesh(mesh, material, transform);
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