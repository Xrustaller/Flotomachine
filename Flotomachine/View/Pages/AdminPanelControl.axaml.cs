using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Flotomachine.ViewModels;

namespace Flotomachine.View
{
    public partial class AdminPanelControl : UserControl
    {
        public AdminPanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
    }
}
