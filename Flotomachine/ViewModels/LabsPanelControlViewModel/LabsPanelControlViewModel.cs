using Flotomachine.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class LabsPanelControlViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private Experiment _experimentSelected;

    public ObservableCollection<Experiment> ExperimentCollection { get; set; } = new ObservableCollection<Experiment>();

    public Experiment ExperimentSelected
    {
        get => _experimentSelected;
        set => _experimentSelected = value;
    }

    public object Experiment { get; }

    public bool VisibleExperiment { get; set; } = true;

    public LabsPanelControlViewModel()
    {

    }

    public LabsPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        ExportExcelExperimentButtonClick = new DelegateCommand(ExportExcelExperiment);
        PrintExperimentButtonClick = new DelegateCommand(PrintExperiment);
        DeleteExperimentButtonClick = new DelegateCommand(DeleteExperiment);

        ExperimentCollection.Clear();
        for (int i = 99999990; i < 100000100; i++)
        {
            ExperimentCollection.Add(new Experiment() { DateStart = DateTime.Now, Id = i });
        }
    }

    public ICommand ExportExcelExperimentButtonClick { get; }
    public ICommand PrintExperimentButtonClick { get; }
    public ICommand DeleteExperimentButtonClick { get; }

    public void ExportExcelExperiment(object obj)
    {

    }

    public void PrintExperiment(object obj)
    {

    }

    public void DeleteExperiment(object obj)
    {

    }
}