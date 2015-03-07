using System;
using System.Threading;
using IPSCM.Entities.Results;
using IPSCM.Logging;

namespace IPSCM.Core.Transactions
{
    public class LoginTransaction : Transaction
    {
        private readonly Thread ProcessThread;

        public LoginTransaction(String userName, String rowPassword)
        {
            this.ProcessThread = new Thread(() =>
            {
                try
                {
                    Engine.GetEngine().UiControl.LoginWindow.Invoke(new Action(() =>
                    {
                        Engine.GetEngine().UiControl.LoginWindow.Resultlabel.Text = "Processing";
                        Engine.GetEngine().UiControl.LoginWindow.LoginButton.Enabled = false;
                    }));

                    var name = userName.Clone().ToString();
                    //TODO:Temporarily do not encode
                    //var passEncoded = HashUtils.CalculateMD5Hash(rowPassword).Clone().ToString();
                    var passEncoded = rowPassword;
                    var result = Engine.GetEngine().CloudParking.LogIn(name, passEncoded);
                    switch (result.ResultCode)
                    {
                        case ResultCode.Success:
                            {
                                //Success

                                this.LoginSuccess(result);
                                break;
                            }
                        default:
                            {
                                //Fault
                                Log.Error(String.Format("Cloud parking failer. Cause:{0} Error code:{1}", result.ErrorMessage, result.ResultCode));
                                this.LoginFailure(result.ResultCode, result.ErrorMessage);
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

        private void LoginSuccess(LoginResult result)
        {
            Engine.GetEngine().UiControl.LoginWindow.Invoke(new Action(() =>
            {
                Engine.GetEngine().UiControl.LoginWindow.Resultlabel.Text = "Success";
                Engine.GetEngine().UiControl.LoginWindow.LoginButton.Enabled = true;
                Engine.GetEngine().UiControl.LoginWindow.Hide();

            }));
            Engine.GetEngine().F3Gate.Start();
            Engine.GetEngine().CloudParking.TickThread.Start();
        }

        private void LoginFailure(ResultCode code, String message)
        {
            Engine.GetEngine().UiControl.LoginWindow.Invoke(new Action(() =>
             {
                 Engine.GetEngine().UiControl.LoginWindow.Resultlabel.Text = String.Format("Login failer!({0}){1}", code, message);
                 Engine.GetEngine().UiControl.LoginWindow.LoginButton.Enabled = true;
             }));
        }
    }
}
