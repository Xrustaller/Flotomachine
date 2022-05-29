using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using Flotomachine.View;
using Flotomachine.ViewModels;
using System;

namespace Flotomachine;

public partial class App : Application
{
    public static readonly MainWindow MainWindow = new() { DataContext = new MainWindowViewModel() };
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (DataBaseService.Initialize() != null)
        {
            Console.WriteLine("Database service initialization error");

        }

        if (ModBusService.Initialize("COM1") != null)
        {
            Console.WriteLine("ModBusSerial service initialization error");
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}