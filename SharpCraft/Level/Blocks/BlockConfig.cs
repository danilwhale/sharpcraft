namespace SharpCraft.Level.Blocks;

public record BlockConfig(
    bool IsSolid = true, 
    bool IsLightBlocker = true, 
    bool IsBreakable = true,
    bool IsPlaceable = true,
    BlockLayer Layer = BlockLayer.Solid)
{
    public static readonly BlockConfig Default = new();
}