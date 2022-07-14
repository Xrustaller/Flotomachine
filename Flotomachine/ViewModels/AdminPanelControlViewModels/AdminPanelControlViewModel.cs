using Flotomachine.Services;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Flotomachine.ViewModels;

public class AdminPanelControlViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    #region Private

    private ChangePasswordViewModel _changePasswordViewModel;
    private RegisterUserViewModel _registerUserViewModel;
    private RfidAdminSettingsViewModel _rfidAdminSettingsViewModel;
    private SerialAdminSettingsViewModel _serialAdminSettingsViewModel;

    #endregion

    #region PublicGetSet

    public ChangePasswordViewModel ChangePasswordViewModel
    {
        get => _changePasswordViewModel;
        set => this.RaiseAndSetIfChanged(ref _changePasswordViewModel, value);
    }

    public RegisterUserViewModel RegisterUserViewModel
    {
        get => _registerUserViewModel;
        set => this.RaiseAndSetIfChanged(ref _registerUserViewModel, value);
    }

    public RfidAdminSettingsViewModel RfidAdminSettingsViewModel
    {
        get => _rfidAdminSettingsViewModel;
        set => this.RaiseAndSetIfChanged(ref _rfidAdminSettingsViewModel, value);
    }

    public SerialAdminSettingsViewModel SerialAdminSettingsViewModel
    {
        get => _serialAdminSettingsViewModel;
        set => this.RaiseAndSetIfChanged(ref _serialAdminSettingsViewModel, value);
    }

    #endregion

    public ObservableCollection<string> UserList { get; set; } = new();

    public AdminPanelControlViewModel()
    {

    }

    public AdminPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        ChangePasswordViewModel = new ChangePasswordViewModel(_mainWindowViewModel);
        RegisterUserViewModel = new RegisterUserViewModel(_mainWindowViewModel, this);
        RfidAdminSettingsViewModel = new RfidAdminSettingsViewModel(_mainWindowViewModel);
        SerialAdminSettingsViewModel = new SerialAdminSettingsViewModel(_mainWindowViewModel);

        _mainWindowViewModel.UserChangedEvent += Refresh;
    }

    public void Refresh(User user)
    {
        RefreshUserList();
    }

    public void RefreshUserList()
    {
        UserList.Clear();
        foreach (User item in DataBaseService.GetUsers())
        {
            UserList.Add(item.Username);
        }
    }
}