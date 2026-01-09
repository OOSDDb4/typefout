using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Typefout.App.ViewModels;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using Style = Microsoft.Maui.Controls.Style;

namespace Typefout.App.Views.Docent
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
            IAuthService authService = App.Services.GetRequiredService<IAuthService>();
            IGroupRepo groupRepo = App.Services.GetRequiredService<IGroupRepo>();
            IUserRepo userRepo = App.Services.GetRequiredService<IUserRepo>();
            BindingContext = new RegistrationViewModel(userRepo, groupRepo, authService);
        }
        private void ToggleViewPassword(object sender, EventArgs e)
        {
            PassWord.IsPassword = !PassWord.IsPassword;
        }
        private void ToggleViewPasswordConfirm(object sender, EventArgs e)
        {
            ConfirmPassWord.IsPassword = !ConfirmPassWord.IsPassword;
        }
    }
}
