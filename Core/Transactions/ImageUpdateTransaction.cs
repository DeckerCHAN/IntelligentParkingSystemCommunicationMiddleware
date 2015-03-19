using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using IPSCM.Entities;
using IPSCM.Entities.Results;
using IPSCM.Utils;

namespace IPSCM.Core.Transactions
{
    public class ImageUpdateTransaction : Transaction
    {
        public Thread ProcessThread { get; private set; }

        public ImageUpdateTransaction(String plateNumber, String time, String type, Byte[] image, Stream responseStream)
        {
            this.ProcessThread = new Thread(() =>
            {
                try
                {
                    StreamUtils.WriteToStreamWithUF8(responseStream, IPSCMJsonConvert.ConvertToJson(new Result() { ResultCode = ResultCode.Success }));
                    responseStream.Flush();
                    responseStream.Close();

                    Engine.GetEngine().CloudParking.ImageUpload(plateNumber, time, type, image);
                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    Logging.Log.Error("Image upload encountered a bad error!", ex);
                    this.Status = TransactionStatus.Errored;
                }
                finally
                {
                    responseStream.Flush();
                    responseStream.Close();
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
