using System;
using System.Drawing;
using System.Windows.Forms;
using IPSCM.Configuration;
using IPSCM.Core.Transactions;
using IPSCM.GUI;
using IPSCM.Logging;
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
            this.F3Gate = new F3Gate();
            this.CloudParking = new CloudParkingGate();
            this.RegisterEvents();
            this.TransactionPool = new TransactionPool();

        }

        void Exit()
        {
            Log.Info("Stoping Engine");
            foreach (var fileConfig in FileConfig.FileConfigs.Values)
            {
                fileConfig.SaveToFile();
            }
            this.F3Gate.Stop();
            this.CloudParking.Stop();
            this.TransactionPool.Dispose();
            Log.Info("Engine stopped");
        }

        public void Run()
        {
            this.UiControl.MainWindow.Show();
            this.TransactionPool.WipeThread.Start();
            Log.Info("Engine starting running...");
            //F3Gate would start after successful log in.
            this.CloudParking.Start();
            Log.Info("Engine started!");
            this.UiControl.LoginWindow.ShowDialog(this.UiControl.MainWindow);
            Application.Run(this);
        }
        public void TryOut(String text, Color color)
        {
            if (this.UiControl != null) this.UiControl.MainWindow.Invoke(new Action(() => { this.UiControl.MainWindow.Out(text, color); }));
        }

        private void RegisterEvents()
        {
            this.ThreadExit += (i, o) => { this.Exit(); };
            Log.GetLogger().OnInfo += i => { this.TryOut(i.Messege, Color.Lime); };
            Log.GetLogger().OnError += i => { this.TryOut(i.Message, Color.Red); };
            this.UiControl.LoginWindow.LoginButton.Click += (i, o) =>
            {
                var username = this.UiControl.LoginWindow.UserNameTextBox.Text.Clone().ToString();
                var password = this.UiControl.LoginWindow.PasswordTextBox.Text.Clone().ToString();
                this.TransactionPool.AddBeforeExecute(new LoginTransaction(username, password));
            };
            this.F3Gate.OnParking += (i, o) =>
            {
                this.TransactionPool.AddBeforeExecute(new ParkingTransaction(o.PlateNumber, o.InTime, o.InImg, o.Response.OutputStream));
            };
            this.F3Gate.OnLeaving += (i, o) =>
            {
                this.TransactionPool.AddBeforeExecute(new LeavingTransaction(o.PlateNumber, o.OutTime, o.OutImg, o.copeMoney, o.actualMoney, o.TicketId, o.Response.OutputStream));
            };

        }

    }
}
