using Typefout.Core.Interfaces;
using Timer = System.Timers.Timer;

namespace Typefout.Core.Data.Services;

public class TimerService() : ITimerService
{
    private readonly Timer _timer = new();
    private TimeSpan? _startTime;
    private TimeSpan? _remainingTime;
    public event EventHandler? Tick;
    public event EventHandler? Finished;

    public void Set(int timerLength)
    {
        if (_startTime.HasValue)
            throw new InvalidOperationException("StartTime already set");

        _startTime = TimeSpan.FromSeconds(timerLength);
        _remainingTime =  _startTime.Value;
    }
    public void Start()
    {
        if (_startTime == null)
        {
            return;
        }
        _timer.Interval = 1000; // 1 second
        _timer.Elapsed += OnTimerTick;
        _timer.Start();
        Tick?.Invoke(this, EventArgs.Empty);
        Finished += OnFinished;
    }
    public void Stop()
    {
        _timer.Stop();
    }
    private void OnTimerTick(object? sender, EventArgs e)
    {
        _remainingTime -= TimeSpan.FromSeconds(1);

        if (_remainingTime.HasValue && _remainingTime.Value.TotalSeconds > 0)
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
        if (!_remainingTime.HasValue || !_startTime.HasValue)
        {
            return "time not set";
        }
        TimeSpan timeUsed = _startTime.Value.Subtract(_remainingTime.Value);
        return timeUsed.ToString(@"mm\:ss");

    }

    public string TimeLeftToString()
    {
        if (_remainingTime.HasValue)
        {
            return _remainingTime.Value.ToString(@"mm\:ss");
        }
        return "time not set";
            
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

}