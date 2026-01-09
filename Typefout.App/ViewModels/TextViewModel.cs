using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.App.Views;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class TextViewModel : BaseExerciseViewModel
    {
        private bool _firstWordCompleted = false;
        private string FirstWord =>
            string.IsNullOrWhiteSpace(TargetText)
                ? ""
                : TargetText.Split(' ')[0];

        public TextViewModel(IAiService aiService, IKeyTrackingService trackingService, ITimerService timerService)
            :base(aiService, trackingService, timerService, exerciseLength: 1, exerciseTime: 180)
        {
        }
        public async Task InitializeAsync()
        {
            await LoadText();
            _timerService.Start();
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
        protected override void HighlightErrors(bool registerLastChar)
        {
            FormattedString formattedString = new FormattedString();

            for (int i = 0; i < InputText.Length; i++)
            {
                char typedChar = InputText[i];
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
        private async Task LoadText()
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
        protected override bool AnyProgressMade()
        {
            CheckFirstWordProgress();
            return _firstWordCompleted;
        }
    }
}