using System.Windows.Input;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class RfidAdminSettingsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private int _busId;
    private int _lineId;
    private int _clockFrequencySpi;

    public int BusId
    {
        get => _busId;
        set => this.RaiseAndSetIfChanged(ref _busId, value);
    }

    public int LineId
    {
        get => _lineId;
        set => this.RaiseAndSetIfChanged(ref _lineId, value);
    }

    public int ClockFrequencySpi
    {
        get => _clockFrequencySpi;
        set => this.RaiseAndSetIfChanged(ref _clockFrequencySpi, value);
    }

    public ICommand SaveClick { get; }

    public RfidAdminSettingsViewModel()
    {

    }

    public RfidAdminSettingsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        SaveClick = new DelegateCommand(SaveSettings);
    }

    private void SaveSettings(object obj)
    {
        
    }
}