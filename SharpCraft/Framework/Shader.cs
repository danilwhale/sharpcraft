using System.Numerics;
using Serilog;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SharpCraft.Framework;

public readonly struct Shader : IDisposable
{
    private readonly uint _program;
    private readonly GL _gl;

    public Shader(GL gl, string vertexCode, string fragmentCode)
    {
        _gl = gl;

        var vertex = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertex, vertexCode);
        gl.CompileShader(vertex);
        
        if (gl.GetShader(vertex, ShaderParameterName.CompileStatus) == 0)
        {
            Log.Error("Failed to compile vertex shader: {0}", gl.GetShaderInfoLog(vertex));
            return;
        }

        var fragment = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(fragment, fragmentCode);
        gl.CompileShader(fragment);
        
        if (gl.GetShader(fragment, ShaderParameterName.CompileStatus) == 0)
        {
            Log.Error("Failed to compile fragment shader: {0}", gl.GetShaderInfoLog(fragment));
            return;
        }

        _program = gl.CreateProgram();
        gl.AttachShader(_program, vertex);
        gl.AttachShader(_program, fragment);
        gl.LinkProgram(_program);
        
        if (gl.GetProgram(_program, ProgramPropertyARB.LinkStatus) == 0)
        {
            Log.Error("Failed to link shader program: {0}", gl.GetProgramInfoLog(_program));
            return;
        }
        
        gl.DeleteShader(vertex);
        gl.DeleteShader(fragment);
        
        Log.Information("Loaded shader program! ID: {0}", _program);
    }

    public void Use()
    {
        _gl.UseProgram(_program);
    }

    public int GetUniformLocation(string uniformName)
    {
        return _gl.GetUniformLocation(_program, uniformName);
    }

    public int GetAttribLocation(string attribName)
    {
        return _gl.GetAttribLocation(_program, attribName);
    }

    public unsafe void SetUniform(string uniformName, Matrix4x4 matrix)
    {
        var uniformLocation = GetUniformLocation(uniformName);
        if (uniformLocation < 0)
        {
            Log.Warning("Failed to find {0} in shader program! ID: {1}", uniformName, _program);
            return;
        }
        
        _gl.UniformMatrix4(uniformLocation, 1, false, (float*)&matrix);
    }
    
    public void Dispose()
    {
        _gl.DeleteProgram(_program);
        Log.Information("Deleted shader program! ID: {0}", _program);
    }
}