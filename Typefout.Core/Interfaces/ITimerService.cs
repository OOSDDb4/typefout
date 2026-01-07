
namespace Typefout.Core.Interfaces;

public interface ITimerService
{
    event EventHandler Tick;
    event EventHandler Finished;
    public void Set(int timerLength);
    public void Start();
    public void Stop();
    public void Dispose();
    public string TimeUsedToString();
    public string TimeLeftToString();
}