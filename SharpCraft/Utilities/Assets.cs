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

    // NOTE: file extension from `texturePath` gets removed: "terrain.png" -> "terrain"
    public static Material GetTextureMaterial(string texturePath)
    {
        var path = Path.GetFileNameWithoutExtension(texturePath);
        if (Materials.TryGetValue(path, out var material)) return material;

        material = LoadMaterialDefault();
        SetMaterialTexture(ref material, MaterialMapIndex.Albedo, GetTexture(texturePath));
        Materials[path] = material;

        return material;
    }

    public static void SetMaterial(string path, Material material)
    {
        Materials[path] = material;
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