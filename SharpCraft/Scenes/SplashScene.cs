using System.Numerics;
using SharpCraft.Utilities;

namespace SharpCraft.Scenes;

public sealed class SplashScene : IScene
{
    private readonly double _startTime = GetTime();

    public void Update()
    {
        if (GetTime() - _startTime >= 2.0)
        {
            Program.Scene = new GameScene();
        }
    }

    public void Draw()
    {
        ClearBackground(Color.SkyBlue);

        var terrain = ResourceManager.GetTexture("terrain.png");

        for (var x = 0; x < GetScreenWidth() / 128; x++)
        {
            for (var y = 0; y < GetScreenHeight() / 128; y++)
            {
                DrawTexturePro(
                    terrain,
                    new Rectangle(16, 0, 16, 16),
                    new Rectangle(x * 128, y * 128, 128, 128),
                    Vector2.Zero,
                    0.0f,
                    new Color(87, 87, 87, 255)
                    );
            }
        }
        
        DrawTextCentered("sharpcraft", 48, 0, Color.White);
        
        DrawTextCentered("NOT AN OFFICIAL MINECRAFT PRODUCT. NOT APPROVED BY OR ASSOCIATED WITH MOJANG OR MICROSOFT", 16, GetScreenHeight() / 2 - 18, Color.LightGray);
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