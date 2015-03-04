using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Logging;
using IPSCM.Protocol.EventArgs;

namespace IPSCM.Protocol.Gates
{
    public delegate void ParkingEventHandler(object sender, ParkingEventArgs arg);
    public delegate void CouponEventHandler(object sender, CouponEventArgs arg);
    public delegate void LeavingEventhandler(object sender, LeavingEventArgs arg);

    public class F3Gate : ControllableObject, IReceive
    {
        public event ReceiveEventHandler OnReceived;
        public event ParkingEventHandler OnParking;
        public event LeavingEventhandler OnLeaving;

        private readonly HttpListener Listener;
        private Thread ListenThread;
        private FileConfig Config { get; set; }
        private UInt32 PortNumber { get; set; }
        private String LocalHost { get; set; }

        private String ParkingUrl { get; set; }
        private String LeavingUrl { get; set; }
        private String CouponReceiveUrl { get; set; }

        public Boolean IsDebug { get; private set; }
        public F3Gate()
        {
            if (!HttpListener.IsSupported)
            {
                Log.Error("Windows XP SP2 or Server 2003 or upper is required to use the HttpListener class.");
                return;
            }
            this.Config = FileConfig.FindConfig("F3.cfg");
            this.IsDebug = Config.GetBoolean("IsDebug");
            this.PortNumber = this.Config.GetUInt("Port");
            this.LocalHost = this.Config.GetString("LocalHost");
            this.ParkingUrl = this.Config.GetString("ParkingUrl");
            this.LeavingUrl = this.Config.GetString("LeavingUrl");
            this.CouponReceiveUrl = Config.GetString("CouponReceiveUrl");
            this.Listener = new HttpListener();
            this.Listener.Prefixes.Add(String.Format("http://{0}:{1}/", this.LocalHost, this.PortNumber));
            this.OnReceived += this.F3Gate_OnReceived;

        }

        void F3Gate_OnReceived(object sender, HttpDataEventArgs arg)
        {

            Log.Info(String.Format("F3 received a request form {0} through {1} method with url: {2}", arg.Request.RemoteEndPoint, arg.Request.HttpMethod, arg.Request.RawUrl));
            if (!arg.Request.HasEntityBody) { return; }
            var stringContent = new Dictionary<string, string>();
            var binaryContent = new Dictionary<string, byte[]>();
            using (var body = arg.Request.InputStream)
            {
                using (new StreamReader(body, arg.Request.ContentEncoding))
                {
                    var streamContent = new StreamContent(body);
                    streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(arg.Request.ContentType);

                    var provider = streamContent.ReadAsMultipartAsync().Result;

                    foreach (var httpContent in provider.Contents)
                    {
                        var name = httpContent.Headers.ContentDisposition.Name.Replace("\"", String.Empty);
                        var fileName = httpContent.Headers.ContentDisposition.FileName;
                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            var value = httpContent.ReadAsStringAsync().Result;
                            stringContent.Add(name, value);
                            continue;
                        }
                        var bytesValue = httpContent.ReadAsByteArrayAsync().Result;
                        binaryContent.Add(name, bytesValue);
                    }

                }
            }
            var url = arg.Request.RawUrl;
            try
            {
                if (url.Equals(this.ParkingUrl))
                {
                    var trigger = this.OnParking;
                    if (trigger != null) trigger(this,
                        new ParkingEventArgs(
                            arg.Request,
                            arg.Response,
                           stringContent[this.Config.GetString("PlateNumber")],
                            DateTime.Parse(stringContent[this.Config.GetString("InTime")]),
                            binaryContent[this.Config.GetString("InImage")])
                            );
                }
                else if (url.Equals(this.LeavingUrl))
                {
                    var trigger = this.OnLeaving;
                    if (trigger != null) trigger(this, new LeavingEventArgs(
                            arg.Request,
                            arg.Response,
                            stringContent[this.Config.GetString("PlateNumber")],
                                DateTime.Parse(stringContent[this.Config.GetString("OutTime")]),
                                 binaryContent[this.Config.GetString("OutImage")],
                                 UInt32.Parse(stringContent[this.Config.GetString("CopeMoney")]),
                                 UInt32.Parse(stringContent[this.Config.GetString("ActualMoney")]),
                                 UInt64.Parse(stringContent[this.Config.GetString("TicketMoney")])
                            ));
                }
                else if (url.Equals(this.CouponReceiveUrl))
                {

                }
                else
                {
                    arg.Response.OutputStream.Close();

                    throw new ArgumentException(String.Format("Can not find url {0}", arg.Request.Url));
                }
            }
            catch (Exception ex)
            {
                arg.Response.OutputStream.Close();
                throw new ArgumentException("Can not process this request!", ex);

            }

        }
        public override void Start()
        {
            base.Start();
            Log.Info("F3 starting...");
            this.ListenThread = new Thread(() =>
            {

                this.Listener.Start();
                Log.Info(String.Format("F3 Binded on:{0}:{1}", this.LocalHost, this.PortNumber));
                while (this.Listener.IsListening)
                {
                    try
                    {
                        var context = this.Listener.GetContext();
                        if (context.Request.HttpMethod == "GET") { continue; }
                        var trigger = this.OnReceived;
                        if (trigger != null) trigger(this, new HttpDataEventArgs(context.Request, context.Response));

                    }
                    catch (ThreadInterruptedException)
                    {
                        Log.Info("Listener stopped");
                        return;

                    }
                    catch (HttpListenerException ex)
                    {
                        if (ex.ErrorCode == 0x80004005)
                        {
                            Log.Info("Listener stopped");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Encounter in listen thread", ex);
                    }
                }
                this.Listener.Stop();



            });
            this.ListenThread.Start();
            Log.Info("F3 started");
        }

        public override void Stop()
        {
            base.Stop();
            Log.Info("F3 stoping...");
            this.Listener.Stop();
            this.ListenThread.Interrupt();
            Log.Info("F3 stopped");
        }
    }
}
