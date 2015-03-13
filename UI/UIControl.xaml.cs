using System.Windows;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class UIControl : Application
    {

        public UIControl()
        {
            this.MainWindow = new MainWindow();
        }
    }
}
