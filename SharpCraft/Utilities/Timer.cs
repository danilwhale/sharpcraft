using System.Diagnostics;

namespace SharpCraft.Utilities;

public sealed class Timer(float ticksPerSecond)
{
    private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
    private const int MaxTicks = 100;
    private const float TimeScale = 1.0f;

    public int Ticks;
    private float _partialTicks;
    public float LastPartialTicks;

    private double _lastTime = Stopwatch.Elapsed.Ticks;

    public void Advance()
    {
        var now = Stopwatch.Elapsed.Ticks;
        var passedNs = Math.Clamp(now - _lastTime, 0, Stopwatch.Frequency);
        _lastTime = now;

        _partialTicks += (float)(passedNs * ticksPerSecond * TimeScale / Stopwatch.Frequency);
        Ticks = Math.Min(MaxTicks, (int)_partialTicks);
        _partialTicks -= Ticks;

        LastPartialTicks = _partialTicks;
    }
}