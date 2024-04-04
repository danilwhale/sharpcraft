using SharpCraft.Gui.Elements;

namespace SharpCraft.Gui.Screens;

public class PauseScreen : Screen
{
    private TextElement _titleElement = new()
    {
        Size = 36.0f,
        Text = "paused"
    };

    public PauseScreen()
    {
        Elements.Add(_titleElement);
    }
    
    public override void Update()
    {
        UpdateElements();
        
        _titleElement.Center();
    }

    public override void Draw()
    {
        DrawRectangleGradientV(0, 0, GetScreenWidth(), GetScreenHeight(), Color.Blank, Color.Black);
        DrawElements();
    }
}