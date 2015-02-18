using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
