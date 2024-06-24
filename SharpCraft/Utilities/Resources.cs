using SharpCraft.Framework;

namespace SharpCraft.Utilities;

public static class Resources
{
    public static Material DefaultTerrainMaterial;

    public static void Load()
    {
        DefaultTerrainMaterial = new Material(
            ResourceManager.GetTexture("terrain.png"),
            ResourceManager.GetShader()
        );
    }

    public static void Unload()
    {
        DefaultTerrainMaterial.Dispose();
    }
}