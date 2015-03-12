#region

using System;
using System.Net;

#endregion

namespace IPSCM.Protocol.EventArgs
{
    public class UpdateSurplusSpaceEventArgs : HttpDataEventArgs
    {
        public UpdateSurplusSpaceEventArgs(HttpListenerRequest request, HttpListenerResponse response,
            UInt16 surplusSpace)
            : base(request, response)
        {
            this.SurplusSpace = surplusSpace;
        }

        public UInt16 SurplusSpace { get; private set; }
    }
}