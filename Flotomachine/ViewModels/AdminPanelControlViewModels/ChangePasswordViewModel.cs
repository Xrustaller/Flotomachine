using Avalonia.Media;
using Flotomachine.Services;
using ReactiveUI;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class ChangePasswordViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

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

    public ICommand ChangePassClick { get; }


    public ChangePasswordViewModel()
    {

    }

    public ChangePasswordViewModel(MainWindowViewModel mainWindowViewModel)
    {
        ChangePassClick = new DelegateCommand(ChangePass);

        _mainWindowViewModel = mainWindowViewModel;
    }

    private void ChangePass(object parameter)
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(PassOne) || string.IsNullOrWhiteSpace(PassTwo))
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Заполните все поля";
            return;
        }

        if (_mainWindowViewModel.CurrentUser.IsRoot())
        {
            User user = DataBaseService.GetUser(Login);
            if (user == null)
            {
                ColorInfo = Brush.Parse("#FF1010");
                Info = "Неверный логин";
                return;
            }

            if (PassOne != PassTwo)
            {
                ColorInfo = Brush.Parse("#FF1010");
                Info = "Разные пароли";
                return;
            }

            user.PassHash = User.GenerateHash(user.Username, PassOne);
            DataBaseService.ChangePassword(user);
        }
        else
        {
            if (PassOne != PassTwo)
            {
                ColorInfo = Brush.Parse("#FF1010");
                Info = "Разные пароли";
                return;
            }

            _mainWindowViewModel.CurrentUser.PassHash = User.GenerateHash(_mainWindowViewModel.CurrentUser.Username, PassOne);
            DataBaseService.ChangePassword(_mainWindowViewModel.CurrentUser);
        }

        Login = "";
        PassOne = "";
        PassTwo = "";
        Info = "Успешно";
        ColorInfo = Brush.Parse("#10FF10");
    }
}