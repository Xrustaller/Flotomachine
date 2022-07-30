using ReactiveUI;

namespace Flotomachine.ViewModels
{
    public class HomePanelControlViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private string _t1Value = "NULL";
        private string _t2Value = "NULL";
        private string _gradValue = "NULL";
        private string _davValue = "NULL";
        private string _vesValue = "NULL";
        private string _oborValue = "NULL";

        public HomePanelControlViewModel()
        {
        }

        public HomePanelControlViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public string T1Value
        {
            get => _t1Value;
            set => this.RaiseAndSetIfChanged(ref _t1Value, value);
        }

        public string T2Value
        {
            get => _t2Value;
            set => this.RaiseAndSetIfChanged(ref _t2Value, value);
        }

        public string GradValue
        {
            get => _gradValue;
            set => this.RaiseAndSetIfChanged(ref _gradValue, value);
        }

        public string DavValue
        {
            get => _davValue;
            set => this.RaiseAndSetIfChanged(ref _davValue, value);
        }

        public string VesValue
        {
            get => _vesValue;
            set => this.RaiseAndSetIfChanged(ref _vesValue, value);
        }

        public string OborValue
        {
            get => _oborValue;
            set => this.RaiseAndSetIfChanged(ref _oborValue, value);
        }
    }
}