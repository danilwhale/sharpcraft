using SharpCraft.Utilities;

namespace SharpCraft.Rendering;

public static class ChunkShader
{
    public static readonly Shader Shader;
    private static readonly int _fogDensityLoc;
    private static readonly int _skyColorLoc;
    private static readonly int _chunkAppearSpeedLoc;
    private static readonly int _chunkTimeLoc;

    static unsafe ChunkShader()
    {
        Shader = LoadShaderFromMemory(
            ResourceManager.GetText("Chunk.vert"),
            ResourceManager.GetText("Chunk.frag")
        );

        Shader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(Shader, "viewPos");
        _fogDensityLoc = GetShaderLocation(Shader, "fogDensity");
        _skyColorLoc = GetShaderLocation(Shader, "skyColor");
        _chunkAppearSpeedLoc = GetShaderLocation(Shader, "chunkAppearSpeed");
        _chunkTimeLoc = GetShaderLocation(Shader, "chunkTime");
        
        SetFogDensity(0.015f);
        SetSkyColor(Color.SkyBlue);
        SetAppearSpeed(0.3f);
    }

    public static unsafe void Update(Player player)
    {
        // update fog view position
        var viewPosLoc = Shader.Locs[(int)ShaderLocationIndex.VectorView];
        SetShaderValue(Shader, viewPosLoc, player.Entity.Camera.Position, ShaderUniformDataType.Vec3);
    }

    public static void SetFogDensity(float density)
    {
        SetShaderValue(Shader, _fogDensityLoc, density, ShaderUniformDataType.Float);
    }

    public static void SetSkyColor(Color color)
    {
        SetShaderValue(Shader, _skyColorLoc, ColorNormalize(color), ShaderUniformDataType.Vec4);
    }

    public static void SetAppearSpeed(float speed)
    {
        SetShaderValue(Shader, _chunkAppearSpeedLoc, speed, ShaderUniformDataType.Float);
    }

    public static void SetTime(double time)
    {
        SetShaderValue(Shader, _chunkTimeLoc, (float)time, ShaderUniformDataType.Float);
    }
}