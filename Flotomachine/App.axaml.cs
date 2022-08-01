using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using Flotomachine.Utility;
using Flotomachine.View;
using Flotomachine.ViewModels;
using System;
using System.IO;

namespace Flotomachine;

public partial class App : Application
{
#if DEBUG    
    public static readonly JsonConfigurationProvider<Settings> Settings = new(Path.Join(Directory.GetCurrentDirectory(), "Flotomachine.config"));
    //public static readonly JsonConfigurationProvider<Settings> Settings = new(Path.Join(@"D:\Programs\GitHub\Flotomachine", "Flotomachine.config"));
#else
    public static readonly JsonConfigurationProvider<Settings> Settings = new(Path.Join(Directory.GetCurrentDirectory(), "Flotomachine.config"));
#endif
    public static MainWindow MainWindow { get; private set; }
    public static MainWindowViewModel MainWindowViewModel { get; private set; }

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

            MainWindowViewModel = new MainWindowViewModel(Settings.Configuration);
            MainWindow = new MainWindow { DataContext = MainWindowViewModel };
            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}