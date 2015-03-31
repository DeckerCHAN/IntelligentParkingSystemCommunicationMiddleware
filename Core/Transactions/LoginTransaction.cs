#region

using System;
using System.IO;
using System.Threading;
using System.Windows;
using IPSCM.Configuration;
using IPSCM.Entities.Results;
using IPSCM.Logging;
using IPSCM.Utils;

#endregion

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

                    Engine.GetEngine().UiControl.LoginWindow.ResultString = "Processing";
                    Engine.GetEngine().UiControl.LoginWindow.IsLoginEnable = false;

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
                                Log.Error(String.Format("Cloud parking failer. Cause:{0} Error code:{1}",
                                    result.ErrorMsg, result.ResultCode));
                                this.LoginFailure(result.ResultCode, result.ErrorMsg);
                                break;
                            }
                    }

                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    Log.Error("Login encountered a error", ex);
                    this.Status = TransactionStatus.Errored;
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

        private void LoginSuccess(LoginResult result)
        {


            Engine.GetEngine().UiControl.LoginWindow.Dispatcher.Invoke(new Action(() =>
            {
                Engine.GetEngine().UiControl.LoginWindow.ResultString = "Success";
                Engine.GetEngine().UiControl.LoginWindow.IsLoginEnable = true;
                Engine.GetEngine().UiControl.LoginWindow.Visibility = Visibility.Collapsed;
            }));
            Engine.GetEngine().UiControl.LoginWindow.Dispatcher.Invoke(new Action(() =>
            {
                if (Engine.GetEngine().UiControl.LoginWindow.PerserverAccount)
                {

                    LoginUtils.PerserveUserNameAndPasswordToFile(
             new FileInfo(FileConfig.FindConfig("GUI.cfg").GetString("PERSERVEACCOUNTFILENAME")),
             Engine.GetEngine().UiControl.LoginWindow.UserNameTextBox.Text,
             Engine.GetEngine().UiControl.LoginWindow.PasswordTextBox.Password);
                }
                else
                {
                    LoginUtils.PerserveUserNameAndPasswordToFile(
             new FileInfo(FileConfig.FindConfig("GUI.cfg").GetString("PERSERVEACCOUNTFILENAME")),
             String.Empty,
             String.Empty);
                }
            }));

            Engine.GetEngine().F3Gate.Start();
            Engine.GetEngine().CloudParking.TickThread.Start();
        }

        private void LoginFailure(ResultCode code, String message)
        {
            Engine.GetEngine().UiControl.LoginWindow.Dispatcher.Invoke(new Action(() =>
            {
                Engine.GetEngine().UiControl.LoginWindow.ResultString =
                    String.Format("Login failer!({0}){1}", code,
                        message);
                Engine.GetEngine().UiControl.LoginWindow.IsLoginEnable = true;
            }));

        }
    }
}