using Typefout.Core.Interfaces;
using Timer = System.Timers.Timer;

namespace Typefout.Core.Data.Services;

public class TimerService() : ITimerService, IDisposable
{
    private Timer? _timer;
    public TimeSpan? StartTime { get; private set; }
    public TimeSpan? RemainingTime { get; private set; }
    private bool _disposed;
    public event EventHandler? Tick;
    public event EventHandler? Finished;

    public void Set(int timerLength)
    {
        _timer = new Timer();
        StartTime = TimeSpan.FromSeconds(timerLength);
        RemainingTime = StartTime.Value;
        _timer.Elapsed += OnTimerTick;
        Finished += OnFinished;
        _timer.Interval = 1000; // 1 second
    }
    public void Start()
    {
        if (StartTime == null)
        {
            return;
        }
        _timer!.Start();
        Tick?.Invoke(this, EventArgs.Empty);
    }
    public void Stop()
    {
        _timer!.Stop();
    }
    private void OnTimerTick(object? sender, EventArgs e)
    {
        RemainingTime -= TimeSpan.FromSeconds(1);

        if (RemainingTime.HasValue && RemainingTime.Value.TotalSeconds >= 0.5)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
    private void OnFinished(object? sender, EventArgs e)
    {
        Stop();
        Dispose();
    }

    public string TimeUsedToString()
    {
        if (!RemainingTime.HasValue || !StartTime.HasValue)
        {
            return "time not set";
        }
        TimeSpan timeUsed = StartTime.Value.Subtract(RemainingTime.Value);
        return timeUsed.ToString(@"mm\:ss");

    }

    public string TimeLeftToString()
    {
        if (RemainingTime.HasValue)
        {
            return RemainingTime.Value.ToString(@"mm\:ss");
        }
        return "time not set";
    }

    public void Dispose()
    {
        if (_disposed) return;
        _timer?.Dispose();
        _disposed = true;
    }

}