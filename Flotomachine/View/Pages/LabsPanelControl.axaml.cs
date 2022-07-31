using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flotomachine.View
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
