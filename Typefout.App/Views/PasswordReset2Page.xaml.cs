using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Typefout.App.ViewModels;
namespace Typefout.App.Views;

public partial class PasswordReset2Page : ContentPage
{
    public PasswordReset2Page(PasswordReset2PageViewmodel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnDigitTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;

        if (string.IsNullOrEmpty(entry.Text)) return;

        if (entry == Digit1) Digit2.Focus();
        else if (entry == Digit2) Digit3.Focus();
        else if (entry == Digit3) Digit4.Focus();
        else if (entry == Digit4) Digit5.Focus();
        else if (entry == Digit5) Digit6.Focus();
        else if (entry == Digit6)
        {
            Digit6.Unfocus();

            if (BindingContext is PasswordReset2PageViewmodel vm)
            {
                vm.VerifyCodeCommand.Execute(null);

                if (string.IsNullOrEmpty(vm.Digit1))
                {
                    Digit1.Focus();
                }
            }
        }
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new PasswordReset1Page());
    }

    private async void OnInvoerClicked(object sender, EventArgs e)
    {
        if (BindingContext is PasswordReset2PageViewmodel vm)
        {
            if (vm.isCodeValid)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new PasswordReset3Page());
                vm.ClearCode();

                vm.isCodeValid = false;
            }
            else return;
        }
        else return;
    }
}