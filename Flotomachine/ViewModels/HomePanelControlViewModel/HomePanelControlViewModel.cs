using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using Flotomachine.Services;
using Flotomachine.View;

namespace Flotomachine.ViewModels;

public class HomePanelControlViewModel : ViewModelBase
{
    public ObservableCollection<HomeModuleDataPanelControl> Modules { get; set; }

    public HomePanelControlViewModel()
    {

    }

    public HomePanelControlViewModel(MainWindowViewModel mainWindowViewModel)
    {
        ModBusService.DataCollected += ModBusServiceOnDataCollected;
        Modules = new ObservableCollection<HomeModuleDataPanelControl>();
    }

    private async void ModBusServiceOnDataCollected(List<HomeModuleDataViewModel> data)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Modules.Clear();
            foreach (HomeModuleDataViewModel item in data)
            {
                Modules.Add(new HomeModuleDataPanelControl() { DataContext = item });
            }
        });
    }

    public override void OnDestroy()
    {
        ModBusService.DataCollected -= ModBusServiceOnDataCollected;
    }
}