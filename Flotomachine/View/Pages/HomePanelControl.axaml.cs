using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Flotomachine.View.Pages
{
    public partial class HomePanelControl : UserControl
    {
        public HomePanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
