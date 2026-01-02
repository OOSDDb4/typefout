using Typefout.App.ViewModels;

namespace Typefout.App.Views.Admin;

public partial class SchoolCreatePage : ContentPage
{
    public SchoolCreatePage(SchoolCreateViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}