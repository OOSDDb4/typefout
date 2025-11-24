using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class TypeView : ContentPage
{
    public TypeView(TypeViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}