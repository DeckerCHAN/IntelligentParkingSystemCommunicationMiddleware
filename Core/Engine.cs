#region

using System;
using System.Reflection;
using IPSCM.Configuration;
using IPSCM.Core.Transactions;
using IPSCM.Logging;
using IPSCM.Protocol.Gates;
using IPSCM.Persistence.Storage;
using IPSCM.UI;

#endregion

namespace IPSCM.Core
{
    public class Engine
    {
        private static Engine _instance;

        private Engine()
        {
            this.UiControl = new UIControl();
            this.F3Gate = new F3Gate();
            this.CloudParking = new CloudParkingGate();
            this.RegisterEvents();
            this.Storage = new DataPool();
            this.TransactionPool = new TransactionPool();
        }

        public static Engine GetEngine()
        {
            return _instance ?? (_instance = new Engine());
        }

        private void Exit()
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
            this.Storage.Initialize();
            this.TransactionPool.WipeThread.Start();
            Log.Info(String.Format("Engine starting running(version:{0})...",
                Assembly.GetEntryAssembly().GetName().Version));
            //F3Gate would start after successful log in.
            this.CloudParking.Start();
            Log.Info("Engine started!");
            this.UiControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.UiControl.MajorWindow.Show();
                this.UiControl.MajorWindow.MainPage.ParkPage.StatisticsData.ItemsSource =
                    this.Storage.GetParkingHistoryOrderByTime(0,10).DefaultView;
                this.UiControl.LoginWindow.Owner = this.UiControl.MajorWindow;
                this.UiControl.LoginWindow.ShowDialog();
            }));
            this.UiControl.Run();
        }

        private void RegisterEvents()
        {
            this.UiControl.Exit += (i, o) => { this.Exit(); };
            this.UiControl.LoginWindow.LoginButton.Click +=
                (i, o) =>
                {
                    var username = this.UiControl.LoginWindow.UserName.Clone().ToString();
                    var password = this.UiControl.LoginWindow.PasswordTextBox.Text.Clone().ToString();
                    this.TransactionPool.AddBeforeExecute(new LoginTransaction(username, password));
                };
            this.F3Gate.OnParking +=
                (i, o) =>
                {
                    this.TransactionPool.AddBeforeExecute(new ParkingTransaction(o.PlateNumber, o.InTime, o.InImg,
                        o.Response.OutputStream));
                };
            this.F3Gate.OnLeaving +=
                (i, o) =>
                {
                    this.TransactionPool.AddBeforeExecute(new LeavingTransaction(o.PlateNumber, o.OutTime, o.OutImg,
                        o.copeMoney, o.actualMoney, o.TicketId, o.Response.OutputStream));
                };
            this.F3Gate.OnSurplusSpaceUpdate +=
                (i, o) => { this.TransactionPool.AddBeforeExecute(new SurplusSpaceUpdateTransaction(o.SurplusSpace, o.Response.OutputStream)); };
            this.CloudParking.OnHeartBeat +=
                (i, o) => { this.TransactionPool.AddBeforeExecute(new HeartBeatTransaction(o.HeartBeatResult)); };
            this.F3Gate.OnCouponNeed +=
                (i, o) =>
                {
                    this.TransactionPool.AddBeforeExecute(new ExtractCouponTransaction(o.PlateNumber,
                        o.Response.OutputStream));
                };
            this.UiControl.MajorWindow.MainPage.ParkPage.JumpToPageButton.Click+= (i, o) =>
            {
                var page = this.UiControl.MajorWindow.MainPage.ParkPage.CurrentPage;
                var start = page*10 - 10;
                this.UiControl.MajorWindow.MainPage.ParkPage.StatisticsData.ItemsSource =
    this.Storage.GetParkingHistoryOrderByTime(start,start+10).DefaultView;
            };
        }

        #region fields

        public TransactionPool TransactionPool { get; private set; }
        public UIControl UiControl { get; private set; }
        public F3Gate F3Gate { get; set; }
        public CloudParkingGate CloudParking { get; set; }
        public DataPool Storage { get; set; }

        #endregion
    }
}