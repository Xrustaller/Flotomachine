using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class RfidAdminSettingsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

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