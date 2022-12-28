// LFInteractive LLC. - All Rights Reserved

namespace FFNodes.Core.Utilities;

public class AdvancedTimer : System.Timers.Timer
{
    private readonly double _duration;
    private long _start;

    /// <summary>
    /// Creates an <see cref="AdvancedTimer"/> using number of milliseconds for the duration!
    /// </summary>
    /// <param name="duration">duration in milliseconds</param>
    public AdvancedTimer(double duration) : base(duration)
    {
        _duration = duration;
        base.Enabled = true;
        Elapsed += (s, e) =>
        {
            if (!AutoReset) Stop();
            _start = DateTime.Now.Ticks;
        };
    }

    /// <summary>
    /// Creates an <see cref="AdvancedTimer"/> using <seealso cref="TimeSpan"/> for the duration!
    /// </summary>
    /// <param name="duration"></param>
    public AdvancedTimer(TimeSpan duration) : this(duration.TotalMilliseconds)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="AdvancedTimer"/> is able to raise
    /// events at a defined interval. The default value is true and can't be changed!
    /// </summary>
    public new bool Enabled => base.Enabled;

    /// <summary>
    /// If the <see cref="Start()"/> can be run while <seealso cref="IsRunning"/> is true. <br/>
    /// This will restart the timer without raising the event!
    /// </summary>
    public bool Interuptable { get; init; }

    /// <summary>
    /// If the <see cref="AdvancedTimer"/> is currently running or not! <br/> This is set when
    /// running the <seealso cref="Start"/> and <seealso cref="Stop"/> methods
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets the time when the timer will elapse next!
    /// </summary>
    /// <returns></returns>
    public DateTime GetNextElapseTime()
    {
        if (!IsRunning) return DateTime.Now;
        return new DateTime(_start).AddMilliseconds(_duration);
    }

    /// <summary>
    /// Gets the number of milliseconds remaining until next elapse is triggered.
    /// </summary>
    /// <returns></returns>
    public double GetRemaining()
    {
        if (!IsRunning) return 0d;
        return GetRemainingTime().TotalMilliseconds;
    }

    /// <summary>
    /// Gets the time remaining until the next elapse is triggered
    /// </summary>
    /// <returns></returns>
    public TimeSpan GetRemainingTime()
    {
        if (!IsRunning) return TimeSpan.Zero;
        return new((GetNextElapseTime() - DateTime.Now).Ticks);
    }

    /// <summary>
    /// Restarts the <see cref="AdvancedTimer"/> without triggering the elapse event!
    /// </summary>
    public void Reset()
    {
        Stop();
        Start();
    }

    /// <summary>
    /// Starts the <see cref="AdvancedTimer"/>! <br/> This can also be used with <seealso
    /// cref="Interuptable"/>. <br/> If called while <seealso cref="IsRunning"/> is <b>true</b> and
    /// <seealso cref="Interuptable"/> is <b>false</b> exception is thrown.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Stops the <see cref="AdvancedTimer"/>!
    /// </summary>
    public new void Stop()
    {
        _start = 0;
        IsRunning = false;
        base.Stop();
    }

    /// <summary>
    /// If <see cref="IsRunning"/> returns the remaining time, otherwise returns json representation
    /// of this object!
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (!IsRunning) return System.Text.Json.JsonSerializer.Serialize(this);
        return GetRemainingTime().ToString();
    }
}