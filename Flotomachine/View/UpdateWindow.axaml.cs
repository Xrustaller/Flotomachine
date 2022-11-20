using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Flotomachine.ViewModels;

namespace Flotomachine.View
{
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = new UpdateWindowViewModel(null, this);
        }

        public UpdateWindow(Window mainWindow)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = new UpdateWindowViewModel(mainWindow, this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
