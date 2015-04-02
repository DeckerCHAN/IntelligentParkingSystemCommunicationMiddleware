using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using IPSCM.Entities.Results;
using IPSCM.UI;

namespace IPSCM.Core.Transactions
{
    public class UpdateTransaction : Transaction
    {
        public Thread ProcessThread;

        public UpdateTransaction()
        {
            this.ProcessThread = new Thread(() =>
            {
                try
                {
                    var currVer = Assembly.GetAssembly(Engine.GetEngine().GetType())
                        .GetName()
                        .Version.ToString();
                    var result = Engine.GetEngine().CloudParking.CheckUpdate();
                    if (result.ResultCode != ResultCode.Success)
                    {
                        Logging.Log.Error(String.Format("Unexpected result code:{0}", result.ResultCode));
                        return;
                    }
                    if (!currVer.Equals(result.Info.Version))
                    {
                        var window = Engine.GetEngine().UiControl.UpdateCheckWindow;
                        Engine.GetEngine().UiControl.Dispatcher.Invoke(new Action(() =>
                        {
                            window.ContentString = String.Format(UI.Properties.Resources.DetectedNewerVersion,result.Info.Version);
                            window.Owner = Engine.GetEngine().UiControl.MajorWindow;
                            window.Show();
                        }));
                        var res = window.ConfirmUpdate;
                        if (res)
                        {
                            Process.Start("IPSCMUpdater.exe",
                                String.Format("{0} {1} {2}", currVer, result.Info.Version, result.Info.DownloadUrl));
                        }
                        else
                        {
                            Logging.Log.Info("Update canceled!");
                        }
                    }
                    else
                    {
                        new PopupWindow(Engine.GetEngine().UiControl.MajorWindow, UI.Properties.Resources.CheckUpdate,
                            UI.Properties.Resources.NoUpdates).ShowDialog();
                    }
                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    this.Status = TransactionStatus.Errored;
                    Logging.Log.Error("Update transaction encountered a bad error!", ex);
                }

            });
        }
        public override void Execute()
        {
            this.ProcessThread.Start();
            base.Execute();
        }

        public override void Interrupt()
        {
            this.ProcessThread.Interrupt();
            base.Interrupt();
        }
    }
}
