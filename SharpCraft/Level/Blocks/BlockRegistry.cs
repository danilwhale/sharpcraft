using SharpCraft.Level.Blocks.Types;

namespace SharpCraft.Level.Blocks;

public static class BlockRegistry
{
    public static readonly Block?[] Blocks = new Block?[255];

    public static readonly Block Stone = new(1, 6);
    public static readonly Block Grass = new GrassBlock(2);
    public static readonly Block Dirt = new(3, 2);
    public static readonly Block Planks = new(4, 4);
    public static readonly Block Sand = new(5, 5);
    public static readonly Block Rock = new(6, 1);
    public static readonly Block Wood = new WoodBlock(7);
    public static readonly Block Leaves = new LeavesBlock(8, 9);
    public static readonly Block WoodenPole = new PoleBlock(9, 10, 4);
    public static readonly Block StonePole = new PoleBlock(10, 11, 1);

    public static readonly Block Glass = new(11, 12,
        BlockConfig.Default with { Layer = BlockLayer.Translucent, IsLightBlocker = false });

    public static int GetExistingBlockCount()
    {
        return Blocks.Count(t => t != null);
    }
}