#region

using IPSCM.Entities.Results;

#endregion

namespace IPSCM.Protocol.EventArgs
{
    public class HeartBeatEventArgs : System.EventArgs
    {
        public HeartBeatEventArgs(HeartBeatResult result)
        {
            this.Result = result;
        }

        public HeartBeatResult Result { get; private set; }
    }
}