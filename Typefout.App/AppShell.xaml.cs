using Typefout.App.Views;

namespace Typefout.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(PasswordReset2Page), typeof(PasswordReset2Page));
    }
}