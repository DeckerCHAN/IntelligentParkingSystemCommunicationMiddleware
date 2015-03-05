using System;
using IPSCM.Configuration;
using Newtonsoft.Json.Linq;

namespace IPSCM.Protocol.Entities.Results
{
    public class LoginResult : Result
    {
        public String Token { get; private set; }

        public LoginResult(String jsonString)
            : base(jsonString)
        {
            var o = JObject.Parse(jsonString);
            this.Token = (o.SelectToken(this.JsonConfig.GetString("TokenPath"), false) ?? String.Empty).ToString();
        }
    }
}
