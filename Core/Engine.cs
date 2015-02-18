using System;
using System.Drawing;
using System.Windows.Forms;
using IPSCM.Configuration;
using IPSCM.Core.Transactions;
using IPSCM.GUI;
using IPSCM.Logging;
using IPSCM.Protocol;
using IPSCM.Protocol.Gates;

namespace IPSCM.Core
{
    class Engine : ApplicationContext
    {
        private static Engine _instance;

        public static Engine GetEngine()
        {
            return _instance ?? (_instance = new Engine());
        }

        #region fields

        public TransactionPool TransactionPool { get; private set; }
        public UiControl UiControl { get; private set; }
        public F3Gate F3Gate { get; set; }
        public CloudParkingGate CloudParking { get; set; }

        #endregion

        private Engine()
        {
            this.UiControl = UiControl.GetUiControl();
            this.MainForm = this.UiControl.MainWindow;
            this.TransactionPool = new TransactionPool();
            this.F3Gate = new F3Gate();
            this.CloudParking = new CloudParkingGate();
            this.ThreadExit += (i, o) => { this.Exit(); };
        }

        void Exit()
        {
            this.F3Gate.Stop();
            this.CloudParking.Stop();
            this.TransactionPool.Dispose();
            foreach (var fileConfig in FileConfig.FileConfigs)
            {
                fileConfig.SaveToFile();
            }
        }

        public void Run()
        {
            this.UiControl.MainWindow.Show();
            this.RegisterEvent();
            Log.Info("Engine starting running...");
            this.F3Gate.Start();
            this.CloudParking.Start();
            Log.Info("Engine started!");
            this.UiControl.LoginWindow.ShowDialog(this.UiControl.MainWindow);
            Application.Run(this);
        }
        public void TryOut(String text, Color color)
        {
            try
            {
                this.UiControl.MainWindow.Invoke(new Action(() => { this.UiControl.MainWindow.Out(text, color); }));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void RegisterEvent()
        {
            Log.OnInfo += i => { this.TryOut(i.Messege, Color.Lime); };
            Log.OnError += i => { this.TryOut(i.Message, Color.Red); };
            this.UiControl.LoginWindow.LoginButton.Click += (i, o) =>
            {
                var username = UiControl.LoginWindow.UserNameTextBox.Text.Clone().ToString();
                var password = UiControl.LoginWindow.PasswordTextBox.Text.Clone().ToString();
                this.TransactionPool.AddBeforeExecute(new LoginTransaction(username, password));
            };

        }

    }
}
