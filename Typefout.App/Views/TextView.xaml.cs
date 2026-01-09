using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Typefout.App.ViewModels;

namespace Typefout.App.Views
{
    public partial class TextView : ContentPage
    {
        private List<int> _sentenceEndPositions = new List<int>();
        private int _currentSentenceIndex = 0;

        public TextView(TextViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            UpdateColors("");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is not TextViewModel vm) return;
            
            await vm.InitializeAsync();
            if (!string.IsNullOrEmpty(vm.TargetText))
            {
                BuildSentenceIndex(vm.TargetText);
                UpdateSentenceWindow(vm.TargetText);
            }

            vm.PropertyChanged += Vm_PropertyChanged;
        }
        private void OnEditorLoaded(object sender, EventArgs e)
        {
            if (sender is Editor editor)
            {
                editor.Focus();
                editor.CursorPosition = editor.Text?.Length ?? 0;
                editor.SelectionLength = 0;
            }
        }
        private void OnEditorTapped(object sender, EventArgs e)
        {
            TypingEditor.Focus();
            TypingEditor.CursorPosition = TypingEditor.Text?.Length ?? 0;
            TypingEditor.SelectionLength = 0;
        }
        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (BindingContext is not TextViewModel vm)
                return;

            if (args.PropertyName == nameof(vm.TargetText))
            {
                if (!string.IsNullOrEmpty(vm.TargetText))
                {
                    BuildSentenceIndex(vm.TargetText);
                    UpdateSentenceWindow(vm.TargetText);
                }
            }
        }
        private void BuildSentenceIndex(string text)
        {
            _sentenceEndPositions.Clear();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '.' || c == '!' || c == '?')
                    _sentenceEndPositions.Add(i);
            }

            _currentSentenceIndex = 0;
        }
        private void CheckSentenceProgress(string typed)
        {
            if (BindingContext is not TextViewModel vm)
                return;

            if (string.IsNullOrEmpty(vm.TargetText))
                return;

            if (_sentenceEndPositions.Count == 0)
                return;

            int typedLength = typed.Length;

            if (_currentSentenceIndex < _sentenceEndPositions.Count)
            {
                int requiredEndIndex = _sentenceEndPositions[_currentSentenceIndex];

                if (typedLength - 1 == requiredEndIndex)
                {
                    _currentSentenceIndex++;

                    if (_currentSentenceIndex > _sentenceEndPositions.Count - 1)
                        _currentSentenceIndex = _sentenceEndPositions.Count - 1;

                    UpdateSentenceWindow(vm.TargetText);
                    return;
                }
            }

            if (_currentSentenceIndex > 0)
            {
                int previousSentenceEnd = _sentenceEndPositions[_currentSentenceIndex - 1];

                if (typedLength - 1 < previousSentenceEnd)
                {
                    _currentSentenceIndex--;

                    if (_currentSentenceIndex < 0)
                        _currentSentenceIndex = 0;

                    UpdateSentenceWindow(vm.TargetText);
                }
            }
        }

        private void UpdateSentenceWindow(string fullText)
        {
            if (_sentenceEndPositions.Count == 0)
            {
                VisibleText.Text = fullText;
                return;
            }

            int startSentence = _currentSentenceIndex;
            int endSentence = Math.Min(startSentence + 3, _sentenceEndPositions.Count);

            List<string> output = new List<string>();

            for (int i = startSentence; i < endSentence; i++)
            {
                int start = i == 0 ? 0 : _sentenceEndPositions[i - 1] + 1;
                int end = _sentenceEndPositions[i] + 1;

                string sentence = fullText.Substring(start, end - start).Trim();
                output.Add(sentence);
            }

            VisibleText.Text = string.Join(" ", output);
        }

        private async void TypingEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newText = e.NewTextValue ?? string.Empty;

            CheckSentenceProgress(newText);
            UpdateColors(newText);

            await Task.Delay(1);
            await TypingScroll.ScrollToAsync(0, double.MaxValue, false);
        }

        private void UpdateColors(string typedText)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (int i = 0; i < alphabet.Length; i++)
            {
                char c = alphabet[i];
                Border button = this.FindByName<Border>($"Key_{c}");
                if (button != null)
                {
                    button.BackgroundColor = Colors.White;
                }
            }

            if (BindingContext is not TextViewModel vm)
                return;

            string targetText = vm.TargetText;

            if (string.IsNullOrEmpty(targetText))
                return;

            if (typedText.Length < targetText.Length)
            {
                char nextLetter = char.ToUpper(targetText[typedText.Length]);
                Border nextButton = this.FindByName<Border>($"Key_{nextLetter}");

                if (nextButton != null)
                {
                    nextButton.BackgroundColor = Colors.LightGreen;
                }
            }
        }
    }
}
