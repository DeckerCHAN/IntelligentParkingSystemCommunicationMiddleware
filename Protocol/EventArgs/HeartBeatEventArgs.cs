using System;

namespace IPSCM.Protocol.EventArgs
{
    public class HeartBeatEventArgs : System.EventArgs
    {
        public String Data { get; private set; }

        public HeartBeatEventArgs(String data)
        {

        }
    }
}
