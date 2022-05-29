using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using System;

namespace Flotomachine.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            ModBusService.Exit();
            base.OnClosed(e);
        }

        private void SelectingItemsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
