using System.Numerics;
using SharpCraft.Framework;
using SharpCraft.Utilities;
using Silk.NET.Maths;

namespace SharpCraft.Scenes;

public class SplashScene : IScene
{
    private readonly double _startTime = Program.MainWindow.Time;
    private readonly int _seed = Random.Shared.Next();

    public void Update(double deltaTime)
    {
        if (Program.MainWindow.Time - _startTime >= 2.0)
        {
            Program.Scene = new GameScene();
        }
    }

    public void Render(MatrixStack matrices, double deltaTime)
    {
        var terrain = ResourceManager.GetTexture("terrain.png");

        var random = new Random(_seed);

        for (var x = 0; x < Program.MainWindow.Size.X / 64; x++)
        {
            for (var y = 0; y < Program.MainWindow.Size.Y / 64; y++)
            {
                // DrawTexturePro(
                //     terrain,
                //     random.NextDouble() < 0.1
                //         ? new Rectangle<float>(16, 0, 16, 16)
                //         : new Rectangle<float>(96, 0, 16, 16),
                //     new Rectangle<float>(x * 64, y * 64, 64, 64),
                //     Vector2.Zero,
                //     0.0f,
                //     new Color(87, 87, 87, 255)
                // );
            }
        }

        // DrawRectangleGradientV(0, 0, GetScreenWidth(), GetScreenHeight(), Color.Blank, Color.Black);

        DrawTextCentered("sharpcraft", 48, 0, Color.White);
    }

    private void DrawTextCentered(string text, int fontSize, int yOffset, Color color)
    {
        // var width = MeasureText(text, fontSize);
        // DrawText(text, GetScreenWidth() / 2 - width / 2, GetScreenHeight() / 2 - fontSize / 2 + yOffset, fontSize,
        //     color);
    }

    public void Dispose()
    {
    }
}