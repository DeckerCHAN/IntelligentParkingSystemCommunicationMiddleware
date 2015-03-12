#region

using System;
using System.Threading;
using IPSCM.Entities.Results;
using IPSCM.Logging;

#endregion

namespace IPSCM.Core.Transactions
{
    public class SurplusSpaceUpdateTransaction : Transaction
    {
        public SurplusSpaceUpdateTransaction(UInt16 surplusSpace)
        {
            this.SurplusSpace = surplusSpace;
            this.WorkThread = new Thread(() =>
            {
                try
                {
                    var result = Engine.GetEngine().CloudParking.SurplusSpaceUpdate(surplusSpace);
                    if (result.ResultCode != ResultCode.Success)
                    {
                        throw new NotSupportedException("Surplus SpaceUpdate Transaction do not support result code:" +
                                                        result.ResultCode);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Surplus SpaceUpdate Transaction encountered a bad error!", ex);
                }
            });
        }

        public Thread WorkThread { get; private set; }
        public UInt16 SurplusSpace { get; private set; }

        public override void Execute()
        {
            this.WorkThread.Start();
            base.Execute();
        }

        public override void Interrupt()
        {
            this.WorkThread.Interrupt();
            base.Interrupt();
        }
    }
}