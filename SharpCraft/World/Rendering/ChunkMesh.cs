namespace SharpCraft.World.Rendering;

public readonly struct ChunkMesh : IDisposable
{
    private const uint PositionLoc = 0;
    private const uint TexCoordLoc = 1;
    private const uint LightLoc = 2;

    public readonly uint Vao;
    public readonly uint Vbo;
    public readonly uint Ebo;

    private readonly int _indicesCount;

    public unsafe ChunkMesh(Span<ChunkVertex> vertexBuffer, Span<ushort> indexBuffer)
    {
        Vao = Rlgl.LoadVertexArray();
        Rlgl.EnableVertexArray(Vao);

        fixed (ChunkVertex* pVertexBuffer = vertexBuffer)
        {
            Vbo = Rlgl.LoadVertexBuffer(pVertexBuffer, vertexBuffer.Length * sizeof(ChunkVertex), false);
        }

        SetupAttributes();

        fixed (ushort* pIndexBuffer = indexBuffer)
        {
            Ebo = Rlgl.LoadVertexBufferElement(pIndexBuffer, indexBuffer.Length * sizeof(ushort), false);
        }

        _indicesCount = indexBuffer.Length;

        Rlgl.DisableVertexArray();
    }

    private static unsafe void SetupAttributes()
    {
        var stride = sizeof(ChunkVertex);

        Rlgl.SetVertexAttribute(
            PositionLoc,
            3,
            Rlgl.FLOAT,
            false,
            stride,
            (void*)0
        );
        Rlgl.EnableVertexAttribute(PositionLoc);

        Rlgl.SetVertexAttribute(
            TexCoordLoc,
            2,
            Rlgl.FLOAT,
            false,
            stride,
            (void*)(3 * sizeof(float))
        );
        Rlgl.EnableVertexAttribute(TexCoordLoc);

        Rlgl.SetVertexAttribute(
            LightLoc,
            1,
            Rlgl.FLOAT,
            false,
            stride,
            (void*)(5 * sizeof(float))
        );
        Rlgl.EnableVertexAttribute(LightLoc);
    }

    public unsafe void Draw(Material material)
    {
        Rlgl.EnableShader(material.Shader.Id);

        // shader uses only diffuse map
        var colorDiffuseIndex = material.Shader.Locs[(int)ShaderLocationIndex.ColorDiffuse];
        if (colorDiffuseIndex != -1)
        {
            var map = material.Maps[(int)MaterialMapIndex.Diffuse];
            var color = stackalloc float[4]
            {
                map.Color.R / 255.0f,
                map.Color.G / 255.0f,
                map.Color.B / 255.0f,
                map.Color.A / 255.0f
            };

            Rlgl.SetUniform(colorDiffuseIndex, color, (int)ShaderUniformDataType.Vec4, 1);
        }

        // calculate matrices right now, game is not in vr
        var model = Rlgl.GetMatrixTransform();
        var view = Rlgl.GetMatrixModelview();
        var projection = Rlgl.GetMatrixProjection();
        var mvp = Raymath.MatrixMultiply(Raymath.MatrixMultiply(model, view), projection);

        var modelIndex = material.Shader.Locs[(int)ShaderLocationIndex.MatrixModel];
        var viewIndex = material.Shader.Locs[(int)ShaderLocationIndex.MatrixView];
        var projectionIndex = material.Shader.Locs[(int)ShaderLocationIndex.MatrixProjection];
        var mvpIndex = material.Shader.Locs[(int)ShaderLocationIndex.MatrixMvp];

        if (modelIndex != -1) Rlgl.SetUniformMatrix(modelIndex, model);
        if (viewIndex != -1) Rlgl.SetUniformMatrix(viewIndex, view);
        if (projectionIndex != -1) Rlgl.SetUniformMatrix(projectionIndex, projection);
        if (mvpIndex != -1) Rlgl.SetUniformMatrix(mvpIndex, mvp);

        // bind only diffuse, we don't use anything else, no need to bind every possible material map
        var diffuseTextureId = material.Maps[(int)MaterialMapIndex.Diffuse].Texture.Id;
        if (diffuseTextureId != 0)
        {
            Rlgl.ActiveTextureSlot(0);
            Rlgl.EnableTexture(diffuseTextureId);
        }

        // finally we can draw mesh!
        Rlgl.EnableVertexArray(Vao);
        
        Rlgl.DrawVertexArrayElements(0, _indicesCount, (void*)0);
        
        // now unbind everything we bound before
        Rlgl.DisableVertexArray();
        Rlgl.DisableVertexBuffer();
        Rlgl.DisableVertexBufferElement();

        if (diffuseTextureId != 0)
        {
            Rlgl.ActiveTextureSlot(0);
            Rlgl.DisableTexture();
        }

        Rlgl.DisableShader();
    }

    public void Dispose()
    {
        Rlgl.UnloadVertexArray(Vao);
        Rlgl.UnloadVertexBuffer(Vbo);
        Rlgl.UnloadVertexBuffer(Ebo);
    }
}