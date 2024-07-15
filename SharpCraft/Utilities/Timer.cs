using System.Diagnostics;

namespace SharpCraft.Utilities;

public sealed class Timer(float ticksPerSecond)
{
    private const long NanosecondsPerSecond = TimeSpan.TicksPerSecond * TimeSpan.NanosecondsPerTick;
    
    private const int MaxTicks = 100;
    private const float TimeScale = 1.0f;

    public int Ticks;
    private float _partialTicks;
    public float LastPartialTicks;

    private double _lastTime = GetTicks();

    public void Advance()
    {
        var now = GetTicks();
        var passedNs = Math.Clamp(now - _lastTime, 0, NanosecondsPerSecond);
        _lastTime = now;

        _partialTicks += (float)(passedNs * ticksPerSecond * TimeScale / NanosecondsPerSecond);
        Ticks = Math.Min(MaxTicks, (int)_partialTicks);
        _partialTicks -= Ticks;

        LastPartialTicks = _partialTicks;
    }

    private static double GetNanosecondsSinceStart()
    {
        return GetTime() * NanosecondsPerSecond;
    }
}