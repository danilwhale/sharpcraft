namespace SharpCraft.Utilities;

public static class Assets
{
    public const string Root = "Assets";
    private static readonly Dictionary<string, Texture2D> Textures = [];
    private static readonly Dictionary<string, Material> Materials = [];

    public static Texture2D GetTexture(string path)
    {
        if (Textures.TryGetValue(path, out var texture)) return texture;

        texture = LoadTexture(Path.Join(Root, path));

        Textures[path] = texture;
        return texture;
    }
    
    public static Material GetTextureMaterial(string texturePath)
    {
        if (Materials.TryGetValue(texturePath, out var material)) return material;

        material = LoadMaterialDefault();
        SetMaterialTexture(ref material, MaterialMapIndex.Albedo, GetTexture(texturePath));
        Materials[texturePath] = material;

        return material;
    }

    public static unsafe void SetMaterialColor(string texturePath, MaterialMapIndex mapIndex, Color color)
    {
        var material = GetTextureMaterial(texturePath);
        material.Maps[(int)mapIndex].Color = color;
        SetMaterial(texturePath, material);
    }

    public static void SetMaterialShader(string texturePath, Shader shader)
    {
        var material = GetTextureMaterial(texturePath);
        material.Shader = shader;
        SetMaterial(texturePath, material);
    }

    public static void SetMaterial(string texturePath, Material material)
    {
        Materials[texturePath] = material;
    }

    public static string GetText(string path)
    {
        if (!File.Exists(Path.Join(Root, path)))
        {
            throw new Exception("Asset not found: " + path);
        }

        return File.ReadAllText(Path.Join(Root, path));
    }

    public static void Unload()
    {
        foreach (var keyValue in Textures)
        {
            UnloadTexture(keyValue.Value);
        }
    }
}