using System.Numerics;
using SharpCraft.Utilities;

namespace SharpCraft.Scenes;

public class SplashScene : IScene
{
    private readonly double _startTime = GetTime();
    private readonly int _seed = Random.Shared.Next();

    public void Update()
    {
        if (IsKeyPressed(KeyboardKey.F11))
        {
            ToggleBorderlessWindowed();
        }

        if (GetTime() - _startTime >= 2.0)
        {
            Program.Scene = new GameScene();
        }
    }

    public void Draw()
    {
        ClearBackground(ColorBrightness(Color.SkyBlue, -0.6f));

        var terrain = ResourceManager.GetTexture("Terrain.png");

        var random = new Random(_seed);

        for (var x = 0; x < GetScreenWidth() / 64; x++)
        {
            for (var y = 0; y < GetScreenHeight() / 64; y++)
            {
                DrawTexturePro(
                    terrain,
                    random.NextDouble() < 0.1
                        ? new Rectangle(16, 0, 16, 16)
                        : new Rectangle(96, 0, 16, 16),
                    new Rectangle(x * 64, y * 64, 64, 64),
                    Vector2.Zero,
                    0.0f,
                    new Color(87, 87, 87, 255)
                );
            }
        }

        DrawRectangleGradientV(0, 0, GetScreenWidth(), GetScreenHeight(), Color.Blank, Color.Black);

        DrawTextCentered("sharpcraft", 48, 0, Color.White);
    }

    private void DrawTextCentered(string text, int fontSize, int yOffset, Color color)
    {
        var width = MeasureText(text, fontSize);
        DrawText(text, GetScreenWidth() / 2 - width / 2, GetScreenHeight() / 2 - fontSize / 2 + yOffset, fontSize,
            color);
    }

    public void Dispose()
    {
    }
}