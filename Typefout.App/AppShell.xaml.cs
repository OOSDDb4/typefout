using Typefout.App.Views;
using Typefout.App.Views.Admin;

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
    }
}