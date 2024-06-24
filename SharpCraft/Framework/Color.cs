using System.Runtime.InteropServices;

namespace SharpCraft.Framework;

[StructLayout(LayoutKind.Sequential, Size = 4)]
public struct Color(byte r, byte g, byte b, byte a)
{
    // used colors
    public static readonly Color SkyBlue = new(102, 191, 255, 255);

    // basic colors
    public static readonly Color White = new(255, 255, 255, 255);
    public static readonly Color Black = new(0, 0, 0, 255);
    public static readonly Color Blank = new(0, 0, 0, 0);
    
    public byte R = r, G = g, B = b, A = a;

    public Color(float r, float g, float b, float a)
        : this((byte)(r * 255.0f), (byte)(g * 255.0f), (byte)(b * 255.0f), (byte)(a * 255.0f))
    {
    }

    public readonly Color WithBrightness(float factor)
    {
        factor = Math.Clamp(factor, -1.0f, 1.0f);

        var r = (float)R;
        var g = (float)G;
        var b = (float)B;
        
        if (factor < 0)
        {
            factor = 1.0f + factor;
            r *= factor;
            g *= factor;
            b *= factor;
        }
        else
        {
            r = (255 - r) * factor + r;
            g = (255 - g) * factor + g;
            b = (255 - b) * factor + b;
        }

        return new Color((byte)r, (byte)g, (byte)b, A);
    }
}