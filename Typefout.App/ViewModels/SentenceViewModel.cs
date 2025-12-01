using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;
using System.Collections.Generic;

namespace Typefout.App.ViewModels
{
    public partial class SentenceViewModel : ObservableObject
    {
        private readonly ISentenceService _sentenceService;

        private List<OefeningZin> _sentences;
        private int _index = 0;

        [ObservableProperty]
        private string _targetText;

        [ObservableProperty]
        private string _inputText;

        [ObservableProperty]
        private FormattedString _highlightedText;

        public SentenceViewModel(ISentenceService sentenceService)
        {
            _sentenceService = sentenceService;
            _sentences = _sentenceService.GetAllSentences();
            LoadSentence();
        }

        private async void LoadSentence()
        {
            if (_index >= _sentences.Count)
            {
                await Shell.Current.DisplayAlert(
                    "Klaar!",
                    "Je hebt alle zinnen overgetypt!",
                    "OK");

                await Shell.Current.Navigation.PopToRootAsync();
                return;
            }

            TargetText = _sentences[_index].Text;
            InputText = string.Empty;
            HighlightedText = new FormattedString();
        }

        partial void OnInputTextChanged(string value)
        {
            HighlightErrors();

            if (!string.IsNullOrEmpty(value) && value == TargetText)
            {
                _index++;
                LoadSentence();
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
                char correct = i < TargetText.Length ? TargetText[i] : '?';

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
