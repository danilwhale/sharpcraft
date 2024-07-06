using System.Diagnostics;

namespace SharpCraft.Utilities;

public sealed class Timer(float ticksPerSecond)
{
    private const long NanosecondsPerSecond = TimeSpan.TicksPerSecond * TimeSpan.NanosecondsPerTick;
    private const int MaxTicks = 100;
    
    private const float TimeScale = 1.0f;
    
    public int Ticks { get; private set; }
    public float PartialTicks { get; private set; }
    public float LastPartialTicks { get; private set; }

    private long _lastTime = Stopwatch.GetTimestamp() * TimeSpan.NanosecondsPerTick;

    public void Advance()
    {
        var now = Stopwatch.GetTimestamp() * TimeSpan.NanosecondsPerTick;
        var passedNs = Math.Clamp(now - _lastTime, 0, NanosecondsPerSecond);
        _lastTime = now;

        PartialTicks += passedNs * ticksPerSecond * TimeScale / NanosecondsPerSecond;
        Ticks = Math.Min(MaxTicks, (int)PartialTicks);
        PartialTicks -= Ticks;

        LastPartialTicks = PartialTicks;
    }
}