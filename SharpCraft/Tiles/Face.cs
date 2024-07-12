namespace SharpCraft.Tiles;

[Flags]
public enum Face : byte
{
    None = 0,
    
    Top = 1, // Y+ 
    Bottom = 2, // Y-
    
    Right = 4, // X+ 
    Left = 8, // X-
    
    Front = 16, // Z+
    Back = 32, // Z-
    
    All = Left | Right | Top | Bottom | Front | Back,
}