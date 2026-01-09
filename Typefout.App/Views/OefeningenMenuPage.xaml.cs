using Microsoft.Extensions.DependencyInjection;
using Typefout.App.ViewModels;
using Typefout.App.Views;
using Typefout.Core;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;

namespace Typefout.App.Views;

public partial class OefeningenMenuPage : ContentPage
{
    private IDatabaseService _databaseService;
    public OefeningenMenuPage()
    {
        _databaseService = new DatabaseService();
        InitializeComponent();
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

    private async void OnNavigateButtonClicked(object sender, EventArgs e)
    {
        TeacherPage detailPage = new TeacherPage();

        await Navigation.PushAsync(detailPage);
    }
    private async void TextExersiceClicked(object sender, EventArgs e)
    {
        TextViewModel vm = App.Services.GetRequiredService<TextViewModel>();
        await Navigation.PushAsync(new TextView(vm));
    }
    private async void OnAdminButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("schools");
    }

}