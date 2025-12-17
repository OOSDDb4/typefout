using Microsoft.Extensions.Logging;
using Typefout.App.ViewModels;
using Typefout.App.Views;
using Typefout.Core.Data.Repo;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using Typefout.Core.Services;

namespace Typefout.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IAiService, AiService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IKeyTrackingService, KeyTrackingService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IUserRepo, UserRepo>();

        builder.Services.AddTransient<LoginPageViewModel>();
        builder.Services.AddTransient<WordViewModel>();
        builder.Services.AddTransient<SentenceViewModel>();
        builder.Services.AddTransient<ResultsViewModel>();
        builder.Services.AddTransient<TextViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TextView>();
        builder.Services.AddTransient<WordView>();
        builder.Services.AddTransient<SentenceView>();
        builder.Services.AddTransient<ResultsPage>();


        return builder.Build();
    }
}