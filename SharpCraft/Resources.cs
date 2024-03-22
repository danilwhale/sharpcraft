namespace SharpCraft;

public static class Resources
{
    public static Material DefaultTerrainMaterial;

    public static void Load()
    {
        DefaultTerrainMaterial = LoadMaterialDefault();
        SetMaterialTexture(ref DefaultTerrainMaterial, MaterialMapIndex.Albedo, ResourceManager.GetTexture("terrain.png"));
    }

    public static void Unload()
    {
        UnloadMaterial(DefaultTerrainMaterial);
    }
}