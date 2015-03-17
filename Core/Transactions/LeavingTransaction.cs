#region

using System;
using System.IO;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Entities;
using IPSCM.Entities.Results;
using IPSCM.Logging;
using IPSCM.Utils;

#endregion

namespace IPSCM.Core.Transactions
{
    public class LeavingTransaction : Transaction
    {
        public LeavingTransaction(String plateNumber, DateTime outTime, Byte[] outImg, Decimal copeMoney,
            Decimal actualMoney, UInt32 ticketId, Stream responseStream)
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
            this.FullOutput = FileConfig.FindConfig("Transaction.cfg").GetBoolean("FullOutput");

            this.WorkThread = new Thread(() =>
            {
                try
                {
                    DateTime start = DateTime.Now;
                    if (this.FullOutput)
                    {
                        Log.Info(String.Format("Leave started[+0ms]"));
                    }
                    //Storage messages
                    this.RecordId = Engine.GetEngine().Storage.PreCarLeave(this.PlateNumber, this.OutTime);
                    if (this.FullOutput)
                    {
                        Log.Info(String.Format("Leave pre-leaved[+{0}ms]", (DateTime.Now - start).TotalMilliseconds));
                    }
                    //Response F3!
                    var json = String.Empty;
                    if (this.RecordId == 0)
                    {
                        json =
                            IPSCMJsonConvert.ConvertToJson(new Result
                         {
                             ResultCode = ResultCode.SuccessButInsufficientFunds
                         });
                    }
                    else if (Engine.GetEngine().Storage.TryDeductBalance(this.PlateNumber, this.ActualMoney))
                    {
                        if (this.FullOutput)
                        {
                            Log.Info(String.Format("Leave Educted Balance[+{0}ms]",
                                (DateTime.Now - start).TotalMilliseconds));
                        }
                        json =
                            IPSCMJsonConvert.ConvertToJson(new Result
                            {
                                ResultCode = ResultCode.Success
                            });
                    }
                    else
                    {
                        json =
                            IPSCMJsonConvert.ConvertToJson(new Result
                            {
                                ResultCode = ResultCode.SuccessButInsufficientFunds
                            });
                    }
                    StreamUtils.WriteToStreamWithUF8(this.ResponseStream, json);
                    this.ResponseStream.Flush();
                    this.ResponseStream.Close();
                    if (this.FullOutput)
                    {
                        Log.Info(String.Format("Leave responded to F3[+{0}ms]", (DateTime.Now - start).TotalMilliseconds));
                    }
                    //Used ticket
                    if (TicketId != 0)
                    {
                        Engine.GetEngine().Storage.UsedTicket(ticketId, outTime);
                    }
                    if (this.FullOutput)
                    {
                        Log.Info(String.Format("Leave ticket processed[+{0}ms]",
                            (DateTime.Now - start).TotalMilliseconds));
                    }

                    //Send message to cloud
                    // Engine.GetEngine().Storage.PreCarLeave(this.PlateNumber, OutTime);
                    var result = Engine.GetEngine()
                        .CloudParking.Leaving(this.RecordId, this.PlateNumber, this.OutTime, this.OutImg, this.CopeMoney,
                            this.ActualMoney, this.TicketId);
                    if (this.FullOutput)
                    {
                        Log.Info(String.Format("Leave result received[+{0}ms]", (DateTime.Now - start).TotalMilliseconds));
                    }

                    switch (result.ResultCode)
                    {
                        case ResultCode.Success:
                            {
                                Engine.GetEngine().Storage.PostCarLeaved(this.PlateNumber, result);
                                break;
                            }
                        default:
                            {
                                Log.Error("Leaving transaction do not support Result code：" + result.ResultCode+" and wrong message is:"+result.ErrorMsg);
                                break;
                            }
                    }
                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    this.Status = TransactionStatus.Errored;
                    Log.Error("Leaving transtraction encountered a bad error!", ex);
                }
                finally
                {
                    this.ResponseStream.Flush();
                    this.ResponseStream.Close();
                }
            });
        }

        public Thread WorkThread { get; private set; }
        public Stream ResponseStream { get; private set; }
        public UInt64 RecordId { get; private set; }
        public String PlateNumber { get; private set; }
        public DateTime OutTime { get; private set; }
        public Byte[] OutImg { get; private set; }
        public Decimal CopeMoney { get; private set; }
        public Decimal ActualMoney { get; private set; }
        public UInt64 TicketId { get; private set; }
        private Config JsonConfig { get; set; }
        private Boolean FullOutput { get; set; }

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