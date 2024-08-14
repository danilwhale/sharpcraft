using System.Numerics;
using Serilog;
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
    private static readonly int ViewPosLoc;

    static WorldShader()
    {
        IsLitLoc = GetShaderLocation(Shader, "isLit");
        ChunkIsLitLoc = GetShaderLocation(ChunkShader, "isLit");
        ViewPosLoc = GetShaderLocation(ChunkShader, "viewPos");
    }

    public static void SetIsLit(bool isLit)
    {
        SetShaderValue(ChunkShader, ChunkIsLitLoc, isLit ? 1 : 0, ShaderUniformDataType.Int);
        SetShaderValue(Shader, IsLitLoc, isLit ? 1 : 0, ShaderUniformDataType.Int);
    }

    public static void SetViewPos(Vector3 viewPos)
    {
        SetShaderValue(ChunkShader, ViewPosLoc, viewPos, ShaderUniformDataType.Vec3);
        SetShaderValue(Shader, ViewPosLoc, viewPos, ShaderUniformDataType.Vec3);
    }

    public static unsafe void UpdateWorldModelMatrix()
    {
        SetShaderValueMatrix(Shader, Shader.Locs[(int)ShaderLocationIndex.MatrixModel], Rlgl.GetMatrixTransform());
    }
}