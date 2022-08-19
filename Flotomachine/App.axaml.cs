using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using Flotomachine.Utility;
using Flotomachine.View;
using Flotomachine.ViewModels;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Flotomachine;

public partial class App : Application
{
    public static readonly string MyDocumentPath = GetDocumentPath();
    public static readonly string LogFolderPath = Path.Join(GetDocumentPath(), "Logs");

    public static readonly JsonConfigurationProvider<Settings> Settings = new(Path.Join(MyDocumentPath, "Flotomachine.config"));

    public static MainWindow MainWindow { get; private set; }
    public static MainWindowViewModel MainWindowViewModel { get; private set; }

    private static string GetDocumentPath() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Documents", "Flotomachine") : Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Flotomachine");

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Exception exceptionDb = DataBaseService.Initialize(MyDocumentPath);
            if (exceptionDb != null)
            {
                Console.WriteLine("Database service initialization error");
                LogManager.ErrorLog(exceptionDb, "ErrorLog_InitDataBase");
            }

            Exception exceptionMb = ModBusService.Initialize();

            if (exceptionMb != null)
            {
                Console.WriteLine("ModBus service initialization error");
                LogManager.ErrorLog(exceptionDb, "ErrorLog_InitModBus");
            }

            MainWindowViewModel = new MainWindowViewModel(false);
            MainWindow = new MainWindow { DataContext = MainWindowViewModel };
            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}