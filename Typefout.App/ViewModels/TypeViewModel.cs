using System.Collections.Generic;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class TypeViewModel : ObservableObject
    {
        private readonly IWordService _wordService;

        private List<OefenWoord> _words;
        private int _index = 0;

        [ObservableProperty]
        private string _targetWord;

        [ObservableProperty]
        private string _inputText;

        [ObservableProperty]
        private FormattedString _highlightedText;

        public TypeViewModel(IWordService wordService)
        {
            _wordService = wordService;
            _words = _wordService.GetAllWords();
            LoadWord();
        }

        private async void LoadWord()
        {
            if (_index >= _words.Count)
            {
                await Shell.Current.DisplayAlert(
                    "Klaar!",
                    "Je hebt alle woorden getypt!",
                    "OK");

                await Shell.Current.Navigation.PopToRootAsync();
                return;
            }

            TargetWord = _words[_index].Text;
            InputText = string.Empty;
            HighlightedText = new FormattedString();
        }

        partial void OnInputTextChanged(string value)
        {
            HighlightErrors();

            if (!string.IsNullOrEmpty(value) && value == TargetWord)
            {
                _index++;
                LoadWord();
            }
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
                char typed = InputText[i];
                char correct = i < TargetWord.Length ? TargetWord[i] : '?';

                formatted.Spans.Add(new Span
                {
                    Text = typed.ToString(),
                    TextColor = typed == correct ? Colors.Black : Colors.Red
                });
            }

            HighlightedText = formatted;
        }
    }
}
