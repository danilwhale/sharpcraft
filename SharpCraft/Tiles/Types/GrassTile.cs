namespace SharpCraft.Tiles.Types;

public sealed class GrassTile(byte id) : Tile(id)
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

    public override void Tick(Level.Level level, int x, int y, int z, Random random)
    {
        if (!level.IsLit(x, y, z))
        {
            level.SetTile(x, y, z, TileRegistry.Dirt.Id);
            return;
        }

        for (var i = 0; i < 4; i++)
        {
            var targetX = x + random.Next(3) - 1;
            var targetY = y + random.Next(5) - 3;
            var targetZ = z + random.Next(3) - 1;

            if (level.IsLit(targetX, targetY, targetZ) &&
                level.GetTile(targetX, targetY, targetZ) == TileRegistry.Dirt.Id)
            {
                level.SetTile(targetX, targetY, targetZ, TileRegistry.Grass.Id);
            }
        }
    }
}