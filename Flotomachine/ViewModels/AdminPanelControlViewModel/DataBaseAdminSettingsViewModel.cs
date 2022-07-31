using Avalonia.Media;
using Flotomachine.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class DataBaseAdminSettingsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private string _serialSelectedItem;
    private int _serialBaudRateSelectedItem;

    private string _info;
    private IBrush _colorInfo;

    public ObservableCollection<string> SerialList { get; set; } = new();
    public ObservableCollection<int> SerialBaudRateList { get; set; } = new();
    public ObservableCollection<int> Rs485PinReDeList { get; set; } = new();

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

    public string Info
    {
        get => _info;
        set => this.RaiseAndSetIfChanged(ref _info, value);
    }

    public IBrush ColorInfo
    {
        get => _colorInfo;
        set => this.RaiseAndSetIfChanged(ref _colorInfo, value);
    }

    public ICommand SaveClick { get; }

    public DataBaseAdminSettingsViewModel()
    {

    }

    public DataBaseAdminSettingsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        SaveClick = new DelegateCommand(SaveSettings);

        _mainWindowViewModel.UserChangedEvent += Refresh;
    }

    public void Refresh(User user)
    {
        SerialList.Clear();
        SerialBaudRateList.Clear();
        Rs485PinReDeList.Clear();

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

        Info = "Успешно";
        ColorInfo = Brush.Parse("#10FF10");
    }
}