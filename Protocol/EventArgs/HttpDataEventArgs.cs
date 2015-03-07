#region

using System.Net;

#endregion

namespace IPSCM.Protocol.EventArgs
{
    public class HttpDataEventArgs : System.EventArgs
    {
        public HttpDataEventArgs(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.Request = request;
            this.Response = response;
        }

        public HttpListenerRequest Request { get; private set; }
        public HttpListenerResponse Response { get; private set; }
    }
}