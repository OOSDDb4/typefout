using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class SentenceView : ContentPage
{
    public SentenceView(SentenceViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}