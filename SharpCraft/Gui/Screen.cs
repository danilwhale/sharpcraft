namespace SharpCraft.Gui;

public abstract class Screen
{
    protected List<Element> Elements = [];

    public abstract void Update();
    public abstract void Draw();

    protected void UpdateElements() => Elements.ForEach(e => e.Update());
    protected void DrawElements() => Elements.ForEach(e => e.Draw());
}