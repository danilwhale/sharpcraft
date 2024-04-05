using System.Numerics;
using SharpCraft.Level.Tiles;
using SharpCraft.Utilities;

namespace SharpCraft.Gui.Elements;

public class BlockSelectionElement : Element
{
    private const int Size = 48;
    
    public byte CurrentTile = 1;
    public Range PreviewRange = 1..TileRegistry.GetNonNullTileCount();
    
    public override void Update()
    {
        
    }

    public override void Draw()
    {
        var textureIndex = TileRegistry.Tiles[CurrentTile]?.TextureIndex ?? 1;
        
        DrawTile(16, GetScreenHeight() - 112, 96, 4, textureIndex);

        for (var i = PreviewRange.Start.Value; i <= PreviewRange.End.Value; i++)
        {
            var tile = TileRegistry.Tiles[i]!;

            var x = 64 + i * (Size + 8);
            var y = GetScreenHeight() - 64;
            
            DrawTile(x, y, Size, 4, tile.TextureIndex);

            if (CurrentTile == tile.Id)
            {
                DrawRectangle(x, y + Size - 4, Size, 4, Color.SkyBlue);
            }
        }
    }

    private void DrawTile(int x, int y, int size, int padding, int textureIndex)
    {
        DrawRectangle(x, y, size, size, Color.White);
        DrawTexturePro(
            ResourceManager.GetTexture("terrain.png"),
            new Rectangle(textureIndex % 16 * 16, textureIndex / 16 * 16, 16, 16),
            new Rectangle(x + padding, y + padding, size - padding * 2, size - padding * 2),
            Vector2.Zero,
            0.0f,
            Color.White);
    }
}