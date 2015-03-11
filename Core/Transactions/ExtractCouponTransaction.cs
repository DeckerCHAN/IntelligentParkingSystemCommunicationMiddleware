using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using IPSCM.Entities;
using IPSCM.Entities.Results;
using IPSCM.Entities.Results.Coupon;
using IPSCM.Utils;

namespace IPSCM.Core.Transactions
{
    class ExtractCouponTransaction : Transaction
    {
        public Thread ProcessThread { get; private set; }
        public String PlateNumber { get; private set; }
        public ExtractCouponTransaction(String plateNumber, Stream responseStream)
        {
            this.PlateNumber = plateNumber;
            this.ProcessThread = new Thread(() =>
            {
                try
                {
                    var result = new CouponResult();
                    var ticket = Engine.GetEngine().Storage.GetTicketByPlateNumber(plateNumber);
                    if (ticket == null)
                    {
                        StreamUtils.WriteToStreamWithUF8(responseStream, IPSCMJsonConvert.ConvertToJson(new Result() { ResultCode = ResultCode.SuccessButNoBinding }));

                    }
                    else
                    {
                        result.Info = new CouponInfo();
                        result.Info.TicketId = ticket.TicketId;
                        result.Info.Type = ticket.Type;
                        result.Info.Value = ticket.Value;
                        StreamUtils.WriteToStreamWithUF8(responseStream, IPSCMJsonConvert.ConvertToJson(result));

                    }
                    responseStream.Close();
                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    this.Status = TransactionStatus.Errored;
                    Logging.Log.Error("Extract Coupon Transaction encountered a bad error!", ex);
                }


            });
        }
        public override void Execute()
        {
            this.ProcessThread.Start();
            base.Execute();
        }

        public override void Interrupt()
        {
            this.ProcessThread.Interrupt();
            base.Interrupt();
        }
    }
}
