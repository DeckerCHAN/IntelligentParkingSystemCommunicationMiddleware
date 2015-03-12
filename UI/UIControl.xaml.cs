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
            this.Startup += App_Startup;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            this.MainWindow.Show();
        }
    }
}
