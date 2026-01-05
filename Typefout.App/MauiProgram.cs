using Microsoft.Extensions.Logging;
using Typefout.App.ViewModels;
using Typefout.App.Views;
using Typefout.App.Views.Admin;
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
        builder.Services.AddSingleton<ISchoolRepo, SchoolRepo>();
        builder.Services.AddSingleton<IGroupRepo, GroupRepo>();

        builder.Services.AddTransient<LoginPageViewModel>();
        builder.Services.AddTransient<WordViewModel>();
        builder.Services.AddTransient<SentenceViewModel>();
        builder.Services.AddTransient<ResultsViewModel>();
        builder.Services.AddTransient<TextViewModel>();

        builder.Services.AddTransient<SchoolsViewModel>();
        builder.Services.AddTransient<SchoolCreateViewModel>();
        builder.Services.AddTransient<SchoolEditViewModel>();
        builder.Services.AddTransient<SchoolInfoViewModel>();
        builder.Services.AddTransient<GroupCreateViewModel>();
        builder.Services.AddTransient<GroupEditViewModel>();
        builder.Services.AddTransient<TeacherCreateViewModel>();
        builder.Services.AddTransient<TeacherEditViewModel>();
        builder.Services.AddTransient<StudentCreateViewModel>();
        builder.Services.AddTransient<StudentEditViewModel>();
        builder.Services.AddTransient<AddUserViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TextView>();
        builder.Services.AddTransient<WordView>();
        builder.Services.AddTransient<SentenceView>();
        builder.Services.AddTransient<ResultsPage>();

        builder.Services.AddTransient<SchoolsPage>();
        builder.Services.AddTransient<SchoolCreatePage>();
        builder.Services.AddTransient<SchoolEditPage>();
        builder.Services.AddTransient<SchoolInfoPage>();
        builder.Services.AddTransient<GroupCreatePage>();
        builder.Services.AddTransient<GroupEditPage>();
        builder.Services.AddTransient<TeacherCreatePage>();
        builder.Services.AddTransient<TeacherEditPage>();
        builder.Services.AddTransient<StudentCreatePage>();
        builder.Services.AddTransient<StudentEditPage>();
        builder.Services.AddTransient<AddUserPage>();

        MauiApp app = builder.Build();

        Routing.RegisterRoute("schools", typeof(SchoolsPage));
        Routing.RegisterRoute("schoolcreate", typeof(SchoolCreatePage));
        Routing.RegisterRoute("schooledit", typeof(SchoolEditPage));
        Routing.RegisterRoute("schoolinfo", typeof(SchoolInfoPage));
        Routing.RegisterRoute("groupcreate", typeof(GroupCreatePage));
        Routing.RegisterRoute("groupedit", typeof(GroupEditPage));
        Routing.RegisterRoute("teachercreate", typeof(TeacherCreatePage));
        Routing.RegisterRoute("teacheredit", typeof(TeacherEditPage));
        Routing.RegisterRoute("studentcreate", typeof(StudentCreatePage));
        Routing.RegisterRoute("studentedit", typeof(StudentEditPage));
        Routing.RegisterRoute("adduser", typeof(AddUserPage));

        return app;
    }
}
