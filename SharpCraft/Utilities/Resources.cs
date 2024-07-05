namespace SharpCraft.Utilities;

public static class Resources
{
    public static Material DefaultTerrainMaterial;
    public static Material CharMaterial;

    public static void Load()
    {
        DefaultTerrainMaterial = LoadTextureMaterial("terrain.png");
        CharMaterial = LoadTextureMaterial("char.png");
    }

    private static Material LoadTextureMaterial(string path)
    {
        var material = LoadMaterialDefault();
        SetMaterialTexture(ref material, MaterialMapIndex.Albedo, ResourceManager.GetTexture(path));
        return material;
    }

    public static void Unload()
    {
        UnloadMaterial(DefaultTerrainMaterial);
    }
}