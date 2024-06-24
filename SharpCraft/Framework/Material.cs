namespace SharpCraft.Framework;

public struct Material(Texture texture, Shader shader) : IDisposable
{
    public Texture Texture = texture;
    public Shader Shader = shader;

    public void Dispose()
    {
        Texture.Dispose();
        Shader.Dispose();
    }
}