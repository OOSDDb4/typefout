using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class TypeView : ContentPage
{
    public TypeView(TypeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        UpdateColors("");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TypeViewModel vm)
        {
            UpdateColors("");

            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TargetWord" || args.PropertyName == "InputText")
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

        string targetWord = "";

        if (BindingContext is TypeViewModel vm)
            targetWord = vm.TargetWord;

        if (string.IsNullOrEmpty(targetWord))
        {
            return;
        }

        if (typedText.Length < targetWord.Length)
        {
            char nextLetter = char.ToUpper(targetWord[typedText.Length]);

            Border nextButton = this.FindByName<Border>($"Key_{nextLetter}");
            if (nextButton != null)
            {
                nextButton.BackgroundColor = Colors.LightGreen;
            }
        }
    }
}