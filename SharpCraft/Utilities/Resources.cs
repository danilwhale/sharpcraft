using SharpCraft.Rendering;

namespace SharpCraft.Utilities;

public static class Resources
{
    public static Material DefaultTerrainMaterial;

    public static void Load()
    {
        DefaultTerrainMaterial = LoadMaterialDefault();
        SetMaterialTexture(ref DefaultTerrainMaterial, MaterialMapIndex.Albedo, ResourceManager.GetTexture("Terrain.png"));
        
        DefaultTerrainMaterial.Shader = ChunkShader.Shader;
    }

    public static void Unload()
    {
        UnloadMaterial(DefaultTerrainMaterial);
    }
}