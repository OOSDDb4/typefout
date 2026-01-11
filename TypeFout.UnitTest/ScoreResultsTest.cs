using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Typefout.App.ViewModels;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Tests
{
    public class ResultsViewModelTests
    {
        [Fact]
        public void GetPoints_CalculatesCorrectly()
        {
            // Arrange
            Mock<IKeyTrackingService> mockTrackingService = new();
            mockTrackingService.Setup(t => t.GetStats())
                .Returns(new List<KeyStat>());

            mockTrackingService.Setup(t => t.TotalAttempts).Returns(10);
            mockTrackingService.Setup(t => t.TotalMistakes).Returns(5);

            ResultsViewModel vm = new(
                mockTrackingService.Object,
                Mock.Of<ITimerService>(),
                Mock.Of<IUserRepo>(),
                Mock.Of<IAuthService>()
            );

            int attempts = 10;
            int mistakes = 5;
            int startTime = 60;
            int remainingTime = 30;

            // Act
            Dictionary<string, int> pointsFromAnswers = vm.GetPoints(attempts, mistakes, startTime, remainingTime);

            // Assert
            Assert.Equal(100, pointsFromAnswers["pointsTotal"]);
            Assert.Equal(50, pointsFromAnswers["pointsFromAnswers"]);
            Assert.Equal(50, pointsFromAnswers["pointsFromTime"]);
        }
        [Fact]
        public async Task LoadStats_CallsUpdateScore()
        {
            // Arrange
            Mock<IKeyTrackingService> tracking = new();
            tracking.SetupGet(t => t.TotalAttempts).Returns(10);
            tracking.SetupGet(t => t.TotalMistakes).Returns(2);
            tracking.Setup(t => t.GetStats()).Returns(new List<KeyStat>());

            Mock<ITimerService> timer = new();
            timer.SetupGet(t => t.RemainingTime).Returns((TimeSpan?)null);

            Mock<IUserRepo> userRepo = new();
            userRepo.Setup(u => u.UpdateScore(It.IsAny<User>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            Mock<IAuthService> auth = new();
            auth.SetupGet(a => a.CurrentUser).Returns(new User());

            // Act
            ResultsViewModel vm = new(tracking.Object, timer.Object, userRepo.Object, auth.Object);

            await Task.Delay(100);

            // Assert
            userRepo.Verify(u => u.UpdateScore(It.IsAny<User>(), It.IsAny<int>()), Times.Once);
        }
    }
}
