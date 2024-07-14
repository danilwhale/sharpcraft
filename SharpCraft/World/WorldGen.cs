namespace SharpCraft.World;

public static class WorldGen
{
    public static void Generate(World world)
    {
        var firstMap = new PerlinNoiseFilter(0).GetMap(world.Width, world.Depth);
        var secondMap = new PerlinNoiseFilter(0).GetMap(world.Width, world.Depth);
        var cliffMap = new PerlinNoiseFilter(1).GetMap(world.Width, world.Depth);
        var rockMap = new PerlinNoiseFilter(1).GetMap(world.Width, world.Depth);

        for (var x = 0; x < world.Width; x++)
        {
            for (var y = 0; y < world.Height; y++)
            {
                for (var z = 0; z < world.Depth; z++)
                {
                    var firstValue = firstMap[x + z * world.Width];
                    var secondValue = secondMap[x + z * world.Width];

                    if (cliffMap[x + z * world.Width] < 128)
                    {
                        secondValue = firstValue;
                    }
                    
                    var maxHeight = Math.Max(firstValue, secondValue) / 8 + world.Height / 3;
                    var maxRockHeight = rockMap[x + z * world.Width] / 8 + world.Height / 3;

                    if (maxRockHeight > maxHeight - 2)
                    {
                        maxRockHeight = maxHeight - 2;
                    }

                    byte id = 0;

                    if (y == maxHeight)
                    {
                        id = Registries.Tiles.Grass.Id;
                    }
                    else if (y <= maxRockHeight)
                    {
                        id = Registries.Tiles.Rock.Id;
                    }
                    else if (y < maxHeight)
                    {
                        id = Registries.Tiles.Dirt.Id;
                    }
                    
                    world.DirectSetTile(x, y, z, id);
                }
            }
        }
    }
}