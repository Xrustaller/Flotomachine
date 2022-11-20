using Avalonia.Controls;
using Avalonia.Threading;
using Flotomachine.Services;
using Flotomachine.View;
using ReactiveUI;
using System;
using System.Timers;
using System.Windows.Input;
using HomePanelControl = Flotomachine.View.HomePanelControl;
using LabsPanelControl = Flotomachine.View.LabsPanelControl;
using SettingsPanelControl = Flotomachine.View.SettingsPanelControl;

namespace Flotomachine.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region LoginForm

    #region Services

    #endregion

    #region Private

    private readonly Window _mainWindow;
    private Timer _timer;

    private bool _cameraButtonIsVisible;
    private bool _homeButtonIsVisible;
    private bool _labsButtonIsVisible;
    private bool _graphButtonIsVisible;
    private bool _settingsButtonIsVisible;
    private bool _adminButtonIsVisible;

    private bool _cameraButtonEnable;
    private bool _homeButtonEnable;
    private bool _labsButtonEnable;
    private bool _graphButtonEnable;
    private bool _settingsButtonEnable;
    private bool _adminButtonEnable;

    private string _login;
    private string _password;

    private InfoViewModel _userUserInfo;

    private User _currentUser = null;
    private UserControlBase _mainContentControl;
    private bool _loginBool;

    #endregion

    #region PublicGetSet

    public event Action<User> UserChangedEvent;

    // SingIn
    public bool CameraButtonIsVisible
    {
        get => _cameraButtonIsVisible;
        set => this.RaiseAndSetIfChanged(ref _cameraButtonIsVisible, value);
    }
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
    public bool GraphButtonIsVisible
    {
        get => _graphButtonIsVisible;
        set => this.RaiseAndSetIfChanged(ref _graphButtonIsVisible, value);
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

    public bool CameraButtonEnable
    {
        get => _cameraButtonEnable;
        set => this.RaiseAndSetIfChanged(ref _cameraButtonEnable, value);
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
    public bool GraphButtonEnable
    {
        get => _graphButtonEnable;
        set => this.RaiseAndSetIfChanged(ref _graphButtonEnable, value);
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

    public UserControlBase MainContentControl
    {
        get => _mainContentControl;
        set => this.RaiseAndSetIfChanged(ref _mainContentControl, value);
    }

    public ICommand CameraButtonClick { get; }
    public ICommand HomeButtonClick { get; }
    public ICommand LabsButtonClick { get; }
    public ICommand GraphButtonClick { get; }
    public ICommand SettingsButtonClick { get; }
    public ICommand AdminButtonClick { get; }

    public ICommand LoginButtonClick { get; }
    public ICommand CardLoginButtonClick { get; }
    public ICommand OutButtonClick { get; }

    public ICommand OnClosed { get; }

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
#if DEBUG
        CameraButtonIsVisible = true;
        HomeButtonIsVisible = true;
        LabsButtonIsVisible = true;
        //GraphButtonIsVisible = true;
        SettingsButtonIsVisible = true;
        AdminButtonIsVisible = true;

        CameraButtonEnable = true;
        HomeButtonEnable = true;
        LabsButtonEnable = true;
        GraphButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
#endif
    }

    public MainWindowViewModel(Window mainWindow)
    {
        _mainWindow = mainWindow;
        UserChangedEvent += RefreshButtons;

        CameraButtonClick = new DelegateCommand(CameraButton);
        HomeButtonClick = new DelegateCommand(HomeButton);
        LabsButtonClick = new DelegateCommand(LabsButton);
        GraphButtonClick = new DelegateCommand(GraphButton);
        SettingsButtonClick = new DelegateCommand(SettingsButton);
        AdminButtonClick = new DelegateCommand(AdminButton);

        LoginButtonClick = new DelegateCommand(LoginButton);
        CardLoginButtonClick = new DelegateCommand(CardLoginButton);
        OutButtonClick = new DelegateCommand(OutButton);

        OnClosed = new DelegateCommand(Closed);

        TestClick = new DelegateCommand(Test);

        if (UpdateService.NeedUpdate && App.Settings.Configuration.Main.CheckUpdatesAtStartUp)
        {
            _timer = new Timer(15 * 1000);
            _timer.Elapsed += CheckUpdate;
            _timer.AutoReset = false;
            _timer.Start();
        }

        CameraButtonIsVisible = true;
        HomeButtonIsVisible = true;

        HomeButton(null);

        //TestText = $"{UpdateService.NeedUpdate} - {UpdateService.NewVersion.ToShortString()}";
    }

    private async void CheckUpdate(object sender, ElapsedEventArgs e)
    {
        _timer?.Stop();
        await Dispatcher.UIThread.InvokeAsync(CheckUpdate);
    }

    private UpdateWindow _updateWindow;
    public void CheckUpdate()
    {
        if (_updateWindow != null)
        {
            return;
        }
        _updateWindow = new UpdateWindow(_mainWindow);
        _updateWindow.Closed += (sender, args) =>
        {
            _updateWindow = null;
        };
        _updateWindow.ShowDialog(_mainWindow);
    }

    private void CameraButton(object parameter)
    {
        ((ViewModelBase)MainContentControl?.DataContext)?.OnDestroy();
        MainContentControl?.OnDestroy();
        MainContentControl = new CameraPanelControl()
        {
            DataContext = new CameraPanelControlViewModel(this)
        };

        CameraButtonEnable = false;
        HomeButtonEnable = true;
        LabsButtonEnable = true;
        GraphButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
    }

    private void HomeButton(object parameter)
    {
        ((ViewModelBase)MainContentControl?.DataContext)?.OnDestroy();
        MainContentControl?.OnDestroy();
        MainContentControl = new HomePanelControl()
        {
            DataContext = new HomePanelControlViewModel(this)
        };

        CameraButtonEnable = true;
        HomeButtonEnable = false;
        LabsButtonEnable = true;
        GraphButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
    }

    private void LabsButton(object parameter)
    {
        ((ViewModelBase)MainContentControl?.DataContext)?.OnDestroy();
        MainContentControl?.OnDestroy();
        MainContentControl = new LabsPanelControl()
        {
            DataContext = new LabsPanelControlViewModel(this)
        };

        CameraButtonEnable = true;
        HomeButtonEnable = true;
        LabsButtonEnable = false;
        GraphButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
    }

    private void GraphButton(object parameter)
    {
        ((ViewModelBase)MainContentControl?.DataContext)?.OnDestroy();
        MainContentControl?.OnDestroy();
        MainContentControl = new GraphPanelControl()
        {
            DataContext = new GraphPanelControlViewModel(this)
        };

        CameraButtonEnable = true;
        HomeButtonEnable = true;
        LabsButtonEnable = true;
        GraphButtonEnable = false;
        SettingsButtonEnable = true;
        AdminButtonEnable = true;
    }

    private void SettingsButton(object parameter)
    {
        ((ViewModelBase)MainContentControl?.DataContext)?.OnDestroy();
        MainContentControl?.OnDestroy();
        MainContentControl = new SettingsPanelControl()
        {
            DataContext = new SettingsPanelControlViewModel(this)
        };

        CameraButtonEnable = true;
        HomeButtonEnable = true;
        LabsButtonEnable = true;
        GraphButtonEnable = true;
        SettingsButtonEnable = false;
        AdminButtonEnable = true;
    }

    private void AdminButton(object parameter)
    {
        ((ViewModelBase)MainContentControl?.DataContext)?.OnDestroy();
        MainContentControl?.OnDestroy();
        MainContentControl = new AdminPanelControl()
        {
            DataContext = new AdminPanelControlViewModel(this)
        };

        CameraButtonEnable = true;
        HomeButtonEnable = true;
        LabsButtonEnable = true;
        GraphButtonEnable = true;
        SettingsButtonEnable = true;
        AdminButtonEnable = false;
    }

    private void OutButton(object obj)
    {
        if (!LoginBool)
        {
            return;
        }

        UserInfo = new InfoViewModel();
        LoginBool = false;
        CurrentUser = null;
    }

    private void LoginButton(object parameter)
    {
        if (LoginBool)
        {
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

        string name = string.IsNullOrEmpty(user.Name) ? user.Username : user.Name;
        UserInfo = new InfoViewModel($"Выполнен вход: {name}", "#10FF10");
        LoginTextBox = "";
        PasswordTextBox = "";
        LoginBool = true;

        CurrentUser = user;
    }

    private async void CardLoginButton(object parameter)
    {
        ReadCardWindow readCard = new ReadCardWindow(App.Settings.Configuration.RfId.BusId, App.Settings.Configuration.RfId.LineId, App.Settings.Configuration.RfId.ClockFrequencySpi);
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

        string name = string.IsNullOrEmpty(user.Name) ? user.Username : user.Name;
        UserInfo = new InfoViewModel($"Выполнен вход: {name}", "#10FF10");
        LoginTextBox = "";
        PasswordTextBox = "";
        LoginBool = true;

        CurrentUser = user;
    }

    private void RefreshButtons(User user)
    {
        ((ViewModelBase)MainContentControl?.DataContext)?.OnDestroy();
        MainContentControl?.OnDestroy();
        MainContentControl = null;

        if (user == null) // Если пользователь не выбран
        {
            CameraButtonIsVisible = true;
            HomeButtonIsVisible = true;
            LabsButtonIsVisible = false;
            //GraphButtonIsVisible = false;
            SettingsButtonIsVisible = false;
            AdminButtonIsVisible = false;
            HomeButton(null);
            return;
        }

        if (user.Root == true) // Если админ
        {
            CameraButtonIsVisible = true;
            HomeButtonIsVisible = true;
            LabsButtonIsVisible = false;
            //GraphButtonIsVisible = false;
            SettingsButtonIsVisible = true;
            AdminButtonIsVisible = true;
#if DEBUG
            CameraButtonIsVisible = true;
            HomeButtonIsVisible = true;
            LabsButtonIsVisible = true;
            //GraphButtonIsVisible = true;
            SettingsButtonIsVisible = true;
            AdminButtonIsVisible = true;
#endif
            AdminButton(null);
            return;
        }

        CameraButtonIsVisible = true;
        HomeButtonIsVisible = true;
        LabsButtonIsVisible = true;
        //GraphButtonIsVisible = true;
        SettingsButtonIsVisible = true;
        AdminButtonIsVisible = false;
        HomeButton(null);
    }

    private void Closed(object parameter)
    {
        ModBusService.Exit();
    }

    private string _testText;

    public string TestText
    {
        get => _testText;
        set => this.RaiseAndSetIfChanged(ref _testText, value);
    }


    public ICommand TestClick { get; }

    private void Test(object parameter)
    {

    }
}