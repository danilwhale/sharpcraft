using System.Reflection;

namespace SharpCraft.Utilities;

public static class ResourceManager
{
    private static readonly Dictionary<string, Texture2D> Textures = new();

    public static Texture2D GetTexture(string path)
    {
        if (Textures.TryGetValue(path, out var texture)) return texture;

        texture = LoadTexture(Path.Join("Assets", path));

        Textures[path] = texture;
        return texture;
    }

    public static string GetText(string path)
    {
        if (!File.Exists(Path.Join("Assets", path)))
        {
            throw new Exception("explod no such resource explod explod!!");
        }

        return File.ReadAllText(Path.Join("Assets", path));
    }

    public static void Unload()
    {
        foreach (var keyValue in Textures)
        {
            UnloadTexture(keyValue.Value);
        }
    }
}