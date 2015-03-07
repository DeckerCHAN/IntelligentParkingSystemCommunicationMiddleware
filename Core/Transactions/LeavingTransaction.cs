using System;
using System.IO;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Entities.Results;
using IPSCM.Entities.Results.Leaving;
using IPSCM.Logging;
using Newtonsoft.Json.Linq;

namespace IPSCM.Core.Transactions
{
    public class LeavingTransaction : Transaction
    {
        public Thread WorkThread { get; private set; }
        public Stream ResponseStream { get; private set; }
        public UInt64 RecordId { get; private set; }
        public String PlateNumber { get; private set; }
        public DateTime OutTime { get; private set; }
        public Byte[] OutImg { get; private set; }
        public UInt32 CopeMoney { get; private set; }
        public UInt32 ActualMoney { get; private set; }
        public UInt64 TicketId { get; private set; }
        private Config JsonConfig { get; set; }

        public LeavingTransaction(String plateNumber, DateTime outTime, Byte[] outImg, UInt32 copeMoney, UInt32 actualMoney, UInt64 ticketId, Stream responseStream)
        {
            //TODO:Using record id which readed from db
            this.RecordId = 0x00;
            this.PlateNumber = plateNumber;
            this.OutTime = outTime;
            this.OutImg = outImg;
            this.CopeMoney = copeMoney;
            this.ActualMoney = actualMoney;
            this.TicketId = ticketId;
            this.ResponseStream = responseStream;
            this.JsonConfig = FileConfig.FindConfig("Json.cfg");
            this.WorkThread = new Thread(() =>
            {
                try
                {
                    var result = Engine.GetEngine()
                        .CloudParking.Leaving(this.RecordId, this.PlateNumber, this.OutTime, this.OutImg, this.CopeMoney, this.ActualMoney, this.TicketId);
                    switch (result.ResultCode)
                    {
                        case ResultCode.Success:
                            {
                                this.successful();
                                break;
                            }
                        default:
                            {
                                this.failure(result);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    var o = new JObject
                    {
                        new JProperty(this.JsonConfig.GetString("ResultCode"), ResultCode.ServerFailure),
                        new JProperty(this.JsonConfig.GetString("ErrorMessagePath"), ex.Message)
                    };
                    new StreamWriter(this.ResponseStream).Write(o.ToString());
                }
                finally
                {
                    this.ResponseStream.Close();
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

        private void successful()
        {
            JObject o = new JObject { new JProperty(this.JsonConfig.GetString("ResultCode"), ResultCode.Success) };
            new StreamWriter(this.ResponseStream).Write(o.ToString());

        }

        private void failure(LeavingResult result)
        {
            JObject o = new JObject { new JProperty(this.JsonConfig.GetString("ResultCode"), ResultCode.Success) };
            if (!String.IsNullOrEmpty(result.ErrorMessage))
            {
                o.Add(new JProperty(JsonConfig.GetString("ErrorMessage"), result.ErrorMessage));
            }
            Log.Error(String.Format("Unexpected result code:{0}", result.ResultCode));
        }
    }
}
