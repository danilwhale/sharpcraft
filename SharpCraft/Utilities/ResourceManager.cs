using System.Reflection;
using Serilog;
using Silk.NET.OpenGL;
using StbImageSharp;
using Texture = SharpCraft.Framework.Texture;
using Shader = SharpCraft.Framework.Shader;

namespace SharpCraft.Utilities;

public static class ResourceManager
{
    private static readonly Dictionary<string, Texture> Textures = new();
    private const string Root = "Assets";
    
    public static Texture GetTexture(string path)
    {
        if (Textures.TryGetValue(path, out var texture)) return texture;

        if (!File.Exists(Path.Join(Root, path)))
        {
            Log.Error("Resource {0} doesn't exist!", path);
            return new Texture();
        }

        using var stream = File.OpenRead(Path.Join(Root, path));
        using var result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        texture = new Texture(Program.Gl, result.Data, (uint)result.Width, (uint)result.Height, PixelFormat.Rgba);
        Textures[path] = texture;

        return texture;
    }

    public static Shader GetShader(string? vertexPath = null, string? fragmentPath = null)
    {
        return new Shader(Program.Gl, GetText(vertexPath ?? "Default.vert"), GetText(fragmentPath ?? "Default.frag"));
    }

    public static string GetText(string path)
    {
        if (!File.Exists(Path.Join(Root, path)))
        {
            Log.Error("Resource {0} doesn't exist!", path);
            return string.Empty;
        }

        return File.ReadAllText(Path.Join(Root, path));
    }

    public static void Unload()
    {
        foreach (var (_, texture) in Textures)
        {
            texture.Dispose();
        }
    }
}