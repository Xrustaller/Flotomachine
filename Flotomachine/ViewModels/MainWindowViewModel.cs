using Flotomachine.Services;
using Flotomachine.View;
using Flotomachine.View.Pages;
using ReactiveUI;
using System;
using System.Windows.Input;
using SettingsPanelControl = Flotomachine.View.SettingsPanelControl;

namespace Flotomachine.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region LoginForm

    #region Private

    private bool _homeTabItemIsVisible;
    private bool _labsTabItemIsVisible;
    private bool _settingsTabItemIsVisible;
    private bool _adminTabItemIsVisible;

    private User _currentUser = null;

    #endregion

    #region PublicGetSet

    public event Action<User> UserChangedEvent;

    // SingIn
    public bool HomeTabItemIsVisible
    {
        get => _homeTabItemIsVisible;
        set => this.RaiseAndSetIfChanged(ref _homeTabItemIsVisible, value);
    }

    public bool LabsTabItemIsVisible
    {
        get => _labsTabItemIsVisible;
        set => this.RaiseAndSetIfChanged(ref _labsTabItemIsVisible, value);
    }

    public bool SettingsTabItemIsVisible
    {
        get => _settingsTabItemIsVisible;
        set => this.RaiseAndSetIfChanged(ref _settingsTabItemIsVisible, value);
    }

    public bool AdminTabItemIsVisible
    {
        get => _adminTabItemIsVisible;
        set => this.RaiseAndSetIfChanged(ref _adminTabItemIsVisible, value);
    }

    public LoginPanelControl LoginUserControl { get; set; }
    public HomePanelControl HomeUserControl { get; set; }
    public LabsPanelControl LabsUserControl { get; set; }
    public SettingsPanelControl SettingsUserControl { get; set; }
    public AdminPanelControl AdminSettingsUserControl { get; set; }

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
        LoginUserControl = new LoginPanelControl()
        {
            DataContext = new LoginPanelControlViewModel(this)
        };

        HomeUserControl = new HomePanelControl()
        {
            DataContext = new HomePanelControlViewModel(this)
        };

        LabsUserControl = new LabsPanelControl()
        {
            DataContext = new LabsPanelControlViewModel(this)
        };

        SettingsUserControl = new SettingsPanelControl()
        {
            DataContext = new SettingsPanelControlViewModel(this)
        };

        AdminSettingsUserControl = new AdminPanelControl()
        {
            DataContext = new AdminPanelControlViewModel(this)
        };

        UserChangedEvent += RefreshTabs;

#if DEBUG
        AdminTabItemIsVisible = true;
        HomeTabItemIsVisible = true;
        LabsTabItemIsVisible = true;
        SettingsTabItemIsVisible = true;
#endif
    }

    private void RefreshTabs(User user)
    {
        if (user == null)
        {
            HomeTabItemIsVisible = false;
            LabsTabItemIsVisible = false;
            SettingsTabItemIsVisible = false;
            AdminTabItemIsVisible = false;
            return;
        }

        if (user.Root == 1)
        {
            AdminSettingsUserControl.DataContext = new AdminPanelControlViewModel(this);

            AdminTabItemIsVisible = true;
            return;
        }

        HomeUserControl.DataContext = new HomePanelControlViewModel(this);
        LabsUserControl.DataContext = new LabsPanelControlViewModel(this);
        SettingsUserControl.DataContext = new SettingsPanelControlViewModel(this);

        HomeTabItemIsVisible = true;
        LabsTabItemIsVisible = true;
        SettingsTabItemIsVisible = true;
    }
}