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

        FormattedString formatted = new FormattedString();

        for (int i = 0; i < _inputText.Length; i++)
        {
            char typedChar = _inputText[i];

            char correctChar = i < TargetWord.Length ? TargetWord[i] : '?';

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
    private async void NextWord()
    {
        InputText = string.Empty;
        IsCompleted = false;

        TypingExerciseText newWord = await _aiService.GetExerciseTextAsync("word");
        TargetWord = newWord.Text;

        HighlightedText = new FormattedString();

        HighlightedText.Spans.Add(new Span
        {
            Text = "|",
            TextColor = Colors.Black,
            FontSize = 18
        });
    }


  
}
