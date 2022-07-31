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

    private InfoViewModel _info;

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

    public InfoViewModel Info
    {
        get => _info;
        set => this.RaiseAndSetIfChanged(ref _info, value);
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
        SerialList.Clear();
        SerialBaudRateList.Clear();

        foreach (string item in SerialPort.GetPortNames())
        {
            SerialList.Add(item);
        }

        foreach (int item in ModBusService.BaudRateList)
        {
            SerialBaudRateList.Add(item);
        }


        if (SerialList.Contains(App.Settings.Configuration.Serial.Port))
        {
            SerialSelectedItem = App.Settings.Configuration.Serial.Port;
        }
        else
        {
            SerialList.Add(App.Settings.Configuration.Serial.Port);
            SerialSelectedItem = App.Settings.Configuration.Serial.Port;
        }

        if (SerialBaudRateList.Contains(App.Settings.Configuration.Serial.BaudRate))
        {
            SerialBaudRateSelectedItem = App.Settings.Configuration.Serial.BaudRate;
        }
        else
        {
            SerialBaudRateList.Add(App.Settings.Configuration.Serial.BaudRate);
            SerialBaudRateSelectedItem = App.Settings.Configuration.Serial.BaudRate;
        }
    }

    private void SaveSettings(object obj)
    {
        App.Settings.Configuration.Serial.BaudRate = SerialBaudRateSelectedItem;
        App.Settings.Configuration.Serial.Port = SerialSelectedItem;

        App.Settings.SaveConfig();

        Info = new InfoViewModel("Успешно", "#10FF10");
    }
}