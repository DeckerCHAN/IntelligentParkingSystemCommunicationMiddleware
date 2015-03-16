#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Logging;
using IPSCM.Protocol.EventArgs;

#endregion

namespace IPSCM.Protocol.Gates
{
    public delegate void ParkingEventHandler(object sender, ParkingEventArgs arg);

    public delegate void CouponEventHandler(object sender, CouponEventArgs arg);

    public delegate void LeavingEventhandler(object sender, LeavingEventArgs arg);

    public delegate void UpdateSurplusSpaceEventhandler(object sender, UpdateSurplusSpaceEventArgs arg);

    public class F3Gate : ControllableObject, IReceive
    {
        private readonly HttpListener Listener;
        private Thread ListenThread;

        public F3Gate()
        {
            if (!HttpListener.IsSupported)
            {
                Log.Error("Windows XP SP2 or Server 2003 or upper is required to use the HttpListener class.");
                return;
            }
            this.Config = FileConfig.FindConfig("F3.cfg");
            this.IsDebug = this.Config.GetBoolean("IsDebug");
            this.PortNumber = this.Config.GetUInt("Port");
            this.LocalHost = this.Config.GetString("LocalHost");
            this.ParkingUrl = this.Config.GetString("ParkingUrl");
            this.LeavingUrl = this.Config.GetString("LeavingUrl");
            this.CouponReceiveUrl = Config.GetString("CouponReceiveUrl");
            this.UpdateUrl = Config.GetString("UpdateUrl");
            this.RegisterHttp(this.LocalHost, this.PortNumber);
            this.Listener = new HttpListener();
            this.Listener.Prefixes.Add(String.Format("http://{0}:{1}/", this.LocalHost, this.PortNumber));
            this.OnReceived += this.F3Gate_OnReceived;
        }

        private FileConfig Config { get; set; }
        private UInt32 PortNumber { get; set; }
        private String LocalHost { get; set; }
        private String ParkingUrl { get; set; }
        private String LeavingUrl { get; set; }
        private String CouponReceiveUrl { get; set; }
        private String UpdateUrl { get; set; }
        public Boolean IsDebug { get; private set; }
        public event ReceiveEventHandler OnReceived;
        public event ParkingEventHandler OnParking;
        public event LeavingEventhandler OnLeaving;
        public event CouponEventHandler OnCouponNeed;
        public event UpdateSurplusSpaceEventhandler OnSurplusSpaceUpdate;

        private void F3Gate_OnReceived(object sender, HttpDataEventArgs arg)
        {
            Log.Info(String.Format("F3 received a request form {0} through {1} method with url: {2}",
                arg.Request.RemoteEndPoint, arg.Request.HttpMethod, arg.Request.RawUrl));
            if (!arg.Request.HasEntityBody)
            {
                return;
            }
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
            //If debug output all message
            if (this.IsDebug)
            {
                foreach (var key in stringContent.Keys)
                {
                    Log.Info(String.Format("Key:{0} Value:{1}",key,stringContent[key]));
                }
                foreach (var key in binaryContent.Keys)
                {
                    Log.Info(String.Format("Key:{0} BinaryLength:{1}",key,binaryContent[key].Length));
                }
            }
            //Switch urls
            var url = arg.Request.RawUrl;
            try
            {
                if (url.Equals(this.ParkingUrl))
                {
                    var trigger = this.OnParking;
                    if (trigger != null)
                        trigger(this,
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
                    if (trigger != null)
                        trigger(this, new LeavingEventArgs(
                            arg.Request,
                            arg.Response,
                            stringContent[this.Config.GetString("PlateNumber")],
                            DateTime.Parse(stringContent[this.Config.GetString("OutTime")]),
                            binaryContent[this.Config.GetString("OutImage")],
                            Decimal.Parse(stringContent[this.Config.GetString("CopeMoney")]),
                            Decimal.Parse(stringContent[this.Config.GetString("ActualMoney")]),
                            UInt32.Parse(stringContent[this.Config.GetString("TicketId")])
                            ));
                }
                else if (url.Equals(this.CouponReceiveUrl))
                {
                    var trigger = this.OnCouponNeed;
                    if (trigger != null)
                        trigger(this,
                            new CouponEventArgs(arg.Request, arg.Response,
                                stringContent[this.Config.GetString("PlateNumber")]));
                }
                else if (url.Equals(this.UpdateUrl))
                {
                    var trigger = this.OnSurplusSpaceUpdate;
                    if (trigger != null)
                        trigger(this,
                            new UpdateSurplusSpaceEventArgs(arg.Request, arg.Response,
                                UInt16.Parse(stringContent[this.Config.GetString("SURPLUSSPACE")])));
                }
                else
                {
                    arg.Response.OutputStream.Close();

                    throw new ArgumentException(String.Format("Can not find url {0}", arg.Request.Url));
                }
            }
            catch (KeyNotFoundException)
            {
                arg.Response.OutputStream.Close();
                throw new ArgumentException("Request argument deficiency!");
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
                        if (context.Request.HttpMethod == "GET")
                        {
                            continue;
                        }
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
                        Log.Error("Encounter error in listen thread", ex);
                    }
                }
                this.Listener.Stop();
            });
            this.ListenThread.Start();
            Log.Info("F3 started");
        }

        public override void Stop()
        {
            if (this.RunningStatus != GateStatus.Started) return;
            Log.Info("F3 stoping...");
            this.Listener.Stop();
            this.ListenThread.Interrupt();
            Log.Info("F3 stopped");
        }

        private void RegisterHttp(String domain, UInt32 port)
        {
            string args = string.Format(@"http add urlacl url=http://{0}:{1}/ user={2}\{3}", domain, port,
                Environment.UserDomainName, Environment.UserName);
            Log.Info(String.Format(@"Trying register url:http://{0}:{1}/ as user user={2}\{3}", domain, port,
                Environment.UserDomainName, Environment.UserName));
            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            var process = Process.Start(psi);
            //var output = new StringBuilder();
            //while (!process.StandardOutput.EndOfStream)
            //{
            //    output.Append(process.StandardOutput.ReadLine());
            //}
            process.WaitForExit();
            //Log.Info(String.Format("Register result:{0}", output));
        }
    }
}