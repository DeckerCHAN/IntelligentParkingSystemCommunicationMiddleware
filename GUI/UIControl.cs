namespace IPSCM.GUI
{

    public class UiControl
    {
        private static UiControl _instance;

        public static UiControl GetUiControl()
        {
            return _instance ?? (_instance = new UiControl());
        }

        public MainForm MainWindow { get; private set; }
        public LoginForm LoginWindow { get; private set; }

        private UiControl()
        {
            this.MainWindow = new MainForm();
            this.LoginWindow = new LoginForm();
        }

        public void CloseAll()
        {
            this.LoginWindow.Close();
            this.MainWindow.Close();
        }


    }
}
