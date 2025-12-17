using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.App.Views;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;


namespace Typefout.App.ViewModels
{
    
    public partial class WordViewModel : ObservableObject
    {
        private readonly IAiService _aiService;
        private readonly IKeyTrackingService _trackingService;
        
        private int _index = 0;
        private const int _exerciseLength = 10;
        private const int _exerciseTime = 30; // seconds
        private int _previousLength = 0;
        private readonly ITimerService _timerService = new TimerService(_exerciseTime);

        [ObservableProperty] private string _targetWord;
        [ObservableProperty] private string _inputText;
        [ObservableProperty] private bool _isCompleted;
        [ObservableProperty] private FormattedString _highlightedText;
        [ObservableProperty] private string _timerText = _exerciseTime.ToString(@"mm\:ss");

        public WordViewModel(IAiService aiService, IKeyTrackingService trackingService)
        {
            _aiService = aiService;
            _trackingService = trackingService;

            _trackingService.Reset();
            _index = 0;
            _previousLength = 0;
            NextWord();
            
            _timerService.Tick += UpdateTimerText;
            _timerService.Finished += OnTimerFinished;
            _timerService.Start();
        }
        partial void OnInputTextChanged(string value)
        {
            bool lengthIncreased = !string.IsNullOrEmpty(value) && value.Length > _previousLength;
            _previousLength = value?.Length ?? 0;

            HighlightErrors(lengthIncreased);

            if (!string.IsNullOrEmpty(value) && value == TargetWord)
            {
                _index++;

                if (_index >= _exerciseLength)
                {
                    ShowResults();
                    return;
                }
                NextWord();
            }
        }
        private void UpdateTimerText(object sender, EventArgs eventArgs)
        {
            TimerText = _timerService.TimeToString();
        }
        private void OnTimerFinished(object sender, EventArgs eventArgs)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ShowResults();
            });
            
        }
        private async void ShowResults()
        {
            await Shell.Current.DisplayAlert("Klaar!", "Je hebt alle woorden getypt!", "OK");

            ResultsViewModel vm = App.Services.GetRequiredService<ResultsViewModel>();
            await Shell.Current.Navigation.PushAsync(new ResultsPage(vm));
        }

        private void HighlightErrors(bool registerLastChar)
        {
            FormattedString formattedString = new FormattedString();

            if (string.IsNullOrEmpty(InputText))
            {
                HighlightedText = formattedString;
                return;
            }

            for (int i = 0; i < InputText.Length; i++)
            {
                char typedChar = InputText[i];
                char correctChar = i < TargetWord.Length ? TargetWord[i] : '?';

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
                    char correctChar = lastIndex < TargetWord.Length ? TargetWord[lastIndex] : '?';

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
        private async void NextWord()
        {
            InputText = string.Empty;
            IsCompleted = false;
            _previousLength = 0;

            TypingExerciseText text = await _aiService.GetExerciseTextAsync("word");
            TargetWord = text.Text;

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
            if (answer)
            {
                if (_index == 0)
                {
                    await Shell.Current.Navigation.PopAsync();
                    return;
                }
                else
                {
                    ResultsViewModel vm = App.Services.GetRequiredService<ResultsViewModel>();
                    await Shell.Current.Navigation.PushAsync(new ResultsPage(vm));
                }
            }
        }


    }
}
