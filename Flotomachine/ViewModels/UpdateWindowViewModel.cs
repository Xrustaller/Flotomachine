using System.Reflection;
using System.Windows.Input;
using Avalonia.Controls;
using Flotomachine.Services;
using Flotomachine.Utility;
using ReactiveUI;

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
        CurrentVersion = "Текущая: v1.0.0";
        NewVersion = "Новая версия: v1.0.0";
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


        _mainWindow.Close();
    }

    private void HoldOver(object obj)
    {
        _updateWindow.Close(); 
    }
}