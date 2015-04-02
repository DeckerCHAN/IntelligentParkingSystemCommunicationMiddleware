using System;
using System.Windows;

namespace IPSCM.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class UIControl : Application
    {
        public LoginWindow LoginWindow { get;private set; }
        public MainWindow MajorWindow { get; private set; }

        public UpdateCheckWindow UpdateCheckWindow
        {
            get
            {
                UpdateCheckWindow window = null;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    window=new UpdateCheckWindow();
                }));
                return window;


            }
            private set { throw new NotImplementedException(); }
        }

        public UIControl()
        {
            this.MajorWindow = new MainWindow();
            this.LoginWindow=new LoginWindow();
        }

        private void UIControl_OnStartup(object sender, StartupEventArgs e)
        {

        }
    }
}
