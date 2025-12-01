using CommunityToolkit.Mvvm.ComponentModel;
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
            FormattedString formatted = new();

            if (string.IsNullOrEmpty(InputText))
            {
                HighlightedText = formatted;
                return;
            }

            for (int i = 0; i < InputText.Length; i++)
            {
                char typedChar = InputText[i];
                char correctChar = i < TargetText.Length ? TargetText[i] : '?';

                formatted.Spans.Add(new Span
                {
                    Text = typedChar.ToString(),
                    TextColor = typedChar == correctChar ? Colors.Black : Colors.Red
                });
            }

            HighlightedText = formatted;
        }

        private async void NextSentence()
        {
            InputText = string.Empty;

            TypingExerciseText newSentence = await _aiService.GetExerciseTextAsync("sentence");
            TargetText = newSentence.Text;

            HighlightedText = new FormattedString();
        }
    }
}
