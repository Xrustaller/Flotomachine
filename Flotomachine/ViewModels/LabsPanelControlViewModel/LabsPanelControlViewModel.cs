namespace Flotomachine.ViewModels
{
    public class LabsPanelControlViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public LabsPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}