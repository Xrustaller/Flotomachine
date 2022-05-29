using ReactiveUI;

namespace Flotomachine.ViewModels;

public class SettingsPanelControlViewModel : ViewModelBase
{
    private ChangePasswordViewModel _changePasswordViewModel;
    private AddDelUserCardViewModel _addDelUserCardViewModel;

    private readonly MainWindowViewModel _mainWindowViewModel;
    
    public ChangePasswordViewModel ChangePasswordViewModel
    {
        get => _changePasswordViewModel;
        set => this.RaiseAndSetIfChanged(ref _changePasswordViewModel, value);
    }

    public AddDelUserCardViewModel AddDelUserCardViewModel
    {
        get => _addDelUserCardViewModel;
        set => this.RaiseAndSetIfChanged(ref _addDelUserCardViewModel, value);
    }

    public SettingsPanelControlViewModel()
    {

    }

    public SettingsPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        ChangePasswordViewModel = new ChangePasswordViewModel(_mainWindowViewModel);
        AddDelUserCardViewModel = new AddDelUserCardViewModel(_mainWindowViewModel);
    }
}