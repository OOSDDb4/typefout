using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Typefout.App.ViewModels;
using Typefout.Core.Interfaces;

namespace Typefout.App.Views
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage(IUserRepo userRepo, IGroupRepo groupRepo, IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new RegistrationViewModel(userRepo, groupRepo, authService);
        }

        private void OnEyeIconTapped(object sender, EventArgs e)
        {
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        }

        private void OnRepeatPasswordEyeIconTapped(object sender, EventArgs e)
        {
            RepeatPasswordEntry.IsPassword = !RepeatPasswordEntry.IsPassword;
        }
    }
}