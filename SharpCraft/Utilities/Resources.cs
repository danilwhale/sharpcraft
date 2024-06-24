namespace SharpCraft.Utilities;

public static class Resources
{
    public static Material DefaultTerrainMaterial;

    public static void Load()
    {
        DefaultTerrainMaterial = LoadMaterialDefault();
        SetMaterialTexture(ref DefaultTerrainMaterial, MaterialMapIndex.Albedo, ResourceManager.GetTexture("Terrain.png"));

        var shader = LoadShaderFromMemory(null, ResourceManager.GetText("DiscardShader.frag"));
        DefaultTerrainMaterial.Shader = shader;
    }

    public static void Unload()
    {
        UnloadMaterial(DefaultTerrainMaterial);
    }
}