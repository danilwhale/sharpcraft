using System.Numerics;
using SharpCraft.Utilities;

namespace SharpCraft.Scenes;

public class SplashScene : IScene
{
    private readonly double _startTime = GetTime();
    private readonly int _seed = Random.Shared.Next();

    public void Update()
    {
        if (GetTime() - _startTime >= 2.0)
        {
            Program.Scene = new GameScene();
        }
    }

    public void Draw()
    {
        ClearBackground(ColorBrightness(Color.SkyBlue, -0.6f));

        var terrain = ResourceManager.GetTexture("terrain.png");

        var random = new Random(_seed);
        
        var halfHeight = GetScreenHeight() / 128 / 2;

        for (var x = 0; x < GetScreenWidth() / 128; x++)
        {
            var level = halfHeight + random.Next(1, 3);
            
            for (var y = 0; y < GetScreenHeight() / 128; y++)
            {
                Rectangle source;
                if (y == halfHeight || y == level) source = new Rectangle(0, 0, 16, 16);
                else if (y < halfHeight) continue;
                else source = new Rectangle(16, 0, 16, 16);

                DrawTexturePro(
                    terrain,
                    source,
                    new Rectangle(x * 128, y * 128, 128, 128),
                    Vector2.Zero,
                    0.0f,
                    new Color(87, 87, 87, 255)
                    );
            }
        }
        
        DrawRectangleGradientV(0, 0, GetScreenWidth(), GetScreenHeight(), Color.Blank, Color.Black);
        
        DrawTextCentered("sharpcraft", 48, 0, Color.White);
        DrawTextCentered("comically long splash screen for fun", 16, GetScreenHeight() / 2 - 48, Color.White);
    }

    private void DrawTextCentered(string text, int fontSize, int yOffset, Color color)
    {
        var width = MeasureText(text, fontSize);
        DrawText(text, GetScreenWidth() / 2 - width / 2, GetScreenHeight() / 2 - fontSize / 2 + yOffset, fontSize, color);
    }
    
    public void Dispose()
    {
        
    }
}