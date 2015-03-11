using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPSCM.Protocol.EventArgs
{
    public class UpdateSurplusSpaceEventArgs : HttpDataEventArgs
    {
        public UInt16 SurplusSpace { get; private set; }

        public UpdateSurplusSpaceEventArgs(HttpListenerRequest request, HttpListenerResponse response, UInt16 surplusSpace)
            : base(request, response)
        {
            this.SurplusSpace = surplusSpace;
        }
    }
}
