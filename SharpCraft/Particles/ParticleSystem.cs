using SharpCraft.Entities;
using SharpCraft.Level;
using SharpCraft.Utilities;

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

    public void Draw(Player player, float lastPartTicks)
    {
        var xOff1 = MathF.Cos(player.Yaw * DEG2RAD);
        var zOff1 = -MathF.Sin(player.Yaw * DEG2RAD);
        var yOff = MathF.Cos(player.Pitch * DEG2RAD);
        var xOff2 = -zOff1 * MathF.Sin(player.Pitch * DEG2RAD);
        var zOff2 = xOff1 * MathF.Sin(player.Pitch * DEG2RAD);
        
        Rlgl.Begin(DrawMode.Quads);
        Rlgl.SetTexture(Assets.GetTexture("terrain.png").Id);

        foreach (var particle in _particles)
        {
            particle.Draw(lastPartTicks, xOff1, zOff1, yOff, xOff2, zOff2);
        }
        
        Rlgl.SetTexture(Rlgl.GetTextureIdDefault());
        Rlgl.End();
    }
}