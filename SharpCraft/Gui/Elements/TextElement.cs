using System.Numerics;

namespace SharpCraft.Gui.Elements;

public class TextElement : Element
{
    public Font Font = GetFontDefault();
    public string Text = string.Empty;
    public float Size = 12.0f;
    public float Spacing = -1.0f;
    public Color Tint = Color.White;
    
    public override void Update()
    {
        
    }

    public override void Draw()
    {
        DrawTextEx(Font, Text, Position, Size, GetSpacing(), Tint);
    }

    private float GetSpacing() => Spacing < 0 ? MathF.Max(Size, Font.BaseSize) / Font.BaseSize : Spacing;

    public void Center()
    {
        var measure = MeasureTextEx(Font, Text, Size, GetSpacing());
        
        Position = new Vector2(
            GetScreenWidth() / 2.0f - measure.X / 2.0f,
            GetScreenHeight() / 2.0f - measure.Y / 2.0f
        );
    }
}