using System.Numerics;
using SharpCraft.Utilities;

namespace SharpCraft.Gui;

public static class LoadingScreen
{
    private const double Timeout = 200 / 1000.0;
    
    private static readonly Color BackgroundTint = new(128, 128, 128, 255);

    public static void Display(string title, string description)
    {
        ElementSystem.CalculateViewSize(out var viewWidth, out var viewHeight);
        
        BeginDrawing();
        
        Rlgl.MatrixMode(MatrixMode.Projection);
        Rlgl.PushMatrix();
        Rlgl.LoadIdentity();
        Rlgl.Ortho(0.0, viewWidth, viewHeight, 0.0, 0.01, 1000.0);
        
        Rlgl.MatrixMode(MatrixMode.ModelView);
        Rlgl.LoadIdentity();
        Rlgl.Translatef(0.0f, 0.0f, -200.0f);

        ClearBackground(Color.Black);

        DrawTexturePro(
            Assets.GetTexture("dirt.png"),
            new Rectangle(0.0f, 0.0f, viewWidth / 2, viewHeight / 2),
            new Rectangle(0.0f, 0.0f, viewWidth, viewHeight),
            Vector2.Zero,
            0.0f,
            BackgroundTint
        );
        
        DrawCenteredText(title, -8, viewWidth, viewHeight, Color.White);
        DrawCenteredText(description, 4, viewWidth, viewHeight, Color.White);
        
        EndDrawing();
        WaitTime(Timeout);
    }

    private static void DrawCenteredText(string text, int yOffset, float viewWidth, float viewHeight, Color color)
    {
        FontManager.DrawWithShadow(
            text,
            new Vector2(
                (viewWidth - FontManager.GetTextWidth(text)) * 0.5f,
                viewHeight * 0.5f - FontManager.CharSize * 0.5f + yOffset),
            color
        );
    }
}