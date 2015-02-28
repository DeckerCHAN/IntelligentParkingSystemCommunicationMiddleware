using System;
using System.IO;
using System.Threading;
using IPSCM.Configuration;
using IPSCM.Logging;
using IPSCM.Protocol.Entities;
using IPSCM.Protocol.Entities.Results;
using Newtonsoft.Json.Linq;

namespace IPSCM.Core.Transactions
{
    public class ParkingTransaction : Transaction
    {
        public String plateNumber { get; private set; }
        public DateTime InTime { get; private set; }
        public Byte[] InImage { get; private set; }
        public Stream ResponseStream { get; private set; }
        public Thread WorkThread { get; private set; }
        public Config JsonConfig { get; private set; }

        public ParkingTransaction(String plateNum, DateTime inTime, Byte[] inImage, Stream responseStream)
        {
            this.plateNumber = plateNum;
            this.InTime = inTime;
            this.InImage = inImage;
            this.ResponseStream = responseStream;
            this.WorkThread = new Thread(i =>
            {
                try
                {
                    ParkingResult result;
                    while (true)
                    {
                        try
                        {
                            result = Engine.GetEngine().CloudParking.Parking(plateNum, inTime, inImage);
                            break;

                        }
                        catch (Exception ex)
                        {
                            //TODO:Catch network exception to re-send
                            throw;
                        }

                    }

                    switch (result.ResultCode)
                    {
                        case ResultCode.Success:
                            {
                                JObject o = new JObject();
                                o.Add(new JProperty(JsonConfig.GetString("ResultCode"), ResultCode.Success));
                                new StreamWriter(this.ResponseStream).Write(o.ToString());
                                break;
                            }
                        case ResultCode.SuccessButNoBinding:
                            {
                                JObject o = new JObject();
                                o.Add(new JProperty(JsonConfig.GetString("ResultCode"), ResultCode.SuccessButNoBinding));
                                new StreamWriter(this.ResponseStream).Write(o.ToString());
                                break;
                            }
                        default:
                            {
                                JObject o = new JObject();
                                o.Add(new JProperty(JsonConfig.GetString("ResultCode"), ResultCode.Success));
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
    }
}
