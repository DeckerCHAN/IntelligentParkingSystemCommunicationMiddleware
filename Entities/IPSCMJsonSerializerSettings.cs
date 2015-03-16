#region

using Newtonsoft.Json;

#endregion

namespace IPSCM.Entities
{
    internal class IPSCMJsonSerializerSettings : JsonSerializerSettings
    {
        public IPSCMJsonSerializerSettings()
        {
            this.ContractResolver = new IPSCMContractResolver();
            this.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}