using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Typefout.App.Views;
using Typefout.Core.Interfaces;

namespace Typefout.App.ViewModels
{
    public class TeacherViewModel : INotifyPropertyChanged
    {
        private View _currentContent;
        private string _pageTitle = "School";

        public event PropertyChangedEventHandler PropertyChanged;

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                OnPropertyChanged();
            }
        }

        public View CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangePageCommand { get; }

        public TeacherViewModel()
        {
            ChangePageCommand = new Command<string>(LoadContentView);

            LoadContentView("School");
        }

        private void LoadContentView(string pageName)
        {
            switch (pageName)
            {
                case "School":
                    CurrentContent = new Views.SchoolContentView();
                    PageTitle = "School";
                    break;
                case "Students":
                    CurrentContent = new Views.StudentsContentView();
                    PageTitle = "Leerlingen";
                    break;
                case "Groepen":
                    CurrentContent = new Label { Text = "Groepen Content", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                    PageTitle = "Groepen";
                    break;
                case "Instellingen":
                    CurrentContent = new Label { Text = "Instellingen Content", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                    PageTitle = "Instellingen";
                    break;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}