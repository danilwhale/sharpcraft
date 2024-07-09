using System.Numerics;
using SharpCraft.Entities;
using SharpCraft.Tiles;

namespace SharpCraft.Particles;

public sealed class Particle : Entity
{
    private readonly int _textureIndex;
    private readonly float _uOffset;
    private readonly float _vOffset;
    private readonly float _size;
    private readonly int _lifetimeTicks;
    private int _ageTicks;
    
    public Particle(Level.Level level, Vector3 position, Vector3 offset, int textureIndex) 
        : base(level, 0.2f, 0.2f)
    {
        _textureIndex = textureIndex;
        
        Width = 0.2f;
        Height = 0.2f;
        HeightOffset = Height * 0.5f;

        Position = position;
        
        Motion = offset + new Vector3(GetRandomMotion(), GetRandomMotion(), GetRandomMotion());

        var speed = (Random.Shared.NextSingle() + Random.Shared.NextSingle() + 1.0f) * 0.15f;
        var distance = Motion.Length();

        Motion = Motion / distance * speed * 0.4f;
        Motion.Y += 0.1f;

        _uOffset = Random.Shared.NextSingle() * 3.0f;
        _vOffset = Random.Shared.NextSingle() * 3.0f;
        _size = Random.Shared.NextSingle() * 0.5f + 0.5f;
        _lifetimeTicks = (int)(4.0f / (Random.Shared.NextSingle() * 0.9f + 0.1f));
    }

    public override void Tick()
    {
        base.Tick();

        if (_ageTicks++ >= _lifetimeTicks)
        {
            Destroy();
        }

        Motion.Y -= 0.04f;
        ApplyMotion(Motion);

        Motion *= 0.98f;

        if (!IsOnGround) return;
        Motion.X *= 0.7f;
        Motion.Z *= 0.7f;
    }

    public override void Draw(float lastPartTicks)
    {
        
    }

    public void Draw(float lastPartTicks, float xOff1, float zOff1, float yOff, float xOff2, float zOff2)
    {
        ApplyLighting(0.8f);
        
        var u0 = ((_textureIndex & 15) + _uOffset * 0.25f) * TileRender.TexFactor;
        var u1 = u0 + TileRender.TexFactor * 0.25f;
        var v0 = ((_textureIndex >> 4) + _vOffset * 0.25f) * TileRender.TexFactor;
        var v1 = v0 + TileRender.TexFactor * 0.25f;

        var size = _size * 0.1f;

        var interpolated = GetInterpolatedPosition(lastPartTicks);
        var (x, y, z) = (interpolated.X, interpolated.Y, interpolated.Z);

        var sXOff1 = xOff1 * size;
        var sZOff1 = zOff1 * size;
        var sYOff = yOff * size;
        var sXOff2 = xOff2 * size;
        var sZOff2 = zOff2 * size;
        
        Rlgl.TexCoord2f(u0, v1);
        Rlgl.Vertex3f(x - sXOff1 - sXOff2, y - sYOff, z - sZOff1 - sZOff2);
        
        Rlgl.TexCoord2f(u0, v0);
        Rlgl.Vertex3f(x - sXOff1 + sXOff2, y + sYOff, z - sZOff1 + sZOff2);
        
        Rlgl.TexCoord2f(u1, v0);
        Rlgl.Vertex3f(x + sXOff1 + sXOff2, y + sYOff, z + sZOff1 + sZOff2);
        
        Rlgl.TexCoord2f(u1, v1);
        Rlgl.Vertex3f(x + sXOff1 - sXOff2, y - sYOff, z + sZOff1 - sZOff2);
    }

    public override void Dispose()
    {
        
    }

    private static float GetRandomMotion()
    {
        return (Random.Shared.NextSingle() * 2.0f - 1.0f) * 0.4f;
    }
}