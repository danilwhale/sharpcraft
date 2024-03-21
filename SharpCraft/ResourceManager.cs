using System.Reflection;

namespace SharpCraft;

public static class ResourceManager
{
    private static Dictionary<string, Texture2D> _textures = new();
    
    public static Texture2D GetTexture(string path)
    {
        if (_textures.TryGetValue(path, out var texture)) return texture;

        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(typeof(ResourceManager), path.Replace(Path.PathSeparator, '.'));
        if (stream == null)
        {
            throw new Exception("explod no such resource explod explod!!");
        }

        var bytes = new byte[stream.Length];
        _ = stream.Read(bytes, 0, bytes.Length);

        var image = LoadImageFromMemory(Path.GetExtension(path), bytes);
        if (!IsImageReady(image)) throw new Exception("failed to load image");
        
        texture = LoadTextureFromImage(image);
        if (!IsTextureReady(texture)) throw new Exception("failed to load texture");
        
        UnloadImage(image);

        _textures[path] = texture;
        return texture;
    }

    public static string GetText(string path)
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(typeof(ResourceManager), path.Replace(Path.PathSeparator, '.'));
        if (stream == null)
        {
            throw new Exception("explod no such resource explod explod!!");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static void Unload()
    {
        foreach (var keyValue in _textures)
        {
            UnloadTexture(keyValue.Value);
        }
    }
}