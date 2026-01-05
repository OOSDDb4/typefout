using Typefout.App.ViewModels;

namespace Typefout.App.Views
{
    public partial class StudentsContentView : ContentView
    {
        public StudentsContentView()
        {
            BindingContext = new StudentsViewModel();
            InitializeComponent();
            if (BindingContext is TeacherViewModel teacherVM)
            {
                teacherVM.ChangePageCommand.Execute("RegistrationPage");
            }
        }
    }
}