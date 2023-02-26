using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using Flotomachine.Utility;
using Flotomachine.View;
using Flotomachine.ViewModels;

namespace Flotomachine;

public partial class App : Application
{
    public static readonly string MyDocumentPath = GetDocumentPath();
    public static readonly string DownloadPath = Path.Join(GetDocumentPath(), "Download");
    public static readonly string LogFolderPath = Path.Join(GetDocumentPath(), "Logs");

    public static readonly JsonConfigurationProvider<Settings> Settings = new(Path.Join(MyDocumentPath, "Flotomachine.config"));

    public static MainWindow MainWindow { get; private set; }
    public static MainWindowViewModel MainWindowViewModel { get; private set; }

    private static string GetDocumentPath() => Path.Join(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Documents" : Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Flotomachine");

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Exception? exceptionDb = DataBaseService.Initialize(MyDocumentPath);
            if (exceptionDb != null)
            {
                Console.WriteLine("Database service initialization error");
                LogManager.ErrorLog(exceptionDb, "ErrorLog_InitDataBaseService");
            }

            Exception? exceptionMb = ModBusService.Initialize();
            if (exceptionMb != null)
            {
                Console.WriteLine("ModBus service initialization error");
                LogManager.ErrorLog(exceptionMb, "ErrorLog_InitModBusService");
            }

            Exception? exceptionC = CameraService.Initialize();
            if (exceptionC != null)
            {
                Console.WriteLine("Camera service initialization error");
                LogManager.ErrorLog(exceptionC, "ErrorLog_InitCameraService");
            }

            UpdateService.CheckUpdates(Settings.Configuration);

            MainWindow = new MainWindow();
            MainWindowViewModel = new MainWindowViewModel(MainWindow);
            MainWindow.DataContext = MainWindowViewModel;

            desktop.MainWindow = MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void Exit()
    {
        ModBusService.Exit();
        CameraService.Exit();
    }
}