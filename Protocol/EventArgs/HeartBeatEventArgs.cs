#region

using IPSCM.Entities.Results;
using IPSCM.Entities.Results.HeartBeat;

#endregion

namespace IPSCM.Protocol.EventArgs
{
    public class HeartBeatEventArgs : System.EventArgs
    {
        public HeartBeatEventArgs(HeartBeatResult heartBeatResult)
        {
            this.HeartBeatResult = heartBeatResult;
        }

        public HeartBeatResult HeartBeatResult { get; private set; }
    }
}