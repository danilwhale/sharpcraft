using System.Numerics;
using SharpCraft.Particles;
using SharpCraft.Rendering;
using SharpCraft.World.Rendering;

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
        Registries.Tiles.Registry[Id = id] = this;
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

    public virtual void Build(IVertexBuilder builder, World.World? world, int x, int y, int z, RenderLayer layer) 
    {
        TileRender.Render(builder, world, this, GetFaces(world, x, y, z, layer), x, y, z);
    }

    private bool ShouldKeepFace(World.World? world, int x, int y, int z, RenderLayer layer)
    {
        if (world == null) return true;
        
        return !world.IsSolidTile(x, y, z) && world.IsLit(x, y, z) ^ layer == RenderLayer.Shadow;
    }

    public virtual int GetFaceTextureIndex(Face face)
    {
        return TextureIndex;
    }

    private Face GetFaces(World.World? world, int x, int y, int z, RenderLayer layer)
    {
        var faces = Face.None;

        if (ShouldKeepFace(world, x + 1, y, z, layer)) faces |= Face.Right;
        if (ShouldKeepFace(world, x - 1, y, z, layer)) faces |= Face.Left;
        if (ShouldKeepFace(world, x, y + 1, z, layer)) faces |= Face.Top;
        if (ShouldKeepFace(world, x, y - 1, z, layer)) faces |= Face.Bottom;
        if (ShouldKeepFace(world, x, y, z + 1, layer)) faces |= Face.Front;
        if (ShouldKeepFace(world, x, y, z - 1, layer)) faces |= Face.Back;

        return faces;
    }

    public virtual void Tick(World.World world, int x, int y, int z, Random random)
    {
    }

    public void Break(World.World world, int x, int y, int z, ParticleSystem particleSystem)
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
                        world,
                        particlePosition,
                        particlePosition - new Vector3(x, y, z) - new Vector3(0.5f),
                        TextureIndex));
                }
            }
        }
    }
}