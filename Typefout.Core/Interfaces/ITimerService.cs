
namespace Typefout.Core.Interfaces;

public interface ITimerService
{
    public TimeSpan? StartTime { get; }
    public TimeSpan? RemainingTime { get; }
    event EventHandler Tick;
    event EventHandler Finished;
    public void Set(int timerLength);
    public void Start();
    public void Stop();
    public void Dispose();
    public string TimeUsedToString();
    public string TimeLeftToString();
}