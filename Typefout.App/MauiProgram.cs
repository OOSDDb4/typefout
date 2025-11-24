using Microsoft.Extensions.Logging;
using Typefout.Core.Interfaces;
using Typefout.Core.Data.Services;

using Typefout.App.ViewModels;
using Typefout.App.Views;

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

        builder.Services.AddSingleton<IWordService, WordService>();

        builder.Services.AddTransient<TypeViewModel>();

        builder.Services.AddTransient<TypeView>();

        return builder.Build();
    }
}