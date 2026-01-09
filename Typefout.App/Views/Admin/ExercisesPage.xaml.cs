using Typefout.App.ViewModels;

namespace Typefout.App.Views.Admin
{
    public partial class ExercisesPage : ContentPage
    {
        private readonly ExercisesViewModel _viewModel;

        public ExercisesPage(ExercisesViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadAsync();
        }
    }
}