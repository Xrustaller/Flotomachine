﻿using Flotomachine.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicData;
using Flotomachine.Utility;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class LabsPanelControlViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private int _experimentSelected;
    private bool _visibleExperiment;
    private ObservableCollection<ExpObj> _experiment = new ObservableCollection<ExpObj>();

    public ObservableCollection<int> ExperimentCollection { get; set; } = new();

    public int ExperimentSelected
    {
        get => _experimentSelected;
        set
        {
            _experimentSelected = value;
            ExperimentSelectedChanged(value);
        } 
    }

    public ObservableCollection<ExpObj> Experiment
    {
        get => _experiment;
        set => this.RaiseAndSetIfChanged(ref _experiment, value);
    }

    public bool VisibleExperiment
    {
        get => _visibleExperiment;
        set => this.RaiseAndSetIfChanged(ref _visibleExperiment, value);
    }

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
        ExperimentCollection.AddRange(DataBaseService.GetExperiments(_mainWindowViewModel.CurrentUser));
    }

    public ICommand ExportExcelExperimentButtonClick { get; }
    public ICommand PrintExperimentButtonClick { get; }
    public ICommand DeleteExperimentButtonClick { get; }

    public void ExperimentSelectedChanged(int experimentId)
    {
        VisibleExperiment = true;
        Experiment.Clear();
        foreach (var item in DataBaseService.GetExperimentData(experimentId))
        {
            Experiment.Add(new ExpObj(item));
        }
    }

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