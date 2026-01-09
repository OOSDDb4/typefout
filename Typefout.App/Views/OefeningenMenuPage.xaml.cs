using Microsoft.Extensions.DependencyInjection;
using Typefout.App.ViewModels;
using Typefout.App.Views;
using Typefout.Core;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;

namespace Typefout.App.Views;

public partial class OefeningenMenuPage : ContentPage
{
    private readonly OefeningenMenuViewModel _viewModel;

    public OefeningenMenuPage(OefeningenMenuViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void WordExersiceClicked(object sender, EventArgs e)
    {
        
        WordViewModel vm = App.Services.GetRequiredService<WordViewModel>();
        await Navigation.PushAsync(new WordView(vm));
    }

    private async void SentenceExersiceClicked(object sender, EventArgs e)
    {
        SentenceViewModel vm = App.Services.GetRequiredService<SentenceViewModel>();
        await Navigation.PushAsync(new SentenceView(vm));
    }

    private async void TextExersiceClicked(object sender, EventArgs e)
    {
        TextViewModel vm = App.Services.GetRequiredService<TextViewModel>();
        await Navigation.PushAsync(new TextView(vm));
    }

}