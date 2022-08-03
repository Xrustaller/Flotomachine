using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace Flotomachine;
internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            //TODO: Сделать генерацию логов после краша приложения.
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().UseReactiveUI().UsePlatformDetect().LogToTrace();
    }
}