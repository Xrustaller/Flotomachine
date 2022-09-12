using Flotomachine.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class AdminPanelControlViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    #region Private

    private string _workDirectory;
    private LoginPassViewModel _changePasswordViewModel;
    private LoginPassViewModel _registerUserViewModel;
    private RfidAdminSettingsViewModel _rfidAdminSettingsViewModel;
    private SerialAdminSettingsViewModel _serialAdminSettingsViewModel;
    private User _selectUser;
    private InfoViewModel _userListInfo;

    #endregion

    #region PublicGetSet

    public string WorkDirectory
    {
        get => _workDirectory;
        set => this.RaiseAndSetIfChanged(ref _workDirectory, value);
    }

    public LoginPassViewModel ChangePasswordViewModel
    {
        get => _changePasswordViewModel;
        set => this.RaiseAndSetIfChanged(ref _changePasswordViewModel, value);
    }

    public LoginPassViewModel RegisterUserViewModel
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

    public ObservableCollection<User> UserList { get; set; } = new();

    public User SelectUser
    {
        get => _selectUser;
        set
        {
            _selectUser = value;
            if (value == null)
            {
                ChangePasswordViewModel.Login = "";
                RegisterUserViewModel.Login = "";
            }
            else
            {
                ChangePasswordViewModel.Login = value.Username;
                RegisterUserViewModel.Login = value.Username;
            }
        }
    }

    public InfoViewModel UserListInfo
    {
        get => _userListInfo;
        set => this.RaiseAndSetIfChanged(ref _userListInfo, value);
    }

    public ICommand DeleteUserClick { get; }

    #endregion

    public AdminPanelControlViewModel()
    {

    }

    public AdminPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        ChangePasswordViewModel = new LoginPassViewModel
        {
            ButtonClick = new DelegateCommand(ChangePass)
        };

        RegisterUserViewModel = new LoginPassViewModel()
        {
            ButtonClick = new DelegateCommand(RegisterUser)
        };

        DeleteUserClick = new DelegateCommand(DeleteUser);

        RfidAdminSettingsViewModel = new RfidAdminSettingsViewModel(_mainWindowViewModel);
        SerialAdminSettingsViewModel = new SerialAdminSettingsViewModel(_mainWindowViewModel);

        foreach (User item in DataBaseService.GetUsers())
        {
            UserList.Add(item);
        }

        WorkDirectory = App.MyDocumentPath;
    }

    private void RegisterUser(object parameter)
    {
        if (string.IsNullOrWhiteSpace(RegisterUserViewModel.Login) || string.IsNullOrWhiteSpace(RegisterUserViewModel.PassOne) || string.IsNullOrWhiteSpace(RegisterUserViewModel.PassTwo))
        {
            RegisterUserViewModel.Info = new InfoViewModel("Заполните все поля", "#FF1010");
            return;
        }

        User user = DataBaseService.GetUser(RegisterUserViewModel.Login);
        if (user != null)
        {
            RegisterUserViewModel.Info = new InfoViewModel("Логин существует", "#FF1010");
            return;
        }

        if (RegisterUserViewModel.PassOne.Length < 4)
        {
            RegisterUserViewModel.Info = new InfoViewModel("Пароль < 4 символов", "#FF1010");
            return;
        }

        if (RegisterUserViewModel.PassOne != RegisterUserViewModel.PassTwo)
        {
            RegisterUserViewModel.Info = new InfoViewModel("Разные пароли", "#FF1010");
            return;
        }

        User newUser = new(RegisterUserViewModel.Login, RegisterUserViewModel.PassOne);
        DataBaseService.CreateUser(newUser);

        RegisterUserViewModel.Login = "";
        RegisterUserViewModel.PassOne = "";
        RegisterUserViewModel.PassTwo = "";
        RegisterUserViewModel.Info = new InfoViewModel("Успешно", "#10FF10");

        UserList.Clear();
        foreach (User item in DataBaseService.GetUsers())
        {
            UserList.Add(item);
        }
    }

    private void ChangePass(object parameter)
    {
        if (string.IsNullOrWhiteSpace(ChangePasswordViewModel.Login) || string.IsNullOrWhiteSpace(ChangePasswordViewModel.PassOne) || string.IsNullOrWhiteSpace(ChangePasswordViewModel.PassTwo))
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Заполните все поля", "#FF1010");
            return;
        }

        if (ChangePasswordViewModel.PassOne.Length < 4)
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Пароль < 4 символов", "#FF1010");
            return;
        }

        if (ChangePasswordViewModel.PassOne != ChangePasswordViewModel.PassTwo)
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Разные пароли", "#FF1010");
            return;
        }

        User user = DataBaseService.GetUser(ChangePasswordViewModel.Login);
        if (user == null)
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Неверный логин", "#FF1010");
            return;
        }

        user.PassHash = User.GenerateHash(user.Username, ChangePasswordViewModel.PassOne);
        DataBaseService.ChangePassword(user);

        ChangePasswordViewModel.Login = "";
        ChangePasswordViewModel.PassOne = "";
        ChangePasswordViewModel.PassTwo = "";
        ChangePasswordViewModel.Info = new InfoViewModel("Успешно", "#10FF10");
    }

    private void DeleteUser(object parameter)
    {
        if (_mainWindowViewModel.CurrentUser.Root != null)
        {
            UserListInfo = new InfoViewModel("Успешно", "#10FF10");
            return;
        }
    }
}