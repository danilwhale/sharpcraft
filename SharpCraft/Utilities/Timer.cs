﻿using System.Diagnostics;

namespace SharpCraft.Utilities;

public sealed class Timer(float ticksPerSecond)
{
    private const long NanosecondsPerSecond = TimeSpan.TicksPerSecond * TimeSpan.NanosecondsPerTick;
    private const int MaxTicks = 100;
    
    private const float TimeScale = 1.0f;
    
    public int Ticks { get; private set; }
    public float DeltaTime { get; private set; }
    public float LastDeltaTime { get; private set; }

    private long _lastTime = Stopwatch.GetTimestamp() * TimeSpan.NanosecondsPerTick;

    public void Advance()
    {
        var now = Stopwatch.GetTimestamp() * TimeSpan.NanosecondsPerTick;
        var passedNs = Math.Clamp(now - _lastTime, 0, NanosecondsPerSecond);
        _lastTime = now;

        passedNs = Math.Clamp(passedNs, 0, NanosecondsPerSecond);

        DeltaTime += passedNs * ticksPerSecond * TimeScale / NanosecondsPerSecond;
        Ticks = Math.Min(MaxTicks, (int)DeltaTime);
        DeltaTime -= Ticks;

        LastDeltaTime = DeltaTime;
    }
}