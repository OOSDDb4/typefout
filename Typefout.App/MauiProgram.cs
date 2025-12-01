using Microsoft.Extensions.Logging;
using Typefout.App.ViewModels;
using Typefout.App.Views;
using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;

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
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegul    ar");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<IWordService, WordService>();
        builder.Services.AddTransient<TypeViewModel>();
        builder.Services.AddTransient<TypeView>();

        builder.Services.AddSingleton<ISentenceService, SentenceService>();
        builder.Services.AddTransient<SentenceViewModel>();
        builder.Services.AddTransient<SentenceView>();
        return builder.Build();
    }
}