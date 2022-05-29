using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flotomachine.View.Pages
{
    public partial class LabsPanelControl : UserControl
    {
        public LabsPanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
