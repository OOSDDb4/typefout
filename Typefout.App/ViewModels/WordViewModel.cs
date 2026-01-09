using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Typefout.App.Views;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;


namespace Typefout.App.ViewModels
{
    public partial class WordViewModel : BaseExerciseViewModel
    {
        public WordViewModel(IAiService aiService, IKeyTrackingService trackingService, ITimerService timerService) 
            : base (aiService,  trackingService, timerService, exerciseLength: 1, exerciseTime: 30)
        {
        }
        protected override void OnCorrectInput()
        {
            NextWord();
        }
        public async Task InitializeAsync()
        {
            await NextWord();
            _timerService.Start();
        }
        protected override void HighlightErrors(bool registerLastChar)
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
        private async Task NextWord()
        {
            InputText = string.Empty;
            _previousLength = 0;

            TypingExerciseText text = await _aiService.GetExerciseTextAsync("word");
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
            return _index > 0;
        }
    }
}
