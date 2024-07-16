using System.Numerics;

namespace SharpCraft.Gui;

public sealed class TextElement : Element
{
    public string Text = string.Empty;
    public Color Tint = Color.White;
    public bool DropShadow = false;

    public override void Update()
    {
    }

    public override void Draw()
    {
        if (DropShadow)
        {
            FontManager.Draw(Text, Vector2.One, Tint, true);
        }

        FontManager.Draw(Text, Vector2.Zero, Tint, false);
    }
}