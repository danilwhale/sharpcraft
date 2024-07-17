namespace SharpCraft.World;

public static class WorldGen
{
    private static readonly Random Random = new();
    
    public static void Generate(World world)
    {
        DoTerrainPass(world);
        DoCavePass(world);
    }

    private static void DoTerrainPass(World world)
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

    private static void DoCavePass(World world)
    {
        var wormCount = world.Width * world.Height * world.Depth / 256 / 64;

        for (var worm = 0; worm < wormCount; worm++)
        {
            var x = Random.NextSingle() * world.Width;
            var y = Random.NextSingle() * world.Height;
            var z = Random.NextSingle() * world.Depth;

            var length = Random.NextSingle() + Random.NextSingle() * 150.0f;

            var yaw = Random.NextSingle() * MathF.PI * 2.0f;
            var yawDelta = 0.0f;
            
            var pitch = Random.NextSingle() * MathF.PI * 2.0f;
            var pitchDelta = 0.0f;

            for (var l = 0; l < length; l++)
            {
                x += MathF.Sin(yaw) * MathF.Cos(pitch);
                y += MathF.Sin(pitch);
                z += MathF.Sin(yaw) * MathF.Cos(pitch);

                yaw += yawDelta * 0.2f;
                
                yawDelta *= 0.9f;
                yawDelta += Random.NextSingle() - Random.NextSingle();

                pitch += pitchDelta * 0.5f;
                pitch *= 0.5f;
                
                pitchDelta *= 0.9f;
                pitchDelta += Random.NextSingle() - Random.NextSingle();

                var size = MathF.Sin(l * MathF.PI / length) * 2.5f + 1.0f;

                for (var xx = (int)(x - size); xx <= (int)(x + size); xx++)
                {
                    for (var yy = (int)(y - size); yy <= (int)(y + size); yy++)
                    {
                        for (var zz = (int)(z - size); zz <= (int)(z + size); zz++)
                        {
                            if (xx < 1 || xx >= world.Width - 1 ||
                                yy < 1 || yy >= world.Height - 1 ||
                                zz < 1 || zz >= world.Depth - 1) continue;
                            
                            var distanceX = xx - x;
                            var distanceY = yy - y;
                            var distanceZ = zz - z;
                            
                            var distanceSq = 
                                distanceX * distanceX + 
                                distanceY * distanceY * 2.0f + 
                                distanceZ * distanceZ;

                            if (distanceSq >= size * size) continue;
                            
                            if (world.DirectGetTile(xx, yy, zz) != Registries.Tiles.Rock.Id) continue;

                            world.DirectSetTile(xx, yy, zz, 0);
                        }
                    }
                }
            }
        }
    }
}