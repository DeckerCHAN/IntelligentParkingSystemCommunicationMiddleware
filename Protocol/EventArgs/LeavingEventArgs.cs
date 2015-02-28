using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPSCM.Protocol.EventArgs
{
    public class LeavingEventArgs : HttpDataEventArgs
    {
        public LeavingEventArgs(HttpListenerRequest request, HttpListenerResponse response) 
            : base(request, response)
        {

        }
    }
}
