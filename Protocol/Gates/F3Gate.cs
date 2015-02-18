using System;
using System.IO;
using System.Net;
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

        private readonly HttpListener Listener;
        private Thread ListenThread;
        private FileConfig Config;
        private UInt32 PortNumber;
        private String LocalHost;
        public F3Gate()
        {
            if (!HttpListener.IsSupported)
            {
                Log.Error("Windows XP SP2 or Server 2003 or upper is required to use the HttpListener class.");
                return;
            }
            this.Config = new FileConfig(new FileInfo("F3.cfg"));
            this.PortNumber = this.Config.GetUInt("Port");
            this.LocalHost = this.Config.GetString("LocalHost");
            this.Listener = new HttpListener();
            this.Listener.Prefixes.Add(String.Format("http://{0}:{1}/", LocalHost, this.PortNumber));
            this.OnReceived += F3Gate_OnReceived;

        }

        void F3Gate_OnReceived(object sender, HttpDataEventArgs arg)
        {
            Log.Info(arg.Request.HttpMethod);
            Log.Info(arg.Request.Url.ToString());
            Log.Info(arg.Request.RawUrl);
            if (!arg.Request.HasEntityBody)
            {
                return;
            }
            using (Stream body = arg.Request.InputStream)
            {
                using (StreamReader reader = new StreamReader(body, arg.Request.ContentEncoding))
                {
#if DEBUG
                    Log.Info(reader.ReadToEnd());
#endif
                }
            }
            arg.Response.OutputStream.Close();
        }
        public void Start()
        {
            base.Start();
            this.ListenThread = new Thread(() =>
            {

                this.Listener.Start();
                Log.Info(String.Format("F3 Binded on:{0}:{1}", this.LocalHost, this.PortNumber));
                while (this.Listener.IsListening)
                {
                    try
                    {
                        var context = this.Listener.GetContext();
                        Log.Info("F3 request received");
                        OnReceived(this, new HttpDataEventArgs(context.Request, context.Response));

                    }
                    catch (ThreadInterruptedException ex)
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
        }

        public void Stop()
        {
            base.Stop();
            this.Listener.Stop();
            this.ListenThread.Interrupt();
        }
    }
}
