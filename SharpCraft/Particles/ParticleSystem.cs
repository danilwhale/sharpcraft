using SharpCraft.Entities;
using SharpCraft.Rendering;
using SharpCraft.Utilities;
using SharpCraft.World.Rendering;

namespace SharpCraft.Particles;

public sealed class ParticleSystem
{
    private readonly List<Particle> _particles = [];
    
    public void Add(Particle particle)
    {
        _particles.Add(particle);
    }
    
    public void Tick()
    {
        for (var i = 0; i < _particles.Count; i++)
        {
            var particle = _particles[i];
            particle.Tick();

            if (!particle.IsDestroyed) continue;
            
            _particles.Remove(particle);
            i--;
        }
    }
    
    public void Draw(PlayerEntity player, float lastPartTicks, RenderLayer layer)
    {
        var xOff1 = MathF.Cos(player.Yaw * DEG2RAD);
        var zOff1 = -MathF.Sin(player.Yaw * DEG2RAD);
        var yOff = MathF.Cos(player.Pitch * DEG2RAD);
        var xOff2 = -zOff1 * MathF.Sin(player.Pitch * DEG2RAD);
        var zOff2 = xOff1 * MathF.Sin(player.Pitch * DEG2RAD);
        
        Rlgl.PushMatrix();
        
        Rlgl.Begin(DrawMode.Quads);
        Rlgl.SetTexture(Assets.GetTexture("terrain.png").Id);
        Rlgl.Color3f(0.8f, 0.8f, 0.8f);

        foreach (var particle in _particles)
        {
            if (particle.IsLit() ^ layer == RenderLayer.Lit)
            {
                continue;
            }
            
            particle.Draw(lastPartTicks, xOff1, zOff1, yOff, xOff2, zOff2);
            WorldShader.UpdateWorldModelMatrix();
        }
        
        Rlgl.SetTexture(Rlgl.GetTextureIdDefault());
        Rlgl.End();
        
        Rlgl.PopMatrix();
    }
}