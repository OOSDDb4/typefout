using Typefout.App.Views;
using Typefout.App.Views.Admin;
using Typefout.App.Views.Docent;

namespace Typefout.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(PasswordReset2Page), typeof(PasswordReset2Page));

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(OefeningenMenuPage), typeof(OefeningenMenuPage));
        Routing.RegisterRoute(nameof(SchoolsPage), typeof(SchoolsPage));
        Routing.RegisterRoute("RegistrationPage", typeof(RegistrationPage));
    }
}