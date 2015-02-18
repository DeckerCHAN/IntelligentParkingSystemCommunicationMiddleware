using System;
using Newtonsoft.Json.Linq;

namespace IPSCM.Protocol.Entities.Results
{
    public class LoginResult
    {
        public UInt16 ResultCode { get; private set; }
        public String Token { get; private set; }
        public String ErrorMessage { get; private set; }
        public LoginResult(UInt16 resultCode, String token, String errorMessage)
        {
            this.ResultCode = resultCode;
            this.Token = token;
            this.ErrorMessage = errorMessage;
        }

        public LoginResult(String gsonString)
        {
            var o = JObject.Parse(gsonString);
            this.ResultCode =o.SelectToken("result_code", true).ToObject<UInt16>();
            this.Token = o.SelectToken("info[0].token", true).ToString();
            this.ErrorMessage = o.SelectToken("error_msg", false).ToString();
        }
    }
}
