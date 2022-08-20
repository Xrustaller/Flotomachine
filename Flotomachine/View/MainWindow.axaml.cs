using System;
using System.Reflection;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Flotomachine.Services;
using Flotomachine.Utility;

namespace Flotomachine.View
{
    public partial class MainWindow : Window
    {
        public Timer _timer;
        public MainWindow()
        {
            InitializeComponent();
            Title = $"Флотомашина v{Assembly.GetEntryAssembly()?.GetName().Version.ToShortString()}";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            _timer = new Timer(15 * 1000);
            _timer.Elapsed += CheckUpdate;
            _timer.Start();
        }

        private void CheckUpdate(object sender, ElapsedEventArgs e)
        {
            if (!UpdateService.NeedUpdate)
            {
                return;
            }

            UpdateWindow win = new(this);
            win.ShowDialog(this);
        }

        public void Full_OnClick(object sender, RoutedEventArgs e) => WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.FullScreen;

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
