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
    public OefeningenMenuPage()
    {
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

    private async void Database_Clicked(object sender, EventArgs e)
    {
        Trace.WriteLine("DatabaseClicked");

        IDatabaseService service = new DatabaseService();
        service.Connect();
        service.Open();
        DataTable words = service.ExecuteQuery("SELECT * FROM Word");

        foreach (DataRow row in words.Rows)
        {
            Trace.WriteLine($"{row["wordId"]}, {row["word"]}");
        }

        service.Close();

        // DatabaseViewModel dvm = App.Services.GetRequiredService<DatabaseViewModel>();
        // await Navigation.PushAsync(new DatabaseView(dvm));
    }
    
}