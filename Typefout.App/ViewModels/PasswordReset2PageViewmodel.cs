using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.App.Views;
using Typefout.Core.Interfaces;


namespace Typefout.App.ViewModels
{
    [QueryProperty(nameof(UserEmail), "Email")]
    public partial class PasswordReset2PageViewmodel : ObservableObject
    {
        private readonly IVerificationService _verificationService;

        public PasswordReset2PageViewmodel(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [ObservableProperty]
        private string _userEmail;

        [ObservableProperty] private string _digit1;
        [ObservableProperty] private string _digit2;
        [ObservableProperty] private string _digit3;
        [ObservableProperty] private string _digit4;
        [ObservableProperty] private string _digit5;
        [ObservableProperty] private string _digit6;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public bool isCodeValid = false;

        partial void OnUserEmailChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _verificationService.GenerateVerificationCode(_userEmail);
            }
        }

        [RelayCommand]
        public async Task VerifyCode()
        {
            string code = $"{Digit1}{Digit2}{Digit3}{Digit4}{Digit5}{Digit6}";

            if (code.Length != 6) return;

            bool isValid = _verificationService.TryValidateCode(_userEmail, code);

            if (isValid)
            {
                isCodeValid = true;
                System.Diagnostics.Debug.WriteLine($"Code is correct");
                ErrorMessage = string.Empty;
            }
            else
            {
                isCodeValid = false;
                System.Diagnostics.Debug.WriteLine($"Code is incorrect");
                ErrorMessage = "De ingevoerde code is onjuist of verlopen.";
                ClearCode();
            }
        }

        public void ClearCode()
        {
            Digit1 = string.Empty;
            Digit2 = string.Empty;
            Digit3 = string.Empty;
            Digit4 = string.Empty;
            Digit5 = string.Empty;
            Digit6 = string.Empty;
        }
    }
}
