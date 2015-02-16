namespace IPSCM.GUI
{

    public class UiControl
    {
        public MainForm MainWindow { get; private set; }
        public LoginForm LoginWindow { get; private set; }

        public UiControl()
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
