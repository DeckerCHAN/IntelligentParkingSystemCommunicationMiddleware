using System.Windows;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class UIControl : Application
    {
        public LoginWindow LoginWindow { get;private set; }
        public UIControl()
        {
            this.Startup += this.UIControl_Startup;
            this.MainWindow = new MainWindow();
            this.LoginWindow=new LoginWindow();

        }

        void UIControl_Startup(object sender, StartupEventArgs e)
        {
            this.MainWindow.Show();

        }
    }
}
