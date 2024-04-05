using System.Numerics;
using SharpCraft.Rendering;

namespace SharpCraft.Level.Tiles;

public class Tile
{
    private const float Darkest = 0.6f;
    private const float Darker = 0.8f;
    private const float Light = 1.0f;

    public readonly byte Id;
    public readonly int TextureIndex;
    public readonly BoundingBox Bounds = new(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f));

    public Tile(byte id, int textureIndex)
    {
        Id = id;
        TileRegistry.Tiles[id] = this;

        TextureIndex = textureIndex;
    }

    public void Build(MeshBuilder builder, Level level, int x, int y, int z)
    {
        var x0 = x + Bounds.Min.X;
        var y0 = y + Bounds.Min.Y;
        var z0 = z + Bounds.Min.Z;
        var x1 = x + Bounds.Max.X;
        var y1 = y + Bounds.Max.Y;
        var z1 = z + Bounds.Max.Z;

        if (!level.IsSolidTile(x - 1, y, z))
        {
            var textureIndex = GetTextureIndexForFace(Face.Left);

            var u0 = textureIndex % 16.0f / 16.0f;
            var u1 = u0 + 1.0f / 16;
            var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
            var v1 = v0 + 1.0f / 16;

            var b = level.GetBrightness(x - 1, y, z) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z0, u0, v0);
        }

        if (!level.IsSolidTile(x + 1, y, z))
        {
            var textureIndex = GetTextureIndexForFace(Face.Right);

            var u0 = textureIndex % 16.0f / 16.0f;
            var u1 = u0 + 1.0f / 16;
            var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
            var v1 = v0 + 1.0f / 16;

            var b = level.GetBrightness(x + 1, y, z) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
            builder.VertexWithTex(x1, y1, z1, u0, v0);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
        }

        if (!level.IsSolidTile(x, y + 1, z))
        {
            var textureIndex = GetTextureIndexForFace(Face.Top);

            var u0 = textureIndex % 16.0f / 16.0f;
            var u1 = u0 + 1.0f / 16;
            var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
            var v1 = v0 + 1.0f / 16;

            var b = level.GetBrightness(x, y + 1, z) * Light;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v1);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x1, y1, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
        }

        if (!level.IsSolidTile(x, y - 1, z))
        {
            var textureIndex = GetTextureIndexForFace(Face.Bottom);

            var u0 = textureIndex % 16.0f / 16.0f;
            var u1 = u0 + 1.0f / 16;
            var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
            var v1 = v0 + 1.0f / 16;

            var b = level.GetBrightness(x, y - 1, z) * Light;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z0, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
        }

        if (!level.IsSolidTile(x, y, z + 1))
        {
            var textureIndex = GetTextureIndexForFace(Face.Front);

            var u0 = textureIndex % 16.0f / 16.0f;
            var u1 = u0 + 1.0f / 16;
            var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
            var v1 = v0 + 1.0f / 16;

            var b = level.GetBrightness(x, y, z + 1) * Darker;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y0, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v0);
        }

        if (!level.IsSolidTile(x, y, z - 1))
        {
            var textureIndex = GetTextureIndexForFace(Face.Back);

            var u0 = textureIndex % 16.0f / 16.0f;
            var u1 = u0 + 1.0f / 16;
            var v0 = MathF.Floor(textureIndex / 16.0f) / 16.0f;
            var v1 = v0 + 1.0f / 16;

            var b = level.GetBrightness(x, y, z - 1) * Darker;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u1, v1);
            builder.VertexWithTex(x0, y1, z0, u1, v0);
            builder.VertexWithTex(x1, y1, z0, u0, v0);

            builder.VertexWithTex(x0, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u0, v0);
            builder.VertexWithTex(x1, y0, z0, u0, v1);
        }
    }

    public int GetFaceCount(Level level, int x, int y, int z)
    {
        var count = 0;

        // check right side
        if (!level.IsSolidTile(x + 1, y, z)) count++;

        // check left side
        if (!level.IsSolidTile(x - 1, y, z)) count++;

        // check top side
        if (!level.IsSolidTile(x, y + 1, z)) count++;

        // check bottom side
        if (!level.IsSolidTile(x, y - 1, z)) count++;

        // check front side
        if (!level.IsSolidTile(x, y, z + 1)) count++;

        // check back side
        if (!level.IsSolidTile(x, y, z - 1)) count++;

        return count;
    }

    public void DrawRlGlFace(TilePosition position, Face face)
    {
        var x0 = position.X + Bounds.Min.X;
        var y0 = position.Y + Bounds.Min.Y;
        var z0 = position.Z + Bounds.Min.Z;
        var x1 = position.X + Bounds.Max.X;
        var y1 = position.Y + Bounds.Max.Y;
        var z1 = position.Z + Bounds.Max.Z;

        switch (face)
        {
            case Face.Left:
                Rlgl.Vertex3f(x0, y0, z0);
                Rlgl.Vertex3f(x0, y0, z1);
                Rlgl.Vertex3f(x0, y1, z1);
                Rlgl.Vertex3f(x0, y1, z0);
                break;

            case Face.Right:
                Rlgl.Vertex3f(x1, y0, z0);
                Rlgl.Vertex3f(x1, y1, z0);
                Rlgl.Vertex3f(x1, y1, z1);
                Rlgl.Vertex3f(x1, y0, z1);
                break;

            case Face.Top:
                Rlgl.Vertex3f(x0, y1, z0);
                Rlgl.Vertex3f(x0, y1, z1);
                Rlgl.Vertex3f(x1, y1, z1);
                Rlgl.Vertex3f(x1, y1, z0);
                break;

            case Face.Bottom:
                Rlgl.Vertex3f(x0, y0, z0);
                Rlgl.Vertex3f(x1, y0, z0);
                Rlgl.Vertex3f(x1, y0, z1);
                Rlgl.Vertex3f(x0, y0, z1);
                break;

            case Face.Front:
                Rlgl.Vertex3f(x0, y0, z1);
                Rlgl.Vertex3f(x1, y0, z1);
                Rlgl.Vertex3f(x1, y1, z1);
                Rlgl.Vertex3f(x0, y1, z1);
                break;

            case Face.Back:
                Rlgl.Vertex3f(x0, y0, z0);
                Rlgl.Vertex3f(x0, y1, z0);
                Rlgl.Vertex3f(x1, y1, z0);
                Rlgl.Vertex3f(x1, y0, z0);
                break;
        }
    }

    protected virtual int GetTextureIndexForFace(Face face)
    {
        return TextureIndex;
    }

    public BoundingBox GetCollision(int x, int y, int z) =>
        new(Bounds.Min + new Vector3(x, y, z), Bounds.Max + new Vector3(x, y, z));

    public virtual bool IsSolid() => true;
    public virtual bool IsLightBlocker() => true;
}