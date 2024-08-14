namespace SharpCraft.Rendering.Meshes;

public unsafe readonly struct GenericMesh<TVertex> : IDisposable
    where TVertex : unmanaged, IVertex
{
    private const uint PositionLoc = 0;
    private const uint TexCoordLoc = 1;
    private const uint ColorLoc = 2;
    
    public readonly uint Vao;
    public readonly uint Vbo;
    public readonly uint Ebo;

    private readonly int _indicesCount;
    private readonly int _vertexCount;

    public GenericMesh(ReadOnlySpan<TVertex> vertexBuffer, ReadOnlySpan<ushort> indexBuffer)
    {
        Vao = Rlgl.LoadVertexArray();
        Rlgl.EnableVertexArray(Vao);

        fixed (TVertex* pVertexBuffer = vertexBuffer)
        {
            Vbo = Rlgl.LoadVertexBuffer(pVertexBuffer, vertexBuffer.Length * sizeof(TVertex), false);
        }

        _vertexCount = vertexBuffer.Length;

        SetupAttributes();

        if (!indexBuffer.IsEmpty)
        {
            fixed (ushort* pIndexBuffer = indexBuffer)
            {
                Ebo = Rlgl.LoadVertexBufferElement(pIndexBuffer, indexBuffer.Length * sizeof(ushort), false);
            }

            _indicesCount = indexBuffer.Length;
        }

        Rlgl.DisableVertexArray();
    }

    private static void SetupAttributes()
    {
        var stride = sizeof(TVertex);
    
        var attributes = TVertex.Attributes;
        
        // it's expected that vertex is implemented without duplicated attributes
        // and has all required attributes
        foreach (var attribute in attributes)
        {
            switch (attribute.Location)
            {
                case VertexAttributeLocation.Position:
                    Rlgl.SetVertexAttribute(
                        PositionLoc,
                        attribute.Size,
                        (int)attribute.Type,
                        attribute.Normalize,
                        stride,
                        (void*)attribute.OffsetBytes
                    );
                    Rlgl.EnableVertexAttribute(PositionLoc);
                    break;
                
                case VertexAttributeLocation.TexCoord:
                    Rlgl.SetVertexAttribute(
                        TexCoordLoc,
                        attribute.Size,
                        (int)attribute.Type,
                        attribute.Normalize,
                        stride,
                        (void*)attribute.OffsetBytes
                    );
                    Rlgl.EnableVertexAttribute(TexCoordLoc);
                    break;
                
                case VertexAttributeLocation.Color:
                    Rlgl.SetVertexAttribute(
                        ColorLoc,
                        attribute.Size,
                        (int)attribute.Type,
                        attribute.Normalize,
                        stride,
                        (void*)attribute.OffsetBytes
                    );
                    Rlgl.EnableVertexAttribute(ColorLoc);
                    break;
            }
        }
    }

    public void Draw(Material material)
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

        if (_indicesCount > 0)
        {
            Rlgl.DrawVertexArrayElements(0, _indicesCount, (void*)0);
        }
        else
        {
            Rlgl.DrawVertexArray(0, _vertexCount);
        }
        
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