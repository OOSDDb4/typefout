using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Style = Microsoft.Maui.Controls.Style;

namespace Typefout.App.Views
{
    // Класс переименован в TeacherPage
    public partial class TeacherPage : ContentPage
    {
        private readonly Dictionary<string, Border> _menuButtons;

        public TeacherPage()
        {
            InitializeComponent();

            _menuButtons = new Dictionary<string, Border>
            {
                { "School", SchoolMenuButton },
                { "Groepen", GroepenMenuButton },
                { "Students", StudentsMenuButton },
                { "Instellingen", InstellingenMenuButton }
            };
        }

        private void MenuItem_Tapped(object sender, TappedEventArgs e)
        {
            if (sender is Border tappedBorder && e.Parameter is string selectedPage)
            {
                ResetMenuStyles();
                tappedBorder.Style = (Style)this.Resources["ActiveMenuItemStyle"];

                PageTitleLabel.Text = selectedPage;
                LoadContentView(selectedPage);
            }
        }

        private void ResetMenuStyles()
        {
            Style defaultStyle = (Style)this.Resources["MenuItemStyle"];

            foreach (Border button in _menuButtons.Values)
            {
                button.Style = defaultStyle;
            }
        }

        private void LoadContentView(string pageName)
        {
            View newContent = null;

            switch (pageName)
            {
                case "School":
                    newContent = new SchoolContentView();
                    break;
                case "Students":
                    newContent = new StudentsContentView();
                    PageTitleLabel.Text = "Leerlingen";
                    break;
                case "Groepen":
                    newContent = new GroupContentView();
                    PageTitleLabel.Text = "Groepen";
                    break;
                case "Instellingen":
                    newContent = new Label { Text = "Instellingen", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                    break;
                default:
                    newContent = new Label { Text = $"Content voor '{pageName}' niet gevonden.", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                    break;
            }
            
            if (newContent != null)
            {
                ContentPlaceholder.Content = newContent;
            }
        }
    }
}