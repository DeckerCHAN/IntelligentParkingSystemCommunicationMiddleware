#region

using System;
using System.IO;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Entities.Results;
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
                    var result = Engine.GetEngine().CloudParking.Parking(plateNum, inTime, inImage);


                    switch (result.ResultCode)
                    {
                        case ResultCode.Success:
                        {
                            this.ParkingSuccess();
                            break;
                        }
                        case ResultCode.SuccessButNoBinding:
                        {
                            this.ParkingSuccessButNoBinding();
                            break;
                        }
                        default:
                        {
                            JObject o = new JObject
                            {
                                new JProperty(this.JsonConfig.GetString("ResultCode"), ResultCode.Success)
                            };
                            if (!String.IsNullOrEmpty(result.ErrorMessage))
                            {
                                o.Add(new JProperty(JsonConfig.GetString("ErrorMessage"), result.ErrorMessage));
                            }
                            Log.Error(String.Format("Unexpected result code:{0}", result.ResultCode));
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Parking Transaction encountered a exception", ex);
                    var o = new JObject
                    {
                        new JProperty(this.JsonConfig.GetString("ResultCode"), ResultCode.ServerFailure)
                    };
                    // this.ResponseStream.Write(Encoding.UTF8.GetBytes(o.ToString()), 0, Encoding.UTF8.GetBytes(o.ToString()).Length);
                    new StreamWriter(this.ResponseStream).Write(o.ToString());
                }
                finally
                {
                    this.ResponseStream.Flush();
                    this.ResponseStream.Close();
                }
            });
        }

        public String plateNumber { get; private set; }
        public DateTime InTime { get; private set; }
        public Byte[] InImage { get; private set; }
        public Stream ResponseStream { get; private set; }
        public Thread WorkThread { get; private set; }
        public Config JsonConfig { get; private set; }

        private void ParkingSuccess()
        {
            JObject o = new JObject {new JProperty(this.JsonConfig.GetString("ResultCode"), ResultCode.Success)};
            StreamUtils.WriteToStreamWithUF8(this.ResponseStream, o.ToString());
        }

        private void ParkingSuccessButNoBinding()
        {
            var o = new JObject {new JProperty(this.JsonConfig.GetString("ResultCode"), ResultCode.SuccessButNoBinding)};
            new StreamWriter(this.ResponseStream).Write(o.ToString());
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