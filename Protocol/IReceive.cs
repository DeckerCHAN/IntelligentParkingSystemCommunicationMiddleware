using IPSCM.Protocol.EventArgs;

namespace IPSCM.Protocol
{
    public delegate void ReceiveEventHandler(object sender, HttpDataEventArgs arg);
 
    public interface IReceive
    {
        event ReceiveEventHandler OnReceived;
    }
}
