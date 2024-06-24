using Serilog;
using Silk.NET.OpenGL;

namespace SharpCraft.Framework;

public readonly struct Texture : IDisposable
{
    private readonly uint _texture;
    private readonly GL _gl;

    public Texture(GL gl, Span<byte> data, uint width, uint height, PixelFormat format)
    {
        _gl = gl;
        _texture = gl.GenTexture();
        SetupData(data, width, height, format);
        Log.Information("Loaded {0}x{1}px texture! ID: {2}", width, height, _texture);
    }

    private unsafe void SetupData(Span<byte> data, uint width, uint height, PixelFormat format)
    {
        Bind();

        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        _gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);

        fixed (byte* pData = data)
        {
            _gl.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                InternalFormat.Rgba, 
                width, height, 
                0, 
                format,
                PixelType.UnsignedByte,
                pData
            );
        }
        
        _gl.GenerateMipmap(TextureTarget.Texture2D);

        Unbind();
    }

    public void Bind()
    {
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
    }

    public void Unbind()
    {
        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_texture);
        Log.Information("Deleted texture! ID: {0}", _texture);
    }
}