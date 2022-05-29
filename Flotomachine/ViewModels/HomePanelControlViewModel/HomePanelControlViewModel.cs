namespace Flotomachine.ViewModels
{
    public class HomePanelControlViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public HomePanelControlViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}