using Avalonia.Media;
using Flotomachine.Services;
using Flotomachine.View;
using ReactiveUI;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class LoginPanelControlViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private string _login;
    private string _password;
    private bool _loginFormVisible = true;
    private string _info;
    private IBrush _colorInfo;

    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
    public bool LoginFormVisible
    {
        get => _loginFormVisible;
        set => this.RaiseAndSetIfChanged(ref _loginFormVisible, value);
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

    public ICommand SingInCardClick { get; }
    public ICommand SingInClick { get; }
    public ICommand SingOutClick { get; }

    public LoginPanelControlViewModel()
    {

    }

    public LoginPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
    {
        SingInCardClick = new DelegateCommand(SingInWithСardId);
        SingInClick = new DelegateCommand(SingIn);
        SingOutClick = new DelegateCommand(SingOut);

        _mainWindowViewModel = mainWindowViewModel;
    }

    private async void SingInWithСardId(object parameter)
    {
        ReadCardWindow readCard = new ReadCardWindow();
        var result = await readCard.ShowDialog<byte[]>(App.MainWindow);

        if (result == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Считывание отменено";
            return;
        }

        User user = DataBaseService.GetUser(result);
        if (user == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Карта не зарегистрирована";
            return;
        }

        ColorInfo = Brush.Parse("#10FF10");
        Info = "Выполнен вход: " + user.Username;
        Login = "";
        Password = "";
        LoginFormVisible = false;

        _mainWindowViewModel.CurrentUser = user;
    }

    private void SingIn(object parameter)
    {
        User user = DataBaseService.GetUser(Login);
        if (user == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Неверный логин";
            return;
        }

        if (!user.CheckPass(Login, Password))
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Неверный пароль";
            return;
        }

        ColorInfo = Brush.Parse("#10FF10");
        Info = "Выполнен вход: " + user.Username;
        Login = "";
        Password = "";
        LoginFormVisible = false;

        _mainWindowViewModel.CurrentUser = user;
    }

    private void SingOut(object parameter)
    {
        Info = "";
        LoginFormVisible = true;
        _mainWindowViewModel.CurrentUser = null;
    }
}