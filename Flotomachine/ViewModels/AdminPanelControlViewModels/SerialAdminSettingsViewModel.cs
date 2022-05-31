using Flotomachine.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class SerialAdminSettingsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private string _serialSelectedItem;
    private int _serialBaudRateSelectedItem;

    public ObservableCollection<string> SerialList { get; set; } = new();
    public ObservableCollection<int> SerialBaudRateList { get; set; } = new();

    public string SerialSelectedItem
    {
        get => _serialSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _serialSelectedItem, value);
    }

    public int SerialBaudRateSelectedItem
    {
        get => _serialBaudRateSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _serialBaudRateSelectedItem, value);
    }

    public ICommand SaveClick { get; }

    public SerialAdminSettingsViewModel()
    {

    }

    public SerialAdminSettingsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        SaveClick = new DelegateCommand(SaveSettings);

        Refresh();
    }

    public void Refresh()
    {
        foreach (string item in SerialPort.GetPortNames())
        {
            SerialList.Add(item);
        }

        foreach (int item in ModBusService.BaudRateList)
        {
            SerialBaudRateList.Add(item);
        }
    }

    private void SaveSettings(object obj)
    {

    }
}