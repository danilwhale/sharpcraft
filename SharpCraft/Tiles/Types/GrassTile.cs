namespace SharpCraft.Tiles.Types;

public sealed class GrassTile(byte id) : Tile(id, 3)
{
    public override int GetFaceTextureIndex(Face face)
    {
        return face switch
        {
            Face.Top => 0,
            Face.Bottom => 2,
            _ => 3
        };
    }

    public override void Tick(World.World world, int x, int y, int z, Random random)
    {
        if (!world.IsLit(x, y, z))
        {
            world.TrySetTile(x, y, z, Registries.Tiles.Dirt.Id);
            return;
        }

        for (var i = 0; i < 4; i++)
        {
            var targetX = x + random.Next(3) - 1;
            var targetY = y + random.Next(5) - 3;
            var targetZ = z + random.Next(3) - 1;

            if (world.IsLit(targetX, targetY, targetZ) &&
                world.GetTile(targetX, targetY, targetZ) == Registries.Tiles.Dirt.Id)
            {
                world.TrySetTile(targetX, targetY, targetZ, Registries.Tiles.Grass.Id);
            }
        }
    }
}