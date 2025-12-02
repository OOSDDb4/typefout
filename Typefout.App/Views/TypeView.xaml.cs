using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class TypeView : ContentPage
{
    public TypeView(TypeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        UpdateKleuren("");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TypeViewModel vm)
        {
            UpdateKleuren("");

            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TargetWord" || args.PropertyName == "InputText")
                {
                    UpdateKleuren(vm.InputText ?? "");
                }
            };
        }
    }

    private void OnInputChanged(object sender, TextChangedEventArgs e)
    {
        UpdateKleuren(e.NewTextValue ?? "");
    }

    private void UpdateKleuren(string getypteTekst)
    {
        string alfabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        foreach (char c in alfabet)
        {
            Border knop = this.FindByName<Border>($"Key_{c}");
            if (knop != null) knop.BackgroundColor = Colors.White;
        }

        string targetWord = "";

        if (BindingContext is TypeViewModel vm)
            targetWord = vm.TargetWord;

        if (string.IsNullOrEmpty(targetWord))
        {
            return;
        }

        if (getypteTekst.Length < targetWord.Length)
        {
            char volgendeLetter = char.ToUpper(targetWord[getypteTekst.Length]);

            Border volgendeKnop = this.FindByName<Border>($"Key_{volgendeLetter}");
            if (volgendeKnop != null)
            {
                volgendeKnop.BackgroundColor = Colors.LightGreen;
            }
        }
    }
}