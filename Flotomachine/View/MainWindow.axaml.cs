using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using System;
using Avalonia.Interactivity;

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


        bool full = false;

        public void Full(object sender, RoutedEventArgs e)
        {
            WindowState = full ? WindowState.Maximized : WindowState.FullScreen;
            full = !full;
        }
        
    }
}
