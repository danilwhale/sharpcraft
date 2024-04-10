using System.Numerics;
using SharpCraft.Rendering;

namespace SharpCraft.Level.Tiles;

public class Tile
{
    private const float Darkest = 0.6f;
    private const float Darker = 0.8f;
    private const float Light = 1.0f;

    public readonly TileConfig Config;

    public readonly byte Id;
    public readonly int TextureIndex;
    protected BoundingBox Bounds = new(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f));

    public Tile(byte id, int textureIndex)
        : this(id, textureIndex, TileConfig.Default)
    { }
    
    public Tile(byte id, int textureIndex, TileConfig config)
    {
        Id = id;
        TileRegistry.Tiles[id] = this;

        TextureIndex = textureIndex;

        Config = config;
    }

    public void Build(MeshBuilder builder, Level level, int x, int y, int z, TileLayer layer)
    {
        if (layer != Config.Layer) return;
        
        var x0 = x + Bounds.Min.X;
        var y0 = y + Bounds.Min.Y;
        var z0 = z + Bounds.Min.Z;
        var x1 = x + Bounds.Max.X;
        var y1 = y + Bounds.Max.Y;
        var z1 = z + Bounds.Max.Z;

        if (ShouldKeepFace(level, x - 1, y, z, Face.Left))
        {
            var textureIndex = GetTextureIndexForFace(Face.Left);
            var coords = GetTextureCoordinates(Face.Left, textureIndex);
            
            var u0 = coords.X;
            var u1 = coords.X + coords.Width;
            var v0 = coords.Y;
            var v1 = coords.Y + coords.Height;

            var b = level.GetBrightness(x - 1, y, z) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z0, u0, v1);
            builder.VertexWithTex(x0, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z0, u0, v0);
        }

        if (ShouldKeepFace(level, x + 1, y, z, Face.Right))
        {
            var textureIndex = GetTextureIndexForFace(Face.Right);
            var coords = GetTextureCoordinates(Face.Right, textureIndex);
            
            var u0 = coords.X;
            var u1 = coords.X + coords.Width;
            var v0 = coords.Y;
            var v1 = coords.Y + coords.Height;

            var b = level.GetBrightness(x + 1, y, z) * Darkest;

            builder.Color(b, b, b);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
            builder.VertexWithTex(x1, y1, z1, u0, v0);

            builder.VertexWithTex(x1, y0, z0, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
        }

        if (ShouldKeepFace(level, x, y + 1, z, Face.Top))
        {
            var textureIndex = GetTextureIndexForFace(Face.Top);
            var coords = GetTextureCoordinates(Face.Top, textureIndex);
            
            var u0 = coords.X;
            var u1 = coords.X + coords.Width;
            var v0 = coords.Y;
            var v1 = coords.Y + coords.Height;

            var b = level.GetBrightness(x, y + 1, z) * Light;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v1);

            builder.VertexWithTex(x0, y1, z0, u0, v0);
            builder.VertexWithTex(x1, y1, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z0, u1, v0);
        }

        if (ShouldKeepFace(level, x, y - 1, z, Face.Bottom))
        {
            var textureIndex = GetTextureIndexForFace(Face.Bottom);
            var coords = GetTextureCoordinates(Face.Bottom, textureIndex);
            
            var u0 = coords.X;
            var u1 = coords.X + coords.Width;
            var v0 = coords.Y;
            var v1 = coords.Y + coords.Height;

            var b = level.GetBrightness(x, y - 1, z) * Light;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z0, u0, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);

            builder.VertexWithTex(x0, y0, z0, u1, v0);
            builder.VertexWithTex(x1, y0, z1, u0, v1);
            builder.VertexWithTex(x0, y0, z1, u1, v1);
        }

        if (ShouldKeepFace(level, x, y, z + 1, Face.Front))
        {
            var textureIndex = GetTextureIndexForFace(Face.Front);
            var coords = GetTextureCoordinates(Face.Front, textureIndex);
            
            var u0 = coords.X;
            var u1 = coords.X + coords.Width;
            var v0 = coords.Y;
            var v1 = coords.Y + coords.Height;
            
            var b = level.GetBrightness(x, y, z + 1) * Darker;

            builder.Color(b, b, b);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y0, z1, u1, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);

            builder.VertexWithTex(x0, y0, z1, u0, v1);
            builder.VertexWithTex(x1, y1, z1, u1, v0);
            builder.VertexWithTex(x0, y1, z1, u0, v0);
        }

        if (ShouldKeepFace(level, x, y, z - 1, Face.Back))
        {
            var textureIndex = GetTextureIndexForFace(Face.Back);
            var coords = GetTextureCoordinates(Face.Back, textureIndex);
            
            var u0 = coords.X;
            var u1 = coords.X + coords.Width;
            var v0 = coords.Y;
            var v1 = coords.Y + coords.Height;

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

    public int GetFaceCount(Level level, int x, int y, int z, TileLayer layer)
    {
        if (layer != Config.Layer) return 0;
        
        var count = 0;

        // check right side
        if (ShouldKeepFace(level, x + 1, y, z, Face.Right)) count++;

        // check left side
        if (ShouldKeepFace(level, x - 1, y, z, Face.Left)) count++;

        // check top side
        if (ShouldKeepFace(level, x, y + 1, z, Face.Top)) count++;

        // check bottom side
        if (ShouldKeepFace(level, x, y - 1, z, Face.Bottom)) count++;

        // check front side
        if (ShouldKeepFace(level, x, y, z + 1, Face.Front)) count++;

        // check back side
        if (ShouldKeepFace(level, x, y, z - 1, Face.Back)) count++;

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

    protected virtual bool ShouldKeepFace(Level level, int x, int y, int z, Face face)
    {
        var id = level.GetTile(x, y, z);
        var tile = TileRegistry.Tiles[id];

        return (!tile?.Config.IsSolid ?? true) || tile.Config.Layer != Config.Layer;
    }

    protected virtual Rectangle GetTextureCoordinates(Face face, int textureIndex)
    {
        return new Rectangle(
                textureIndex % 16.0f / 16.0f,
                MathF.Floor(textureIndex / 16.0f) / 16.0f,
                1.0f / 16,
                1.0f / 16
        );
    }

    public BoundingBox GetCollision(int x, int y, int z) =>
        new(Bounds.Min + new Vector3(x, y, z), Bounds.Max + new Vector3(x, y, z));
}