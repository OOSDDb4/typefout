using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class ResultsViewModel : ObservableObject
    {
        private readonly IKeyTrackingService _trackingService;
        private readonly ITimerService _timerService;
        private readonly IUserRepo _userRepo;
        private readonly IAuthService _authService;
        [ObservableProperty] private List<KeyStat> _worstKeys;
        [ObservableProperty] private List<KeyStat> _bestKeys;
        [ObservableProperty] private int _totalMistakes;
        [ObservableProperty] private string _timeUsed;
        [ObservableProperty] private string _timeLeft;
        [ObservableProperty] private int _pointsFromAnswers;
        [ObservableProperty] private int _pointsFromTime;
        
        private const double _errorThreshold = 0.15;

        public ResultsViewModel(
            IKeyTrackingService trackingService, 
            ITimerService timerService, 
            IUserRepo userRepo, 
            IAuthService authService)
        {
            _userRepo = userRepo;
            _authService = authService;
            _trackingService = trackingService;
            _timerService = timerService;
            LoadStats();
        }
        private void LoadStats()
        {
            // var allStats = _trackingService.GetStats();
            List<KeyStat> stats = _trackingService.GetStats()
                .Where(s => s.Attempts >= 2)
                .ToList();

            WorstKeys = stats
                .Where(s => s.ErrorRate >= _errorThreshold)
                .OrderByDescending(s => s.Attempts)
                .ToList();

            BestKeys = stats
                .Where(s => s.ErrorRate < _errorThreshold)
                .OrderByDescending(s => s.Attempts)
                .Take(5)
                .ToList();

            TotalMistakes = WorstKeys.Sum(k => (int)(k.Attempts * k.ErrorRate));
            TimeUsed = _timerService.TimeUsedToString();
            TimeLeft = _timerService.TimeLeftToString();
            
            int attempts = _trackingService.TotalAttempts;
            int mistakes = _trackingService.TotalMistakes;
            int startTime = _timerService.StartTime.HasValue ? (int)_timerService.StartTime.Value.TotalSeconds : 0;
            int remainingTime = _timerService.RemainingTime.HasValue ? (int)_timerService.RemainingTime.Value.TotalSeconds : 0;

            Dictionary<string, int> points = GetPoints(attempts, mistakes, startTime, remainingTime);
            PointsFromAnswers = points["pointsFromAnswers"];
            PointsFromTime = points["pointsFromTime"];
            int pointsTotal = points["pointsTotal"];
            UpdateUserScoreAsync(pointsTotal);
        }
        public Dictionary<string, int> GetPoints(
            int attempts, int mistakes, int startTime, int remainingTime)
        {
            double correct = attempts - mistakes;

            int pointsFromAnswers = (int)Math.Round(correct / attempts * 100);

            int pointsFromTime = (int)Math.Round(
                (double)remainingTime / startTime * 100
            );
        
            int pointsTotal = (pointsFromAnswers + pointsFromTime);
            return new Dictionary<string, int>
            {
                ["pointsFromAnswers"] = pointsFromAnswers,
                ["pointsFromTime"] = pointsFromTime,
                ["pointsTotal"] = pointsTotal
            };
        }
        private async Task UpdateUserScoreAsync(int points)
        {
            try
            {
                User user = _authService.CurrentUser;
                await _userRepo.UpdateScore(user, points);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update user score: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RestartExercise()
        {
            await Shell.Current.GoToAsync("//TypeView");
        }
    }
}