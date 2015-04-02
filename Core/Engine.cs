#region

using System;
using System.IO;
using System.Reflection;
using IPSCM.Configuration;
using IPSCM.Core.Transactions;
using IPSCM.Logging;
using IPSCM.Persistence;
using IPSCM.Protocol.Gates;
using IPSCM.UI;
using IPSCM.Utils;

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
            this.UiControl.Shutdown();
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
                Assembly.GetAssembly(this.GetType()).GetName().Version));
            //F3Gate would start after successful log in.
            this.CloudParking.Start();
            Log.Info("Engine started!");
            this.UiControl.Dispatcher.Invoke(new Action(() =>
            {
                this.UiControl.MajorWindow.Show();
                this.UiControl.MajorWindow.MainPage.ParkPage.RefreshData
                    (
                    this.Storage.GetTodayParking()
                    );
                this.UiControl.MajorWindow.MainPage.TicketPage.RefreshData
                    (
                  this.Storage.GetTodayTicketUsed()
                  );
                this.UiControl.LoginWindow.Owner = this.UiControl.MajorWindow;
                this.UiControl.LoginWindow.UserNameTextBox.Text =
                    LoginUtils.ReadPerservedAccount(
                        new FileInfo(FileConfig.FindConfig("GUI.cfg").GetString("PERSERVEACCOUNTFILENAME"))).Item1;
                this.UiControl.LoginWindow.PasswordTextBox.Password =
                LoginUtils.ReadPerservedAccount(
                    new FileInfo(FileConfig.FindConfig("GUI.cfg").GetString("PERSERVEACCOUNTFILENAME"))).Item2;

                this.UiControl.LoginWindow.ShowDialog();
            }));
            this.UiControl.Run();
        }

        private void RegisterEvents()
        {
            this.UiControl.LoginWindow.LoginButton.Click +=
                (i, o) =>
                {
                    var username = this.UiControl.LoginWindow.UserNameTextBox.Text.Clone().ToString();
                    var password = this.UiControl.LoginWindow.PasswordTextBox.Password.Clone().ToString();
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
            this.F3Gate.OnImageUpdate +=
                (i, o) =>
                {
                    this.TransactionPool.AddBeforeExecute(new ImageUpdateTransaction(o.PlateNumber, o.Time, o.Type, o.Image, o.Response.OutputStream));
                };
            //When parking page refresh button pressed
            this.UiControl.MajorWindow.MainPage.ParkPage.RefreshButton.Click += (i, o) =>
            {
                this.UiControl.MajorWindow.MainPage.ParkPage.RefreshData
                (
                this.Storage.GetTodayParking()
                );
            };
            //When parking page search button pressed
            this.UiControl.MajorWindow.MainPage.ParkPage.SearchButton.Click += (i, o) =>
            {
                var keyWord = this.UiControl.MajorWindow.MainPage.ParkPage.SearchKeyWork;
                this.UiControl.MajorWindow.MainPage.ParkPage.RefreshData
                (
                String.IsNullOrEmpty(keyWord) ? this.Storage.GetTodayParking() : this.Storage.GetTodayParking(keyWord)
                );
            };
            //When ticket page refresh buttion pressed
            this.UiControl.MajorWindow.MainPage.TicketPage.RefreshButton.Click += (i, o) =>
            {
                this.UiControl.MajorWindow.MainPage.TicketPage.RefreshData
                      (
                    this.Storage.GetTodayTicketUsed()
                    );
            };
            //When ticket page search button pressed
            this.UiControl.MajorWindow.MainPage.TicketPage.SearchButton.Click += (i, o) =>
            {
                var keyWord = this.UiControl.MajorWindow.MainPage.TicketPage.SearchKeyWord;
                this.UiControl.MajorWindow.MainPage.TicketPage.RefreshData
                      (
                    this.Storage.GetTodayTicketUsed(keyWord)
                    );
            };

            this.UiControl.MajorWindow.CheckUpdateButton.Click += (i, o) =>
            {
                this.TransactionPool.AddBeforeExecute(new UpdateTransaction());
            };
            this.UiControl.MajorWindow.ExitButton.Click += (i, o) =>
            {
                this.Exit();
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