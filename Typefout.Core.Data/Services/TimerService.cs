
using System.Timers;
using Typefout.Core.Interfaces;
using Timer = System.Timers.Timer;

namespace Typefout.Core.Data.Services;

public class TimerService(int timerLength) : ITimerService
{
    private Timer _timer = new();
    private TimeSpan _remainingTime = TimeSpan.FromSeconds(timerLength);
    public event EventHandler? Tick;
    public event EventHandler? Finished;

    public void Start()
    {
        _timer.Stop();
        _timer = new Timer();
        _timer.Interval = 1000; // 1 second
        _timer.Elapsed += OnTimerTick;
        _timer.Start();
    }
    private void OnTimerTick(object? sender, EventArgs e)
    {
        _remainingTime -= TimeSpan.FromSeconds(1);
        TimeToString();

        if (_remainingTime.TotalSeconds > 0)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _timer.Stop();
            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
    public string TimeToString()
    {
        return _remainingTime.ToString(@"mm\:ss");
    }
}