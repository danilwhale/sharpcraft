namespace SharpCraft.Level.Blocks.Types;

public class LeavesBlock(byte id, int textureIndex) : 
    Block(id, textureIndex, new BlockConfig(IsLightBlocker: false, Layer: BlockLayer.Translucent))
{
    protected override bool ShouldKeepFace(Level level, int x, int y, int z, Face face)
    {
        return true;
    }
}