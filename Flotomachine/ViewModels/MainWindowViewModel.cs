using Flotomachine.Services;
using Flotomachine.View;
using Flotomachine.View.Pages;
using ReactiveUI;
using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media;
using LabsPanelControl = Flotomachine.View.LabsPanelControl;
using SettingsPanelControl = Flotomachine.View.SettingsPanelControl;

namespace Flotomachine.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region LoginForm

    #region Private

    private bool _homeButtonIsVisible;
    private bool _labsButtonIsVisible;
    private bool _settingsButtonIsVisible;
    private bool _adminButtonIsVisible;

    private bool _homeButtonEnable;
    private bool _labsButtonEnable;
    private bool _settingsButtonEnable;
    private bool _adminButtonEnable;

    private string _login;
    private string _password;

    private InfoViewModel _userUserInfo;

    private User _currentUser = null;
    private UserControl _mainContentControl;
    private bool _loginBool;

    #endregion

    #region PublicGetSet

    public event Action<User> UserChangedEvent;

    // SingIn
    public bool HomeButtonIsVisible
    {
        get => _homeButtonIsVisible;
        set => this.RaiseAndSetIfChanged(ref _homeButtonIsVisible, value);
    }
    public bool LabsButtonIsVisible
    {
        get => _labsButtonIsVisible;
        set => this.RaiseAndSetIfChanged(ref _labsButtonIsVisible, value);
    }
    public bool SettingsButtonIsVisible
    {
        get => _settingsButtonIsVisible;
        set => this.RaiseAndSetIfChanged(ref _settingsButtonIsVisible, value);
    }
    public bool AdminButtonIsVisible
    {
        get => _adminButtonIsVisible;
        set => this.RaiseAndSetIfChanged(ref _adminButtonIsVisible, value);
    }

    public bool HomeButtonEnable
    {
        get => _homeButtonEnable;
        set => this.RaiseAndSetIfChanged(ref _homeButtonEnable, value);
    }
    public bool LabsButtonEnable
    {
        get => _labsButtonEnable;
        set => this.RaiseAndSetIfChanged(ref _labsButtonEnable, value);
    }
    public bool SettingsButtonEnable
    {
        get => _settingsButtonEnable;
        set => this.RaiseAndSetIfChanged(ref _settingsButtonEnable, value);
    }
    public bool AdminButtonEnable
    {
        get => _adminButtonEnable;
        set => this.RaiseAndSetIfChanged(ref _adminButtonEnable, value);
    }

    public string LoginTextBox
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }
    public string PasswordTextBox
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public bool LoginBool
    {
        get => _loginBool;
        set => this.RaiseAndSetIfChanged(ref _loginBool, value);
    }

    public InfoViewModel UserInfo
    {
        get => _userUserInfo;
        set => this.RaiseAndSetIfChanged(ref _userUserInfo, value);
    }

    public UserControl MainContentControl
    {
        get => _mainContentControl;
        set => this.RaiseAndSetIfChanged(ref _mainContentControl, value);
    }

    public ICommand HomeButtonClick { get; }
    public ICommand LabsButtonClick { get; }
    public ICommand SettingsButtonClick { get; }
    public ICommand AdminButtonClick { get; }

    public ICommand LoginButtonClick { get; }
    public ICommand CardLoginButtonClick { get; }

    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            UserChangedEvent?.Invoke(value);
        }
    }

    #endregion

    #endregion

    public MainWindowViewModel()
    {
        UserChangedEvent += RefreshButtons;

        HomeButtonClick = new DelegateCommand(HomeButton);
        LabsButtonClick = new DelegateCommand(LabsButton);
        SettingsButtonClick = new DelegateCommand(SettingsButton);
        AdminButtonClick = new DelegateCommand(AdminButton);

        LoginButtonClick = new DelegateCommand(LoginButton);
        CardLoginButtonClick = new DelegateCommand(CardLoginButton);

#if DEBUG
        AdminButtonIsVisible = true;
        HomeButtonIsVisible = true;
        LabsButtonIsVisible = true;
        SettingsButtonIsVisible = true;

        HomeButtonEnable = true;
        LabsButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
#endif
    }

    private void HomeButton(object parameter)
    {
        MainContentControl = new HomePanelControl()
        {
            DataContext = new HomePanelControlViewModel(this)
        };

        HomeButtonEnable = false;
        LabsButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
    }

    private void LabsButton(object parameter)
    {
        MainContentControl = new LabsPanelControl()
        {
            DataContext = new LabsPanelControlViewModel(this)
        };

        HomeButtonEnable = true;
        LabsButtonEnable = false;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
    }

    private void SettingsButton(object parameter)
    {
        MainContentControl = new SettingsPanelControl()
        {
            DataContext = new SettingsPanelControlViewModel(this)
        };

        HomeButtonEnable = true;
        LabsButtonEnable = true;
        SettingsButtonEnable = false;
        AdminButtonEnable = true;
    }

    private void AdminButton(object parameter)
    {
        MainContentControl = new AdminPanelControl()
        {
            DataContext = new AdminPanelControlViewModel(this)
        };

        HomeButtonEnable = true;
        LabsButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = false;
    }

    private void LoginButton(object parameter)
    {
        if (CurrentUser != null)
        {
            UserInfo = new InfoViewModel();
            LoginBool = false;
            CurrentUser = null;
            return;
        }

        User user = DataBaseService.GetUser(LoginTextBox);
        if (user == null)
        {
            UserInfo = new InfoViewModel("Неверный логин", "#FF1010");
            return;
        }

        if (!user.CheckPass(LoginTextBox, PasswordTextBox))
        {
            UserInfo = new InfoViewModel("Неверный пароль", "#FF1010");
            return;
        }

        UserInfo = new InfoViewModel("Выполнен вход: " + user.Username, "#10FF10");
        LoginTextBox = "";
        PasswordTextBox = "";
        LoginBool = true;

        CurrentUser = user;
    }
    private async void CardLoginButton(object parameter)
    {
        ReadCardWindow readCard = new ReadCardWindow();
        var result = await readCard.ShowDialog<byte[]>(App.MainWindow);

        if (result == null)
        {
            UserInfo = new InfoViewModel("Считывание отменено", "#FF1010");
            return;
        }

        User user = DataBaseService.GetUser(result);
        if (user == null)
        {
            UserInfo = new InfoViewModel("Карта не зарегистрирована", "#FF1010");
            return;
        }

        UserInfo = new InfoViewModel("Выполнен вход: " + user.Username, "#10FF10");
        LoginTextBox = "";
        PasswordTextBox = "";
        LoginBool = true;

        CurrentUser = user;
    }

    private void RefreshButtons(User user)
    {
        MainContentControl = null;

        HomeButtonEnable = true;
        LabsButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;

        if (user == null)
        {
            AdminButtonIsVisible = false;
            HomeButtonIsVisible = false;
            LabsButtonIsVisible = false;
            SettingsButtonIsVisible = false;
            HomeButton(null);
            return;
        }

        if (user.IsRoot())
        {
            AdminButtonIsVisible = true;
            AdminButton(null);
            return;
        }

        HomeButtonIsVisible = true;
        LabsButtonIsVisible = true;
        SettingsButtonIsVisible = true;
    }
}