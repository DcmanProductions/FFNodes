// LFInteractive LLC. - All Rights Reserved

namespace FFNodes.Core.Utilities;

public class AdvancedTimer : System.Timers.Timer
{
    private readonly double _duration;
    private long _start;

    public AdvancedTimer(double duration) : base(duration)
    {
        _duration = duration;
        Enabled = true;
        Elapsed += (s, e) =>
        {
            if (!AutoReset) Stop();
            _start = DateTime.Now.Ticks;
        };
    }

    public AdvancedTimer(TimeSpan duration) : this(duration.TotalMilliseconds)
    {
    }

    public bool Interuptable { get; init; }
    public bool IsRunning { get; private set; }

    public DateTime GetEndTime()
    {
        if (!IsRunning) return DateTime.Now;
        return new DateTime(_start).AddMilliseconds(_duration);
    }

    public double GetRemaining()
    {
        if (!IsRunning) return 0d;
        return GetRemainingTime().TotalMilliseconds;
    }

    public TimeSpan GetRemainingTime()
    {
        if (!IsRunning) return TimeSpan.Zero;
        return new((GetEndTime() - DateTime.Now).Ticks);
    }

    public void Reset()
    {
        Stop();
        Start();
    }

    public new void Start()
    {
        if (Interuptable && IsRunning)
        {
            Stop();
        }
        else if (!Interuptable && IsRunning)
        {
            throw new InvalidOperationException("Timer is already running!");
        }
        IsRunning = true;
        _start = DateTime.Now.Ticks;
        base.Start();
    }

    public new void Stop()
    {
        _start = 0;
        IsRunning = false;
        base.Stop();
    }

    public override string ToString()
    {
        if (!IsRunning) return System.Text.Json.JsonSerializer.Serialize(this);
        return GetRemainingTime().ToString();
    }
}