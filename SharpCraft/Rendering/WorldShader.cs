using SharpCraft.Entities;
using SharpCraft.Utilities;

namespace SharpCraft.Rendering;

public static class WorldShader
{
    public static readonly Shader Shader = LoadShaderFromMemory(
        Assets.GetText("World.vert"),
        Assets.GetText("World.frag")
        );

    public static readonly Shader ChunkShader = LoadShaderFromMemory(
        Assets.GetText("Chunk.vert"),
        Assets.GetText("World.frag")
    );

    private static readonly int IsLitLoc;
    private static readonly int ChunkIsLitLoc;

    static WorldShader()
    {
        IsLitLoc = GetShaderLocation(Shader, "isLit");
        ChunkIsLitLoc = GetShaderLocation(ChunkShader, "isLit");
    }

    public static void SetIsLit(bool isLit)
    {
        SetShaderValue(Shader, IsLitLoc, isLit ? 1 : 0, ShaderUniformDataType.Int);
        SetShaderValue(ChunkShader, ChunkIsLitLoc, isLit ? 1 : 0, ShaderUniformDataType.Int);
    }
}