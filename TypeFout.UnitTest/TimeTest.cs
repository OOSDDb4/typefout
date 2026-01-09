using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using Xunit;
using Xunit.Abstractions;
namespace TypeFout.UnitTest;

public class TimeTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TimeTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TimerServiceTest()
    {
        
        // Arrange
        ITimerService timerService = new TimerService();
        TaskCompletionSource<bool> finished = new ();
        timerService.Set(3);
        timerService.Tick += (_, _) => _testOutputHelper.WriteLine(timerService.TimeLeftToString());
        
        timerService.Finished += (_, _) => finished.SetResult(true);
            

        
        // Act
        timerService.Start();
        Thread.Sleep(4000);

        // Assert
        Assert.Equal("00:00", timerService.TimeLeftToString());
    }
    [Fact]
    public async Task TimerAccuracyWithinAcceptableRange()
    {
        // Arrange
        
        int testTime = 10;
        double maxOffset = 0.1;
        
        ITimerService timerService = new TimerService();
        Stopwatch stopwatch = new();
        timerService.Set(testTime);
        

        TaskCompletionSource<bool> finished = new();
        timerService.Finished += (_, _) =>
        {
            stopwatch.Stop();
            finished.SetResult(true);
        };

        // Act
        timerService.Start();
        stopwatch.Start();
        await finished.Task;

        // Assert
        
        Assert.InRange(
            stopwatch.Elapsed.TotalSeconds, 
            testTime - maxOffset, 
            testTime + maxOffset
        );
        
    }
    [Fact]
    public void Sanity_Check_ShoudlAlwayPass()
    {
        // Arrange
        int a = 2;
        int b = 3;

        // Act
        int result = a + b;

        // Assert
        Assert.Equal(5, result);
    }
}