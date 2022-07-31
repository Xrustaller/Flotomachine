using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flotomachine.View
{
    public partial class CameraPanelControl : UserControl
    {
        public CameraPanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
