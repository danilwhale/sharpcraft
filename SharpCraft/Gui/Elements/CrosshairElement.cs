namespace SharpCraft.Gui.Elements;

public class CrosshairElement(int width, int height, int thickness) : Element
{
    public int Width = width;
    public int Height = height;
    public int Thickness = thickness;
    
    public override void Update()
    {
        
    }

    public override void Draw()
    {
        Rlgl.SetBlendMode(BlendMode.SubtractColors);

        // x0 -> x1
        DrawRectangle(
            GetScreenWidth() / 2 - Width / 2,
            GetScreenHeight() / 2 - Thickness / 2,
            Width,
            Thickness,
            Color.White);

        // y0 -> y0.4
        DrawRectangle(
            GetScreenWidth() / 2 - Thickness / 2,
            GetScreenHeight() / 2 - Height / 2,
            Thickness,
            Height / 2 - Thickness / 2,
            Color.White);

        // y0.6 -> y1
        DrawRectangle(
            GetScreenWidth() / 2 - Thickness / 2,
            GetScreenHeight() / 2 + Thickness / 2,
            Thickness,
            Height / 2 - Thickness / 2,
            Color.White);

        Rlgl.SetBlendMode(BlendMode.Alpha);
    }
}