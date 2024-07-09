using System.Numerics;
using SharpCraft.Level;
using SharpCraft.Particles;
using SharpCraft.Rendering;

namespace SharpCraft.Tiles;

public class Tile
{
    private const int ParticlesPerTile = 4;
    private const float ParticlesPerTileFactor = 1.0f / ParticlesPerTile;
    
    public readonly TileCapabilities Capabilities = TileCapabilities.Default;

    public readonly byte Id;
    protected readonly int TextureIndex;

    protected Tile(byte id)
    {
        TileRegistry.Registry[Id = id] = this;
    }

    public Tile(byte id, int textureIndex)
        : this(id)
    {
        TextureIndex = textureIndex;
    }

    protected Tile(byte id, TileCapabilities capabilities)
        : this(id)
    {
        Capabilities = capabilities;
    }

    public Tile(byte id, int textureIndex, TileCapabilities capabilities)
        : this(id, textureIndex)
    {
        Capabilities = capabilities;
    }

    public void Build(ChunkBuilder builder, Level.Level level, int x, int y, int z, RenderLayer layer)
    {
        if (Capabilities.Layer != layer) return;
        Build(builder, level, x, y, z);
    }

    protected virtual void Build(ChunkBuilder builder, Level.Level level, int x, int y, int z) 
    {
        TileRender.Render(builder, level, this, GetFaces(level, x, y, z), x, y, z);
    }

    private bool ShouldKeepFace(Level.Level level, int x, int y, int z)
    {
        return !level.IsSolidTile(x, y, z);
    }

    public virtual int GetFaceTextureIndex(Face face)
    {
        return TextureIndex;
    }

    private Face GetFaces(Level.Level level, int x, int y, int z)
    {
        var faces = Face.None;

        if (ShouldKeepFace(level, x + 1, y, z)) faces |= Face.Right;
        if (ShouldKeepFace(level, x - 1, y, z)) faces |= Face.Left;
        if (ShouldKeepFace(level, x, y + 1, z)) faces |= Face.Top;
        if (ShouldKeepFace(level, x, y - 1, z)) faces |= Face.Bottom;
        if (ShouldKeepFace(level, x, y, z + 1)) faces |= Face.Front;
        if (ShouldKeepFace(level, x, y, z - 1)) faces |= Face.Back;

        return faces;
    }

    public virtual void Tick(Level.Level level, int x, int y, int z, Random random)
    {
    }

    public void Break(Level.Level level, int x, int y, int z, ParticleSystem particleSystem)
    {
        for (var i = 0; i < ParticlesPerTile; i++)
        {
            for (var j = 0; j < ParticlesPerTile; j++)
            {
                for (var k = 0; k < ParticlesPerTile; k++)
                {
                    var particleX = x + (i + 0.5f) * ParticlesPerTileFactor;
                    var particleY = y + (j + 0.5f) * ParticlesPerTileFactor;
                    var particleZ = z + (k + 0.5f) * ParticlesPerTileFactor;
                    var particlePosition = new Vector3(particleX, particleY, particleZ);
                    
                    particleSystem.Add(new Particle(
                        level,
                        particlePosition,
                        particlePosition - new Vector3(x, y, z) - new Vector3(0.5f),
                        TextureIndex));
                }
            }
        }
    }
}