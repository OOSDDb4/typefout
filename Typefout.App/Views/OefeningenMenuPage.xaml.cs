using Microsoft.Extensions.DependencyInjection;
using Typefout.App.ViewModels;
using Typefout.App.Views;

namespace Typefout.App.Views;

public partial class OefeningenMenuPage : ContentPage
{
    public OefeningenMenuPage()
    {
        InitializeComponent();
    }

    private async void Oefening1_Clicked(object sender, EventArgs e)
    {
        WordViewModel vm = App.Services.GetRequiredService<WordViewModel>();
        await Navigation.PushAsync(new WordView(vm));
    }

    private async void Oefening2_Clicked(object sender, EventArgs e)
    {
        SentenceViewModel vm = App.Services.GetRequiredService<SentenceViewModel>();
        await Navigation.PushAsync(new SentenceView(vm));
    }

    private async void Oefening3_Clicked(object sender, EventArgs e)
    {
        TextViewModel vm = App.Services.GetRequiredService<TextViewModel>();
        await Navigation.PushAsync(new TextView(vm));
    }


}