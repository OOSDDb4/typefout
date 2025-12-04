using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class SentenceViewModel : ObservableObject
    {
        private readonly IAiService _aiService;

        private int _index = 0;
        private const int _exerciseLength = 5;

        [ObservableProperty] private string _targetText;
        [ObservableProperty] private string _inputText;
        [ObservableProperty] private FormattedString _highlightedText;

        public SentenceViewModel(IAiService aiService)
        {
            _aiService = aiService;
            NextSentence();
        }

        partial void OnInputTextChanged(string value)
        {
            HighlightErrors();

            if (!string.IsNullOrEmpty(value) && value == TargetText)
            {
                _index++;

                if (_index >= _exerciseLength)
                {
                    ShowCompletionPopup();
                    return;
                }

                NextSentence();
            }
        }

        private async void ShowCompletionPopup()
        {
            await Shell.Current.DisplayAlert("Klaar!", "Je hebt alle zinnen getypt!", "OK");
            await Shell.Current.Navigation.PopToRootAsync();
        }

        private void HighlightErrors()
        {
            FormattedString formatted = new FormattedString();

            for (int i = 0; i < _inputText.Length; i++)
            {
                char typedChar = _inputText[i];

                char correctChar = i < TargetText.Length ? TargetText[i] : '?';

                bool isCorrect = typedChar == correctChar;

                formatted.Spans.Add(new Span
                {
                    Text = typedChar.ToString(),
                    TextColor = isCorrect ? Colors.Black : Colors.Red,
                    FontSize = 18
                });
            }

            formatted.Spans.Add(new Span
            {
                Text = "|",
                TextColor = Colors.Black,
                FontSize = 18
            });

            HighlightedText = formatted;
        }

        [RelayCommand]
        private async void NextSentence()
        {
            InputText = string.Empty;

            TypingExerciseText newSentence = await _aiService.GetExerciseTextAsync("sentence");
            TargetText = newSentence.Text;

            HighlightedText = new FormattedString();

            HighlightedText.Spans.Add(new Span
            {
                Text = "|",
                TextColor = Colors.Black,
                FontSize = 18
            });
        }
    }
}
