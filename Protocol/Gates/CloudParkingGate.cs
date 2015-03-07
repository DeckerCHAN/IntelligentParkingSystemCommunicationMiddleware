#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Entities;
using IPSCM.Entities.Results;
using IPSCM.Entities.Results.Leaving;
using IPSCM.Entities.Results.Parking;
using IPSCM.Logging;
using IPSCM.Protocol.EventArgs;
using IPSCM.Utils;

#endregion

namespace IPSCM.Protocol.Gates
{
    public delegate void HeartBeatEventHandler(object sender, HeartBeatEventArgs arg);

    public delegate void LoginEventHandler(object sender, LoginEvenArgs arg);

    public class CloudParkingGate : ControllableObject, ISend
    {
        public CloudParkingGate()
        {
            this.Config = FileConfig.FindConfig("Parking.cfg");
            this.SecurityKey = Config.GetString("SecurityKey");
            this.Token = String.Empty;
            this.TickThread = new Thread(this.Tick);
        }

        public Thread TickThread { get; private set; }
        public String Token { get; private set; }
        public Config Config { get; private set; }
        private String SecurityKey { get; set; }

        public string Send(string url, Dictionary<string, string> textData, Dictionary<string, byte[]> rowData)
        {
            var sign = this.GetSign(textData);
            var requestContent = new MultipartFormDataContent();
            if (!String.IsNullOrEmpty(this.Token))
            {
                requestContent.Headers.Add(this.Config.GetString("Token"), this.Token);
            }
            requestContent.Headers.Add(Config.GetString("Sign"), sign);
            foreach (var key in textData.Keys)
            {
                requestContent.Add(new StringContent(textData[key]), key);
            }
            foreach (var key in rowData.Keys)
            {
                requestContent.Add(new ByteArrayContent(rowData[key]), key, key);
            }
            String result;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url, requestContent).Result;
                result = response.Content.ReadAsStringAsync().Result;
            }
            return result;
        }

        public event HeartBeatEventHandler OnHeartBeat;
        public event LoginEventHandler OnLoggedin;

        public override void Start()
        {
            Log.Info("CloudParking Starting...");
            base.Start();
            Log.Info("CloudParking Started");
        }

        public override void Stop()
        {
            Log.Info("CloudParking Stoping...");
            base.Stop();
            this.TickThread.Interrupt();
            Log.Info("CloudParking Stopped");
        }

        public LoginResult LogIn(String userName, String password)
        {
            var data = new Dictionary<string, string>();
            data.Add(Config.GetString("UserName"), userName);
            data.Add(Config.GetString("Password"), password);
            var responseJson = this.Send(this.Config.GetString("LoginUrl"), data, new Dictionary<string, byte[]>());
            var loginRes = IPSCMJsonConvert.Parse<LoginResult>(responseJson);
            if (loginRes.ResultCode == ResultCode.Success)
            {
                Log.Info(String.Format("Cloud parking login successful. Preserved token:{0}", loginRes.Info.Token));
                this.Token = loginRes.Info.Token;
            }


            var trigger = this.OnLoggedin;
            if (trigger != null) trigger(this, new LoginEvenArgs(loginRes));
            return loginRes;
        }

        public ParkingResult Parking(String plateNumber, DateTime inTime, Byte[] inImg)
        {
            var stringData = new Dictionary<string, string>();
            var binaryData = new Dictionary<String, Byte[]>();
            stringData.Add(Config.GetString("PlateNumber"), plateNumber);
            stringData.Add(Config.GetString("InTime"), inTime.ToString("yyyy-MM-dd HH:mm:ss"));
            binaryData.Add(Config.GetString("InImage"), inImg);
            var responseJson = this.Send(this.Config.GetString("ParkUrl"), stringData, binaryData);
            var parkingRes = IPSCMJsonConvert.Parse<ParkingResult>(responseJson);
            return parkingRes;
        }

        public LeavingResult Leaving(UInt64 recordId, String plateNumber, DateTime outTime, Byte[] outImg,
            UInt32 copeMoney,
            UInt32 actualMoney, UInt64 ticketId)
        {
            var stringData = new Dictionary<string, string>();
            var binaryData = new Dictionary<String, Byte[]>();

            stringData.Add(Config.GetString("RecordId"), recordId.ToString());
            stringData.Add(Config.GetString("PlateNumber"), plateNumber);
            stringData.Add(Config.GetString("OutTime"), outTime.ToString("yyyy-MM-dd HH:mm:ss"));
            binaryData.Add(Config.GetString("OutImage"), outImg);
            stringData.Add(Config.GetString("CopeMoney"), copeMoney.ToString());
            stringData.Add(Config.GetString("ActualMoney"), actualMoney.ToString());
            stringData.Add(Config.GetString("TicketId"), ticketId.ToString());
            var responseJson = this.Send(this.Config.GetString("LeaveUrl"), stringData, binaryData);
            var leavingResult = IPSCMJsonConvert.Parse<LeavingResult>(responseJson);
            return leavingResult;
        }

        private HeartBeatResult HeartBeat()
        {
            var response = this.Send
                (
                    this.Config.GetString("HEARTBEATURL"),
                    new Dictionary<string, string>(),
                    new Dictionary<string, byte[]>()
                );
            var heartBeatResult = IPSCMJsonConvert.Parse<HeartBeatResult>(response);

            var trigger = this.OnHeartBeat;
            if (trigger != null) trigger(this, new HeartBeatEventArgs(heartBeatResult));
            return heartBeatResult;
        }

        private String GetSign(Dictionary<String, String> data)
        {
            var sum = new StringBuilder();
            var sort = from key in data.Keys orderby key select key;
            foreach (var key in sort)
            {
                sum.Append(data[key]);
            }
            sum.Append(this.SecurityKey);
            var sign = HashUtils.CalculateMD5Hash(sum.ToString()).ToLower();
            return sign;
        }

        private void Tick()
        {
            Log.Info("Cloud parking start heart beat");
            while (true)
            {
                try
                {
                    Thread.Sleep(this.Config.GetInt("HeartBeatInterval"));
                    this.HeartBeat();
                }
                catch (ThreadInterruptedException)
                {
                    Log.Info("Cloud parking tick exit");
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error("Cloud parking tick encountered unexpected error", ex);
                }
            }
        }
    }
}