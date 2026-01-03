using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Typefout.App.ViewModels;
using Typefout.Core.Interfaces;
using Style = Microsoft.Maui.Controls.Style;

namespace Typefout.App.Views
{
    public partial class TeacherPage : ContentPage
    {
        private readonly Dictionary<string, Border> _menuButtons;

        public TeacherPage()
        {
            InitializeComponent();
            this.BindingContext = new ViewModels.TeacherViewModel();
        }
    }
}