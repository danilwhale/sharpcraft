using System.Numerics;
using SharpCraft.Level.Tiles;
using SharpCraft.Utilities;

namespace SharpCraft.Gui.Elements;

public class BlockSelectionElement : Element
{
    public byte CurrentTile = 1;
    
    public override void Update()
    {
        
    }

    public override void Draw()
    {
        DrawRectangle(16 - 4, GetScreenHeight() - 96 - 16 - 4, 96 + 8, 96 + 8, Color.White);

        var textureIndex = TileRegistry.Tiles[CurrentTile].TextureIndex;
        
        DrawTexturePro(
            ResourceManager.GetTexture("terrain.png"),
            new Rectangle(textureIndex % 16 * 16, textureIndex / 16 * 16, 16, 16),
            new Rectangle(16, GetScreenHeight() - 96 - 16, 96, 96),
            Vector2.Zero,
            0.0f,
            Color.White);
    }
}