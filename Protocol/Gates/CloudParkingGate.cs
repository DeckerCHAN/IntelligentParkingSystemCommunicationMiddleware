using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IPSCM.Configuration;
using IPSCM.Logging;
using IPSCM.Protocol.Entities;
using IPSCM.Protocol.Entities.Results;
using IPSCM.Protocol.EventArgs;
using IPSCM.Utils;

namespace IPSCM.Protocol.Gates
{
    public delegate void HeartBeatEventHandler(object sender, HeartBeatEventArgs arg);

    public delegate void LoginEventHandler(object sender, LoginEvenArgs arg);
    public class CloudParkingGate : ControllableObject, ISend
    {
        public event HeartBeatEventHandler OnHeartBeat;
        public event LoginEventHandler OnLoggedin;
        public Thread TickThread { get; private set; }
        public String Token { get; private set; }
        public Config Config { get; private set; }
        private HttpClient Client { get; set; }
        private String SecurityKey { get; set; }

        public CloudParkingGate()
        {
            this.Config = new FileConfig(new FileInfo("Parking.cfg"));
            this.SecurityKey = Config.GetString("SecurityKey");
            this.Client = new HttpClient();
            this.Token = String.Empty;
            this.TickThread = new Thread(this.Tick);
        }
        public override void Start()
        {
            Log.Info("CloudParking Starting...");
            base.Start();
            this.TickThread.Start();
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
            Dictionary<String, String> data = new Dictionary<string, string>();
            data.Add("username", userName);
            data.Add("password", password);
            var responseJson = this.Send(this.Config.GetString("LoginUrl"), data, new Dictionary<string, byte[]>());
            //TODO:Using real data
            var loginRes = new LoginResult(responseJson);
            this.Token = loginRes.Token;

            var trigger = this.OnLoggedin;
            if (trigger != null) trigger(this, new LoginEvenArgs(loginRes));
            return loginRes;
        }

        public String Parking(String plateNumber, DateTime inTime, Byte[] inImg)
        {
            throw new NotImplementedException();
        }

        private String HeartBeat()
        {
            String data = String.Empty;
            var trigger = this.OnHeartBeat;
            if (trigger != null) trigger(this, new HeartBeatEventArgs(data));
            throw new NotImplementedException();
        }

        public string Send(string url, Dictionary<string, string> textData, Dictionary<string, byte[]> rowData)
        {
            var sign = this.GetSign(textData);
            textData.Add("sign", sign);
            var requestContent = new MultipartFormDataContent();
            requestContent.Headers.Add("sign", sign);
            foreach (var key in textData.Keys)
            {
                requestContent.Add(new StringContent(textData[key]), key);
            }
            foreach (var key in rowData.Keys)
            {
                requestContent.Add(new ByteArrayContent(rowData[key]), key);
            }



            var response = this.Client.PostAsync(url, requestContent).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            return result;
        }

        private String GetSign(Dictionary<String, String> datas)
        {
            var sum = new StringBuilder();
            var sort = from key in datas.Keys orderby key select key;
            foreach (var key in sort)
            {
                sum.Append(datas[key]);
            }
            sum.Append(this.SecurityKey);
            var sign = HashUtils.CalculateMD5Hash(sum.ToString());
            return sign;

        }

        private void Tick()
        {
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
