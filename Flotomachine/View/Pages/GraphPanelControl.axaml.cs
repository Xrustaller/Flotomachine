using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flotomachine.View
{
    public partial class GraphPanelControl : UserControl
    {
        public GraphPanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
