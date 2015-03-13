#region

using System;
using System.IO;
using System.Threading;
using IPSCM.Entities;
using IPSCM.Entities.Results;
using IPSCM.Logging;
using IPSCM.Utils;

#endregion

namespace IPSCM.Core.Transactions
{
    public class SurplusSpaceUpdateTransaction : Transaction
    {
        public SurplusSpaceUpdateTransaction(UInt16 surplusSpace,Stream responseStream)
        {
            this.SurplusSpace = surplusSpace;
            this.WorkThread = new Thread(() =>
            {
                try
                {
                    StreamUtils.WriteToStreamWithUF8(responseStream,
                        IPSCMJsonConvert.ConvertToJson(new Result { ResultCode = ResultCode.Success }));
                    responseStream.Flush();
                    responseStream.Close();
                    var result = Engine.GetEngine().CloudParking.SurplusSpaceUpdate(surplusSpace);
                    if (result.ResultCode != ResultCode.Success)
                    {
                        throw new NotSupportedException("Surplus SpaceUpdate Transaction do not support result code:" +
                                                        result.ResultCode);
                    }
                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    Log.Error("Surplus SpaceUpdate Transaction encountered a bad error!", ex);
                    this.Status = TransactionStatus.Errored;
                }
                finally
                {
                    responseStream.Flush();
                    responseStream.Close();
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