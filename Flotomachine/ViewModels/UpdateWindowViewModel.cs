using System.Diagnostics;
using Avalonia.Controls;
using Flotomachine.Services;
using Flotomachine.Utility;
using ReactiveUI;
using System.Reflection;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class UpdateWindowViewModel : ViewModelBase
{
    private Window _mainWindow;
    private Window _updateWindow;

    private string _newVersion;
    private string _currentVersion;
    public string NewVersion
    {
        get => _newVersion;
        set => this.RaiseAndSetIfChanged(ref _newVersion, value);
    }

    public string CurrentVersion
    {
        get => _currentVersion;
        set => this.RaiseAndSetIfChanged(ref _currentVersion, value);
    }

    public ICommand HoldOverClick { get; }
    public ICommand DownloadClick { get; }

    public UpdateWindowViewModel()
    {
        CurrentVersion = "Текущая: ERROR";
        NewVersion = "Новая версия: ERROR";
    }

    public UpdateWindowViewModel(Window window, Window thisWindow)
    {
        _mainWindow = window;
        _updateWindow = thisWindow;
        HoldOverClick = new DelegateCommand(HoldOver);
        DownloadClick = new DelegateCommand(Download);
        CurrentVersion = "Текущая: v" + Assembly.GetEntryAssembly()?.GetName().Version.ToShortString();
        NewVersion = "Новая версия: v" + UpdateService.NewVersion.ToShortString();
    }

    private void Download(object obj)
    {
        string path = UpdateService.DownloadLatestReleaseFile();
        Process proc = new();
        proc.StartInfo.FileName = "bash";
        proc.StartInfo.Arguments = $"-c \"sudo dpkg -i {path}; sleep 15; Flotomachine\"";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        _mainWindow.Close();
    }

    private void HoldOver(object obj)
    {
        _updateWindow.Close();
    }
}