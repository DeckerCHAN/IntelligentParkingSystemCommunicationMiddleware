using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IPSCM.Entities.Results;

namespace IPSCM.Core.Transactions
{
    public class SurplusSpaceUpdateTransaction : Transaction
    {
        public Thread WorkThread { get; private set; }
        public UInt16 SurplusSpace { get; private set; }

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
                        throw new NotSupportedException("Surplus SpaceUpdate Transaction do not support result code:" + result.ResultCode);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log.Error("Surplus SpaceUpdate Transaction encountered a bad error!", ex);
                }
            });
        }
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
