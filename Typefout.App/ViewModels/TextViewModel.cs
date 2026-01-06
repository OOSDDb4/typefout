using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Typefout.App.Views;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class TextViewModel : ObservableObject
    {
        private readonly IAiService _aiService;
        private readonly IKeyTrackingService _trackingService;
        private const int _exerciseTime = 30; // seconds
        private int _previousLength = 0;
        private bool _firstWordCompleted = false;
        private readonly ITimerService _timerService = new TimerService(_exerciseTime);

        [ObservableProperty] private string _targetText;
        [ObservableProperty] private string _inputText;
        [ObservableProperty] private FormattedString _highlightedText;
        [ObservableProperty] private string _timerText = _exerciseTime.ToString(@"mm\:ss");
        private string FirstWord =>
            string.IsNullOrWhiteSpace(TargetText)
                ? ""
                : TargetText.Split(' ')[0];
        public TextViewModel(IAiService aiService, IKeyTrackingService trackingService)
        {
            _aiService = aiService;
            _trackingService = trackingService;

            _trackingService.Reset();
            _previousLength = 0;

            LoadText();
            _timerService.Tick += UpdateTimerText;
            _timerService.Finished += OnTimerFinished;
            _timerService.Start();
        }
        private void UpdateTimerText(object sender, EventArgs eventArgs)
        {
            TimerText = _timerService.TimeToString();
        }
        private void OnTimerFinished(object sender, EventArgs eventArgs)
        {
            MainThread.BeginInvokeOnMainThread(ShowResults);
        }
        partial void OnInputTextChanged(string value)
        {
            bool lengthIncreased = !string.IsNullOrEmpty(value) && value.Length > _previousLength;
            _previousLength = value?.Length ?? 0;

            HighlightErrors(lengthIncreased);
            CheckFirstWordProgress();

            if (!string.IsNullOrEmpty(value) && value == TargetText)
            {
                ExerciseFinished();
            }
        }
        private void CheckFirstWordProgress()
        {
            if (string.IsNullOrEmpty(InputText))
                return;

            if (InputText.StartsWith(FirstWord))
            {
                _firstWordCompleted = true;
            }
        }
        private void ExerciseFinished()
        {
            _timerService.Stop();
            _timerService.Dispose();
            ShowResults();
        }
        private async void ShowResults()
        {
            await Shell.Current.DisplayAlert("Klaar!", "Je hebt de tekst getypt!", "OK");

            ResultsViewModel vm = App.Services.GetRequiredService<ResultsViewModel>();
            await Shell.Current.Navigation.PushAsync(new ResultsPage(vm));
        }

        private void HighlightErrors(bool registerLastChar)
        {
            FormattedString formattedString = new FormattedString();

            for (int i = 0; i < _inputText.Length; i++)
            {
                char typedChar = _inputText[i];
                char correctChar = i < TargetText.Length ? TargetText[i] : '?';

                Span span = new Span
                {
                    Text = typedChar.ToString(),
                    TextColor = typedChar == correctChar ? Colors.Black : Colors.Red
                };

                formattedString.Spans.Add(span);
            }

            if (registerLastChar)
            {
                int lastIndex = InputText.Length - 1;

                if (lastIndex >= 0)
                {
                    char typedChar = InputText[lastIndex];
                    char correctChar = lastIndex < TargetText.Length ? TargetText[lastIndex] : '?';

                    if (correctChar != '?')
                    {
                        _trackingService.RegisterResult(correctChar, typedChar);
                    }
                }
            }

            formattedString.Spans.Add(new Span
            {
                Text = "|",
                TextColor = Colors.Black,
                FontSize = 18
            });

            HighlightedText = formattedString;
        }

        [RelayCommand]
        private async void LoadText()
        {
            InputText = string.Empty;
            _previousLength = 0;

            TypingExerciseText text = await _aiService.GetExerciseTextAsync("text");
            TargetText = text.Text;

            HighlightedText = new FormattedString();

            HighlightedText.Spans.Add(new Span
            {
                Text = "|",
                TextColor = Colors.Black,
                FontSize = 18
            });
        }
        [RelayCommand]
        private async void StopExercise()
        {
            bool answer = await Shell.Current.DisplayAlert("Stoppen", "Weet je zeker dat je wilt stoppen?", "Ja", "Nee");
            if (!answer)
                return;

            if (_firstWordCompleted)
            {
                ResultsViewModel vm = App.Services.GetRequiredService<ResultsViewModel>();
                await Shell.Current.Navigation.PushAsync(new ResultsPage(vm));
            }
            else
            {
                await Shell.Current.Navigation.PopAsync();
            }
        }
    }
}