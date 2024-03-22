using System.Reflection;

namespace SharpCraft.Utilities;

public static class ResourceManager
{
    private static readonly Dictionary<string, Texture2D> Textures = new();

    public static Texture2D GetTexture(string path)
    {
        if (Textures.TryGetValue(path, out var texture)) return texture;

        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(
                typeof(Program),
                "Assets." + path.Replace(Path.PathSeparator, '.')
            );
        
        if (stream == null)
        {
            throw new Exception("explod no such resource explod explod!!");
        }

        var bytes = new byte[stream.Length];
        _ = stream.Read(bytes, 0, bytes.Length);

        var image = LoadImageFromMemory(Path.GetExtension(path), bytes);
        texture = LoadTextureFromImage(image);
        UnloadImage(image);

        Textures[path] = texture;
        return texture;
    }

    public static string GetText(string path)
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(
                typeof(Program),
                "Assets." + path.Replace(Path.PathSeparator, '.')
            );
        
        if (stream == null)
        {
            throw new Exception("explod no such resource explod explod!!");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static void Unload()
    {
        foreach (var keyValue in Textures)
        {
            UnloadTexture(keyValue.Value);
        }
    }
}