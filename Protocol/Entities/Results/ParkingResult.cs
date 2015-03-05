using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace IPSCM.Protocol.Entities.Results
{
    public class ParkingResult : Result
    {
        public UInt32 Money { get; private set; }
        public String UserID { get; private set; }
        public String PhoneNumber { get; private set; }

        public ParkingResult(String jsonString)
            : base(jsonString)
        {
            var o = JObject.Parse(jsonString);
            switch (this.ResultCode)
            {
                case ResultCode.Success:
                    {

                        this.Money = (o.SelectToken("MoneyPath", false) ?? 0).ToObject<UInt32>();
                        this.UserID = (o.SelectToken("UserIDPath", false) ?? String.Empty).ToObject<String>();
                        this.PhoneNumber = (o.SelectToken("PhoneNumberPath", false) ?? String.Empty).ToObject<String>();
                        break;
                    }
                case ResultCode.SuccessButNoBinding:
                    {

                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(String.Format("Parking operation not support resultcode:{0}", this.ResultCode));
                    }
            }
        }
    }
}
