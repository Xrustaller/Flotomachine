using Flotomachine.Services;
using Flotomachine.Utility;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class AdminPanelControlViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    #region Private

    private string _programInfo = $"Флотомашина v{Assembly.GetEntryAssembly()?.GetName().Version.ToShortString()}";
    private string _workDirectory;
    private string _myDocumentDirectory = App.MyDocumentPath;
    private InfoViewModel _updateInfo;
    private string _updateButtonText;

    private LoginPassViewModel _changePasswordViewModel;
    private LoginPassViewModel _registerUserViewModel;
    private RfidAdminSettingsViewModel _rfidAdminSettingsViewModel;
    private SerialAdminSettingsViewModel _serialAdminSettingsViewModel;
    private User _selectUser;
    private InfoViewModel _userListInfo;
    private bool _isVisibleSystemSetting;

    #endregion

    #region PublicGetSet

    public string ProgramInfo
    {
        get => _programInfo;
        set => this.RaiseAndSetIfChanged(ref _programInfo, value);
    }

    public string MyDocumentDirectory
    {
        get => _myDocumentDirectory;
        set => this.RaiseAndSetIfChanged(ref _myDocumentDirectory, value);
    }

    public string WorkDirectory
    {
        get => _workDirectory;
        set => this.RaiseAndSetIfChanged(ref _workDirectory, value);
    }

    public InfoViewModel UpdateInfo
    {
        get => _updateInfo;
        set => this.RaiseAndSetIfChanged(ref _updateInfo, value);
    }

    public bool AutoUpdate
    {
        get => App.Settings.Configuration.Main.CheckUpdatesAtStartUp;
        set
        {
            App.Settings.Configuration.Main.CheckUpdatesAtStartUp = value;
            App.Settings.SaveConfig();
            this.RaisePropertyChanged();
        }
    }

    public string UpdateButtonText
    {
        get => _updateButtonText;
        set => this.RaiseAndSetIfChanged(ref _updateButtonText, value);
    }

    public bool IsVisibleSystemSetting
    {
        get => _isVisibleSystemSetting;
        set => this.RaiseAndSetIfChanged(ref _isVisibleSystemSetting, value);
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


    public ICommand UpdateClick { get; }

    public ICommand DeleteUserClick { get; }

    public ICommand SystemSettings { get; }


    #endregion

    public AdminPanelControlViewModel()
    {
        if (UpdateService.NeedUpdate)
        {
            UpdateButtonText = "Загрузить и обновить";
            UpdateInfo = new InfoViewModel($"Вышла новая версия: v{UpdateService.NewVersion}", "#FFFF10");
        }
        else
        {
            UpdateButtonText = "Проверить обновления";
            UpdateInfo = new InfoViewModel("Обновление не требуется", "#10FF10");
        }
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

        UpdateClick = new DelegateCommand(Update);

        SystemSettings = new DelegateCommand(_ => IsVisibleSystemSetting = !IsVisibleSystemSetting);

        RfidAdminSettingsViewModel = new RfidAdminSettingsViewModel(_mainWindowViewModel);
        SerialAdminSettingsViewModel = new SerialAdminSettingsViewModel(_mainWindowViewModel);

        foreach (User item in DataBaseService.GetUsers())
        {
            UserList.Add(item);
        }

        if (UpdateService.NeedUpdate)
        {
            UpdateButtonText = "Загрузить и обновить";
            UpdateInfo = new InfoViewModel($"Вышла новая версия: v{UpdateService.NewVersion}", "#FFFF10");
        }
        else
        {
            UpdateButtonText = "Проверить обновления";
            UpdateInfo = new InfoViewModel("Обновление не требуется", "#10FF10");
        }
    }

    private void Update(object obj)
    {
        if (UpdateService.NeedUpdate)
        {
            _mainWindowViewModel.CheckUpdate();
        }
        else
        {
            UpdateButtonText = "Проверить обновления";
            UpdateInfo = new InfoViewModel("Обновление не требуется", "#10FF10");
            UpdateService.CheckUpdates();
        }
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

        DataBaseService.GetAndSet(context =>
        {
            context.Users.Update(user);
        });

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