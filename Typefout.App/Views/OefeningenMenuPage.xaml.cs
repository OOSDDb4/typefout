using System.Data;
using System.Diagnostics;
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

    private async void Oefening1_Clicked(object sender, EventArgs e)
    {
        TypeViewModel vm = App.Services.GetRequiredService<TypeViewModel>();
        await Navigation.PushAsync(new TypeView(vm));
    }

    private async void Oefening2_Clicked(object sender, EventArgs e)
    {
        SentenceViewModel vm = App.Services.GetRequiredService<SentenceViewModel>();
        await Navigation.PushAsync(new SentenceView(vm));
    }
}