using System.Numerics;
using Serilog;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SharpCraft.Framework;

public sealed class Mesh(GL gl) : IDisposable
{
    public uint Vao;
    private uint _vbo;
    private uint _ebo;

    private int _indices;

    public unsafe void Upload(Shader shader, Span<Vertex> vertexBuffer, Span<ushort> indexBuffer)
    {
        _indices = indexBuffer.Length;

        Vao = gl.GenVertexArray();
        _vbo = gl.GenBuffer();
        _ebo = gl.GenBuffer();

        gl.BindVertexArray(Vao);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        gl.BufferData<Vertex>(BufferTargetARB.ArrayBuffer, vertexBuffer, GLEnum.StaticDraw);

        var vertPos = shader.GetAttribLocation("vertPos");
        var vertTexCoord = shader.GetAttribLocation("vertTexCoord");
        var vertColor = shader.GetAttribLocation("vertColor");

        gl.VertexAttribPointer(
            (uint)vertPos,
            3,
            VertexAttribPointerType.Float,
            false,
            (uint)sizeof(Vertex),
            (void*)0
        );
        gl.EnableVertexAttribArray((uint)vertPos);

        gl.VertexAttribPointer(
            (uint)vertTexCoord,
            2,
            VertexAttribPointerType.Float,
            false,
            (uint)sizeof(Vertex),
            (void*)(3 * sizeof(float))
        );
        gl.EnableVertexAttribArray((uint)vertTexCoord);

        gl.VertexAttribPointer(
            (uint)vertColor,
            4,
            VertexAttribPointerType.UnsignedByte,
            true,
            (uint)sizeof(Vertex),
            (void*)(5 * sizeof(float))
        );
        gl.EnableVertexAttribArray((uint)vertColor);

        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        gl.BufferData<ushort>(BufferTargetARB.ElementArrayBuffer, indexBuffer, BufferUsageARB.StaticDraw);

        gl.BindVertexArray(0);

        Log.Information("Loaded mesh! VAO: {0}", Vao);
    }

    public unsafe void Draw(MatrixStack matrices, PrimitiveType mode, Material material, Matrix4x4 transform)
    {
        matrices.SetMode(MatrixMode.ModelView);
        
        matrices.Push();
        
        matrices.MultMatrix(transform);

        material.Shader.Use();
        material.Shader.SetUniform("mvp", matrices.ModelView * matrices.Projection);
        
        material.Texture.Bind();

        gl.BindVertexArray(Vao);

        gl.DrawElements(
            mode,
            (uint)(_indices),
            DrawElementsType.UnsignedShort,
            (void*)0
        );
        
        gl.BindVertexArray(0);
        
        material.Texture.Unbind();
        gl.UseProgram(0);

        matrices.Pop();
    }

    public void Dispose()
    {
        gl.DeleteVertexArray(Vao);
        gl.DeleteBuffer(_vbo);
        gl.DeleteBuffer(_ebo);
        Log.Information("Deleted mesh! VAO: {0}", Vao);
    }
}