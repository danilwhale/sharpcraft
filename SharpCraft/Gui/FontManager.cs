using System.Numerics;
using SharpCraft.Utilities;

namespace SharpCraft.Gui;

public static class FontManager
{
    private const string AssetName = "default.gif";
    public const int CharSize = 8;

    public static readonly Font Font = Load();

    private static unsafe Font Load()
    {
        var image = LoadImage(Path.Join(Assets.Root, AssetName));
        
        var charsX = image.Width / CharSize;
        var charsY = image.Height / CharSize;

        var font = new Font();
        font.GlyphCount = charsX * charsY;
        font.Glyphs = New<GlyphInfo>((uint)font.GlyphCount);
        font.Recs = New<Rectangle>((uint)font.GlyphCount);
        font.BaseSize = CharSize;
        font.GlyphPadding = 0;
        font.Texture = LoadTextureFromImage(image);

        for (var i = 0; i < font.GlyphCount; i++)
        {
            var glyphX = i % charsX;
            var glyphY = i / charsY;

            var width = 0;
            for (var isColumnEmpty = false; width < CharSize && !isColumnEmpty; width++)
            {
                var pixelX = glyphX * CharSize + width;
                isColumnEmpty = true;
                
                for (var y = 0; y < CharSize && isColumnEmpty; y++)
                {
                    var pixelY = glyphY * CharSize + y;
                    
                    if (GetImageColor(image, pixelX, pixelY).R <= 128) continue;
                    isColumnEmpty = false;
                }
            }

            if (i == ' ') width = 4;
            
            var rect = new Rectangle(glyphX * CharSize, glyphY * CharSize, width, CharSize);
            font.Recs[i] = rect;

            font.Glyphs[i] = new GlyphInfo
            {
                Value = i,
                OffsetX = 0,
                OffsetY = 0,
                AdvanceX = 0,
                Image = ImageFromImage(image, rect)
            };
        }
        
        UnloadImage(image);

        return font;
    }

    public static void Draw(string text, Vector2 position, Color color, bool darken)
    {
        if (darken)
        {
            // bits magic to darken color
            var hex = (uint)ColorToInt(color);
            hex = ((hex & 0xFCFCFCFF) >> 2) | 0x000000FF; // OR with 0x000000FF to restore alpha

            color = GetColor(hex);
        }

        DrawTextEx(Font, text, position, CharSize, 0.0f, color);
    }

    public static void Unload()
    {
        UnloadFont(Font);
    }
}