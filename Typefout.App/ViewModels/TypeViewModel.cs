using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class TypeViewModel : ObservableObject
    {
        private readonly IWordService _wordService;

        [ObservableProperty]
        private string _targetWord;

        [ObservableProperty]
        private string _inputText;

        [ObservableProperty]
        private bool _isCompleted;

        public TypeViewModel(IWordService wordService)
        {
            _wordService = wordService;
            NextWord();
        }

        partial void OnInputTextChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (value == TargetWord)
            {
                IsCompleted = true;
                NextWord();
            }
        }

        [RelayCommand]
        private void NextWord()
        {
            InputText = string.Empty;
            IsCompleted = false;

            OefenWoord newWord = _wordService.GetRandomized();
            _targetWord = newWord.Text;
        }
    }
}
