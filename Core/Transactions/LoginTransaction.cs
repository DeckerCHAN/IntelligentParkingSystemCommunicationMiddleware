using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IPSCM.Core.Properties;
using IPSCM.Logging;
using IPSCM.Utils;

namespace IPSCM.Core.Transactions
{
    public class LoginTransaction : Transaction
    {
        private Thread ProcessThread;

        public LoginTransaction(String userName, String rowPassword)
            : base()
        {
            this.ProcessThread = new Thread(() =>
            {
                try
                {
                    var name = userName.Clone().ToString();
                    var passEncoded = HashUtils.CalculateMD5Hash(rowPassword).Clone().ToString();
                    var result = Engine.GetEngine().CloudParking.LogIn(name, passEncoded);
                    switch (result.ResultCode)
                    {
                        case 200:
                            {
                                //Success
                                Engine.GetEngine().UiControl.LoginWindow.Resultlabel.Text = Resources.LoginTransaction_LoginTransaction_Success;
                                Engine.GetEngine().UiControl.LoginWindow.Close();
                                break;
                            }
                        default:
                            {
                                //Fault
                                Engine.GetEngine().UiControl.LoginWindow.Resultlabel.Text = result.ErrorMessage;
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Login encountered a error", ex);
                    this.Status = TransactionStatus.Errored;
                }

                this.Status = TransactionStatus.Exhausted;


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
