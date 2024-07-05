namespace SharpCraft.Scenes;

public sealed class NoAssetsScene : IScene
{
    public void Update()
    {

    }

    public void Draw()
    {
        ClearBackground(Color.Red);
        DrawRectangleGradientV(0, 0, GetScreenWidth(), GetScreenHeight(), new Color(65, 32, 24, 255), new Color(176, 32, 24, 255));

        DrawTextCentered("no assets found!", 48, 0, Color.White);
        DrawTextCentered("please read the instructions in readme (in github repo) to get assets", 28, 48, Color.White);
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