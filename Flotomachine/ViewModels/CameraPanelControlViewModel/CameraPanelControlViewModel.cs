namespace Flotomachine.ViewModels
{
    public class CameraPanelControlViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        public CameraPanelControlViewModel()
        {
        }

        public CameraPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

    }
}