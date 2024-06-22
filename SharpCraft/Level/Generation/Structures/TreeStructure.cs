using SharpCraft.Level.Blocks;

namespace SharpCraft.Level.Generation.Structures;

public class TreeStructure : Structure
{
    public TreeStructure() : base(5, 7, 5)
    {
        byte a = 0, w = BlockRegistry.Wood.Id, l = BlockRegistry.Leaves.Id;

        WriteBlocks([
                // y=0
                new byte[][]
                {
                    [a, a, a, a, a],
                    [a, a, a, a, a],
                    [a, a, w, a, a],
                    [a, a, a, a, a],
                    [a, a, a, a, a]
                },
                // y=1
                new byte[][]
                {
                    [a, a, a, a, a],
                    [a, a, a, a, a],
                    [a, a, w, a, a],
                    [a, a, a, a, a],
                    [a, a, a, a, a]
                },
                // y=2
                new byte[][]
                {
                    [a, a, a, a, a],
                    [a, a, a, a, a],
                    [a, a, w, a, a],
                    [a, a, a, a, a],
                    [a, a, a, a, a]
                },
                // y=3
                new byte[][]
                {
                    [l, l, l, l, l],
                    [l, l, l, l, l],
                    [l, l, w, l, l],
                    [l, l, l, l, l],
                    [l, l, l, l, l]
                },
                // y=4
                new byte[][]
                {
                    [l, l, l, l, a],
                    [l, l, l, l, l],
                    [l, l, w, l, l],
                    [l, l, l, l, l],
                    [a, l, l, l, a]
                },
                // y=5
                new byte[][]
                {
                    [a, a, a, a, a],
                    [a, l, l, l, a],
                    [a, l, l, l, a],
                    [a, l, l, l, a],
                    [a, a, a, a, a]
                },
                // y=6
                new byte[][]
                {
                    [a, a, a, a, a],
                    [a, a, l, l, a],
                    [a, l, l, l, a],
                    [a, a, l, a, a],
                    [a, a, a, a, a]
                }
            ]
        );
    }
}