using SharpCraft.Registries;
using SharpCraft.Rendering;
using SharpCraft.World.Rendering;

namespace SharpCraft.Tiles.Types;

public sealed class BushTile(byte id) : Tile(id, 15, new TileCapabilities
{
    IsSolid = false,
    CanBlockLight = false
})
{
    private const int Rotations = 2;
    
    public override void Build(IVertexBuilder builder, World.World? world, int x, int y, int z)
    {
        var u0 = (TextureIndex & 15) * TileRender.TexFactor;
        var u1 = u0 + TileRender.TexFactor;
        var v0 = (TextureIndex >> 4) * TileRender.TexFactor;
        var v1 = v0 + TileRender.TexFactor;

        var y1 = y + 1.0f;
        
        builder.Light(1.0f);

        for (var rot = 0; rot < Rotations; rot++)
        {
            var xOff = MathF.Sin(rot * MathF.PI / Rotations + MathF.PI * 0.25f) * 0.5f;
            var zOff = MathF.Cos(rot * MathF.PI / Rotations + MathF.PI * 0.25f) * 0.5f;

            var x0 = x + 0.5f - xOff;
            var x1 = x + 0.5f + xOff;
            var z0 = z + 0.5f - zOff;
            var z1 = z + 0.5f + zOff;
            
            builder.VertexTex(x0, y1, z0, u1, v0);
            builder.VertexTex(x1, y1, z1, u0, v0);
            builder.VertexTex(x1, y, z1, u0, v1);
            builder.VertexTex(x0, y, z0, u1, v1);
            
            builder.VertexTex(x1, y1, z1, u0, v0);
            builder.VertexTex(x0, y1, z0, u1, v0);
            builder.VertexTex(x0, y, z0, u1, v1);
            builder.VertexTex(x1, y, z1, u0, v1);
        }
    }

    public override void Tick(World.World world, int x, int y, int z, Random random)
    {
        var tileBelow = world.GetTile(x, y - 1, z);
        if (!world.IsLit(x, y, z) || !TileGroups.Growable.HasTile(tileBelow))
        {   
            world.TrySetTile(x, y, z, 0);
        }
    }
}