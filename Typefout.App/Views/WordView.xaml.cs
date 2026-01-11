using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class WordView : ContentPage
{
    public WordView(WordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        UpdateColors("");
    }

    private void OnEntryLoaded(object sender, EventArgs e)
    {
        if (sender is Entry entry)
        {
            entry.Focus();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is WordViewModel vm)
        {
            await vm.InitializeAsync();
            UpdateColors("");

            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TargetText" || args.PropertyName == "InputText")
                {
                    UpdateColors(vm.InputText ?? "");
                }
            };
        }
    }

    private void OnInputChanged(object sender, TextChangedEventArgs e)
    {
        UpdateColors(e.NewTextValue ?? "");
    }

    private void UpdateColors(string typedText)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        foreach (char c in alphabet)
        {
            Border button = this.FindByName<Border>($"Key_{c}");
            if (button != null) button.BackgroundColor = Colors.White;
        }

        string targetText = "";

        if (BindingContext is WordViewModel vm)
            targetText = vm.TargetText;

        if (string.IsNullOrEmpty(targetText))
        {
            return;
        }

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