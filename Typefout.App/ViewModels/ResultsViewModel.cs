using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class ResultsViewModel : ObservableObject
    {
        private readonly IKeyTrackingService _trackingService;
        private readonly ITimerService _timerService;
        [ObservableProperty] private List<KeyStat> _worstKeys;
        [ObservableProperty] private List<KeyStat> _bestKeys;
        [ObservableProperty] private int _totalMistakes;
        [ObservableProperty] private string _timeUsed;
        [ObservableProperty] private string _timeLeft;
        private const double _errorThreshold = 0.15;

        public ResultsViewModel(IKeyTrackingService trackingService, ITimerService timerService)
        {
            _trackingService = trackingService;
            _timerService = timerService;
            LoadStats();
        }
        private void LoadStats()
        {
            List<KeyStat> stats = _trackingService.GetStats()
                .Where(s => s.Attempts >= 2)
                .ToList();

            _worstKeys = stats
                .Where(s => s.ErrorRate >= _errorThreshold)
                .OrderByDescending(s => s.Attempts)
                .ToList();

            _bestKeys = stats
                .Where(s => s.ErrorRate < _errorThreshold)
                .OrderByDescending(s => s.Attempts)
                .Take(5)
                .ToList();

            _totalMistakes = _worstKeys.Sum(k => (int)(k.Attempts * k.ErrorRate));
            TimeUsed = _timerService.TimeUsedToString();
            TimeLeft = _timerService.TimeLeftToString();
        }

        [RelayCommand]
        private async Task RestartExercise()
        {
            await Shell.Current.GoToAsync("//TypeView");
        }
    }
}