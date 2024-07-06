namespace SharpCraft.Utilities;

public static class Assets
{
    private const string Root = "Assets";
    private static readonly Dictionary<string, Texture2D> Textures = [];
    private static readonly Dictionary<string, Material> Materials = [];

    public static Texture2D GetTexture(string path)
    {
        if (Textures.TryGetValue(path, out var texture)) return texture;

        texture = LoadTexture(Path.Join(Root, path));

        Textures[path] = texture;
        return texture;
    }

    public static Material GetTextureMaterial(string path)
    {
        if (Materials.TryGetValue(path, out var material)) return material;

        material = LoadMaterialDefault();
        SetMaterialTexture(ref material, MaterialMapIndex.Albedo, GetTexture(path));
        Materials[path] = material;

        return material;
    }

    public static string GetText(string path)
    {
        if (!File.Exists(path))
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