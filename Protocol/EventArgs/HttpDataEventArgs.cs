using System.Net;

namespace IPSCM.Protocol.EventArgs
{
    public class HttpDataEventArgs : System.EventArgs
    {
        public HttpListenerRequest Request { get; private set; }

        public HttpListenerResponse Response { get; private set; }

        public HttpDataEventArgs(HttpListenerRequest request, HttpListenerResponse response)
        {
            this.Request = request;
            this.Response = response;
        }
    }
}
