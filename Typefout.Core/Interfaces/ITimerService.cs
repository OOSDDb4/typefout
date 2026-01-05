
namespace Typefout.Core.Interfaces;

public interface ITimerService
{
    event EventHandler Tick;
    event EventHandler Finished;
    public void Start();
    public string TimeToString();
}