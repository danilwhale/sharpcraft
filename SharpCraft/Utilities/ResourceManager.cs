using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SharpCraft.Utilities;

public static class ResourceManager
{
    private const string Root = "Assets";
    private static readonly Dictionary<string, Texture2D> Textures = new();
    
    [RequiresAssemblyFiles]
    public static Texture2D GetTexture(string path)
    {
        if (Textures.TryGetValue(path, out var texture)) return texture;

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Program), $"{Root}.{path}");
        if (stream == null)
        {
            throw new Exception("explod no such resource explod explod!!");
        }
        
        using var ms = new MemoryStream();
        stream.CopyTo(ms);

        var image = LoadImageFromMemory(Path.GetExtension(path), ms.ToArray());
        texture = LoadTextureFromImage(image);
        UnloadImage(image);

        Textures[path] = texture;
        return texture;
    }

    [RequiresAssemblyFiles]
    public static string GetText(string path)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Program), $"{Root}.{path}");
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