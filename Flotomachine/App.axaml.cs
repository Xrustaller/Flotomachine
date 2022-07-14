using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using Flotomachine.View;
using Flotomachine.ViewModels;
using System;
using System.IO;
using Flotomachine.Utility;

namespace Flotomachine;

public partial class App : Application
{
#if DEBUG
    public static readonly JsonConfigurationProvider<Settings> Settings = new(Path.Join(@"D:\Programs\GitHub\Flotomachine", "Flotomachine.config"));
#else
    public static readonly JsonConfigurationProvider<Settings> Settings = new(Path.Join(Directory.GetCurrentDirectory(), "Flotomachine.config"));
#endif

    public static readonly MainWindow MainWindow = new() { DataContext = new MainWindowViewModel() };


    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (DataBaseService.Initialize() != null)
            {
                Console.WriteLine("Database service initialization error");

            }

            if (ModBusService.Initialize() != null)
            {
                Console.WriteLine("ModBusSerial service initialization error");
            }

            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}