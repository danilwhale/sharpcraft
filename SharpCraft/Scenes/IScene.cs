namespace SharpCraft.Scenes;

public interface IScene : IDisposable
{
    void Update();
    void Draw();
}