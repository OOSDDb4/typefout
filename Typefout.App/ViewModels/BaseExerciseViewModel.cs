
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.App.Views;
using Typefout.Core.Interfaces;

namespace Typefout.App.ViewModels
{
    public partial class BaseExerciseViewModel : ObservableObject
    {
        [ObservableProperty] private string _targetText;
        [ObservableProperty] private string _inputText;
        [ObservableProperty] private FormattedString _highlightedText;
        [ObservableProperty] private string _timerText;

        protected readonly IAiService _aiService;
        protected readonly IKeyTrackingService _trackingService;
        protected readonly ITimerService _timerService;
        protected int _previousLength = 0;
        protected int _index = 0;
        public int ExerciseLength { get; protected set; }
        public int ExerciseTime { get; protected set; } // seconds
        public BaseExerciseViewModel(
            IAiService aiService,
            IKeyTrackingService trackingService,
            ITimerService timerService,
            int exerciseLength,
            int exerciseTime)
        {
            _aiService = aiService;
            ExerciseLength = exerciseLength;
            ExerciseTime = exerciseTime;
            _trackingService = trackingService;
            _trackingService.Reset();

            _timerService = timerService;
            _timerService.Tick += UpdateTimerText;
            _timerService.Finished += OnTimerFinished;
            _timerService.Set(exerciseTime);
            _previousLength = 0;
        }
        protected virtual void OnCorrectInput() { }
        protected virtual void HighlightErrors(bool lengthIncreased) { }
        protected virtual bool AnyProgressMade() { return false; }
        private void UpdateTimerText(object? sender, EventArgs eventArgs)
        {
            TimerText = _timerService.TimeLeftToString();
        }
        private void OnTimerFinished(object? sender, EventArgs eventArgs)
        {
            // ShowResults();
            MainThread.BeginInvokeOnMainThread(TimeUp);
        }
        private async void ExerciseFinished()
        {
            _timerService.Stop();
            await Shell.Current.DisplayAlert("Klaar!", "Je hebt alle woorden getypt!", "OK");
            ShowResults();
            _timerService.Dispose();
        }
        private async void TimeUp()
        {
            await Shell.Current.DisplayAlert("Tijd op!", "De tijd is op!", "OK");
            ShowResults();
        }
        protected static async void ShowResults()
        {
            // ResultsViewModel vm = new(_trackingService, _timerService, _userRepo, 
            //     _authService);
            ResultsViewModel vm = App.Services.GetRequiredService<ResultsViewModel>();
            await Shell.Current.Navigation.PushAsync(new ResultsPage(vm));
        }
        partial void OnInputTextChanged(string value)
        {
            bool lengthIncreased = !string.IsNullOrEmpty(value) && value.Length > _previousLength;

            _previousLength = value?.Length ?? 0;

            HighlightErrors(lengthIncreased);

            OnProgress(value);
        }
        private void OnProgress(string value)
        {
            if (string.IsNullOrEmpty(value) || value != TargetText)
                return;

            _index++;
            if (_index >= ExerciseLength)
            {
                ExerciseFinished();
                return;
            }

            OnCorrectInput();
        }
        [RelayCommand]
        private async Task StopExercise()
        {
            _timerService.Stop();
            bool answer =
                await Shell.Current.DisplayAlert("Stoppen", "Weet je zeker dat je wilt stoppen?", "Ja", "Nee");
            if (!answer)
            {
                _timerService.Start();
                return;
            }

            if (AnyProgressMade())
            {
                ShowResults();
                _timerService.Dispose();
            }
            else
            {
                _timerService.Stop();
                _timerService.Dispose();
                await Shell.Current.Navigation.PopAsync();
            }
        }
    }
}