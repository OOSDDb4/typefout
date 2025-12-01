using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

public partial class TypeViewModel : ObservableObject
{
    private readonly IAiService _aiService;

    private int _index = 0;
    private const int _exerciseLength = 10;

    [ObservableProperty] private string _targetWord;
    [ObservableProperty] private string _inputText;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private FormattedString _highlightedText;

    public TypeViewModel(IAiService aiService)
    {
        _aiService = aiService;
        NextWord();
    }

    partial void OnInputTextChanged(string value)
    {
        HighlightErrors();

        if (!string.IsNullOrWhiteSpace(value) && value == TargetWord)
        {
            _index++;

            if (_index >= _exerciseLength)
            {
                ShowCompletionPopup();
                return;
            }

            NextWord();
        }
    }

    private async void ShowCompletionPopup()
    {
        await Shell.Current.DisplayAlert("Klaar!", "Je hebt alle woorden getypt!", "OK");
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

    [RelayCommand]
    private async void NextWord()
    {
        InputText = string.Empty;
        IsCompleted = false;

        TypingExerciseText newWord = await _aiService.GetExerciseTextAsync("word");
        TargetWord = newWord.Text;

        HighlightedText = new FormattedString();
    }
}
