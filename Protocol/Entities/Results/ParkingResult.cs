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

                        this.Money = o[this.FieldConfig.GetString("Info")][this.FieldConfig.GetString("Money")].ToObject<UInt32>();
                        this.UserID = o[this.FieldConfig.GetString("Info")][this.FieldConfig.GetString("UserID")].ToObject<String>();
                        this.PhoneNumber = o[this.FieldConfig.GetString("Info")][this.FieldConfig.GetString("PhoneNumber")].ToObject<String>();
                        break;
                    }
                case ResultCode.SuccessButNoBinding:
                    {

                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(String.Format("Parking operation not support resultcode{0}", this.ResultCode));
                    }
            }
        }
    }
}
