using System;
using Microsoft.Maui.Controls;
namespace Typefout.App;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;
        MainPage = new AppShell();
    }
}