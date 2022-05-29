using Avalonia.Media;
using Flotomachine.Services;
using ReactiveUI;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class RegisterUserViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly AdminPanelControlViewModel _adminPanelControlViewModel;

    #region Private

    private string _login;
    private string _passOne;
    private string _passTwo;
    private string _info;
    private IBrush _colorInfo;

    #endregion

    #region PublicGetSet

    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    public string PassOne
    {
        get => _passOne;
        set => this.RaiseAndSetIfChanged(ref _passOne, value);
    }

    public string PassTwo
    {
        get => _passTwo;
        set => this.RaiseAndSetIfChanged(ref _passTwo, value);
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

    #endregion

    public ICommand RegisterUserClick { get; }

    public RegisterUserViewModel()
    {

    }

    public RegisterUserViewModel(MainWindowViewModel mainWindowViewModel, AdminPanelControlViewModel adminPanelControlViewModel)
    {
        RegisterUserClick = new DelegateCommand(RegisterUser);

        _mainWindowViewModel = mainWindowViewModel;
        _adminPanelControlViewModel = adminPanelControlViewModel;
    }

    private void RegisterUser(object parameter)
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(PassOne) || string.IsNullOrWhiteSpace(PassTwo))
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Заполните все поля";
            return;
        }
        User user = DataBaseService.GetUser(Login);
        if (user != null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Логин существует";
            return;
        }

        if (PassOne != PassTwo)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Разные пароли";
            return;
        }

        User newUser = new(Login, PassOne);
        DataBaseService.CreateUser(newUser);

        Login = "";
        PassOne = "";
        PassTwo = "";
        Info = "Успешно";
        ColorInfo = Brush.Parse("#10FF10");
        _adminPanelControlViewModel.RefreshUserList(null);
    }

}