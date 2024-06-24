using SharpCraft.Framework;

namespace SharpCraft.Scenes;

public interface IScene : IDisposable
{
    void Update(double deltaTime);
    void Render(MatrixStack matrices, double deltaTime);
}