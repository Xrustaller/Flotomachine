using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flotomachine.View
{
    public partial class SettingsPanelControl : UserControl
    {

        public SettingsPanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
