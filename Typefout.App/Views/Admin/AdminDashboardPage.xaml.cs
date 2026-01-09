namespace Typefout.App.Views.Admin
{
    public partial class AdminDashboardPage : ContentPage
    {
        public AdminDashboardPage()
        {
            InitializeComponent();
        }

        private async void OnManageSchoolsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("schools");
        }

        private async void OnManageExercisesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("exercises");
        }
    }
}