using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class SerialAdminSettingsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private string _serialSelectedItem;

    public ObservableCollection<string> SerialList { get; set; } = new();

    public string SerialSelectedItem
    {
        get => _serialSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _serialSelectedItem, value);
    }

    public ICommand SaveClick { get; }

    public SerialAdminSettingsViewModel()
    {

    }

    public SerialAdminSettingsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        SaveClick = new DelegateCommand(SaveSettings);
        _mainWindowViewModel.TappedTabEvent += PanelChanged;
    }

    public override void PanelChanged()
    {
        SerialList.Clear();
        foreach (var item in SerialPort.GetPortNames())
        {
            SerialList.Add(item);
        }
    }

    private void SaveSettings(object obj)
    {
        
    }
}