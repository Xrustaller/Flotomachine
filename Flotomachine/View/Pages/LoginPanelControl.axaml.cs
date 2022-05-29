using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flotomachine.View
{
    public partial class LoginPanelControl : UserControl
    {
        public LoginPanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
