namespace IPSCM.GUI
{
    public class UiControl
    {
        private static UiControl _instance;

        private UiControl()
        {
            this.MainWindow = new MainConsoleForm();
            this.LoginWindow = new LoginForm();
        }

        public MainConsoleForm MainWindow { get; private set; }
        public LoginForm LoginWindow { get; private set; }

        public static UiControl GetUiControl()
        {
            return _instance ?? (_instance = new UiControl());
        }

        public void CloseAll()
        {
            this.LoginWindow.Close();
            this.MainWindow.Close();
        }
    }
}