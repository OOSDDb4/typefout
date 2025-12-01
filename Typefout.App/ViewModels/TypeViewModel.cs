using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;


namespace Typefout.App.ViewModels
{
    public partial class TypeViewModel : ObservableObject
    {
        private readonly IAIService _aIService;

        [ObservableProperty]
        private string _targetWord;

        [ObservableProperty]
        private string _inputText;

        [ObservableProperty]
        private bool _isCompleted;

        [ObservableProperty]
        private FormattedString _highlightedText;

        public TypeViewModel(IAIService AIService)
        {
            _aIService = AIService;
            NextWord();
        }

        partial void OnInputTextChanged(string value)
        {
            WrongInputHighlight();

            if (!string.IsNullOrEmpty(value) && value == TargetWord)
            {
                IsCompleted = true;
                NextWord();
            }
        }

        private void WrongInputHighlight()
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

            HighlightedText = formattedString;
        }

        [RelayCommand]
        private async void NextWord()
        {
            InputText = string.Empty;
            IsCompleted = false;

            TypingExerciseText newWord = await _aIService.GetExerciseTextAsync();
            TargetWord = newWord.Text;

            HighlightedText = new FormattedString(); // Reset
        }
    }
}
