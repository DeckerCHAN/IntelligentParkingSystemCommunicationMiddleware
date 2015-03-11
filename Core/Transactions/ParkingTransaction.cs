#region

using System;
using System.IO;
using System.Text;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Entities.Results;
using IPSCM.Entities.Results.Parking;
using IPSCM.Logging;
using IPSCM.Utils;
using Newtonsoft.Json.Linq;

#endregion

namespace IPSCM.Core.Transactions
{
    public class ParkingTransaction : Transaction
    {
        public ParkingTransaction(String plateNum, DateTime inTime, Byte[] inImage, Stream responseStream)
        {
            this.JsonConfig = FileConfig.FindConfig("Json.cfg");
            this.plateNumber = plateNum;
            this.InTime = inTime;
            this.InImage = inImage;
            this.ResponseStream = responseStream;
            this.WorkThread = new Thread(i =>
            {
                try
                {
                    var json = Entities.IPSCMJsonConvert.ConvertToJson(new Result() { ResultCode = ResultCode.Success });
                    StreamUtils.WriteToStreamWithUF8(this.ResponseStream, json);
                    this.ResponseStream.Flush();
                    this.ResponseStream.Close();
                    var Id = Engine.GetEngine().Storage.PreCarParked(this.plateNumber, this.InTime);
                    var result = Engine.GetEngine().CloudParking.Parking(plateNum, inTime, inImage);


                    switch (result.ResultCode)
                    {
                        case ResultCode.Success:
                            {
                                Engine.GetEngine().Storage.PostCarParked(Id, this.plateNumber, result);
                                break;
                            }
                        case ResultCode.SuccessButNoBinding:
                            {
                                break;
                            }
                        default:
                            {
                                Log.Error(String.Format("Unexpected result code:{0}", result.ResultCode));
                                break;
                            }
                    }
                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    Log.Error("Parking Transaction encountered a exception", ex);
                }
            });
        }

        public String plateNumber { get; private set; }
        public DateTime InTime { get; private set; }
        public Byte[] InImage { get; private set; }
        public Stream ResponseStream { get; private set; }
        public Thread WorkThread { get; private set; }
        public Config JsonConfig { get; private set; }

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