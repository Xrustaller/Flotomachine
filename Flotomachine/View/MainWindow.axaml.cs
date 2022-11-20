using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Flotomachine.Utility;
using System.Reflection;

namespace Flotomachine.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = $"Флотомашина v{Assembly.GetEntryAssembly()?.GetName().Version.ToShortString()}";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Full_OnClick(object sender, RoutedEventArgs e) => WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.FullScreen;

        private void Close_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}
