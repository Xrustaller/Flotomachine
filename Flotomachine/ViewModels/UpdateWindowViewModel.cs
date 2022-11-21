using Avalonia.Controls;
using Avalonia.Threading;
using Flotomachine.Services;
using Flotomachine.Utility;
using ReactiveUI;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class UpdateWindowViewModel : ViewModelBase
{
    private readonly Window _mainWindow;
    private readonly Window _updateWindow;

    private InfoViewModel _text = new("Текущая: v" + Assembly.GetEntryAssembly()?.GetName().Version.ToShortString() + "\n" + "Новая версия: v" + UpdateService.NewVersion.ToShortString(), "#FFFFFF");
    private bool _buttonsEnable = true;

    public InfoViewModel Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    public bool ButtonsEnable
    {
        get => _buttonsEnable;
        set => this.RaiseAndSetIfChanged(ref _buttonsEnable, value);
    }

    public ICommand HoldOverClick { get; }
    public ICommand DownloadClick { get; }

    public UpdateWindowViewModel()
    {

    }

    public UpdateWindowViewModel(Window window, Window thisWindow)
    {
        _mainWindow = window;
        _updateWindow = thisWindow;
        HoldOverClick = new DelegateCommand(HoldOver);
        DownloadClick = new DelegateCommand(Download);
    }

    private void Download(object obj)
    {
        ButtonsEnable = false;
        string path = UpdateService.DownloadLatestReleaseFile();
        if (string.IsNullOrEmpty(path))
        {
            Text.Text = "Ошибка получения файла";
            return;
        }
        Text.Text = "Запуск обновления\nОбновление длится 1-5 мин.\nПрограмма будет закрыта и перезапущена\nНе выключайте компьютер и не запускайте программу заново";
        Task.Run(() =>
        {
            Thread.Sleep(10000);
            UpdateService.InstallDebFile(path);
            Dispatcher.UIThread.InvokeAsync(() => _mainWindow.Close());
        });
    }

    private void HoldOver(object obj)
    {
        _updateWindow.Close();
    }
}