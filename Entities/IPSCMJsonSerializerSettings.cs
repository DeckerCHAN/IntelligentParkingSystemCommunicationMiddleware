#region

using Newtonsoft.Json;

#endregion

namespace IPSCM.Entities
{
    public class IPSCMJsonSerializerSettings : JsonSerializerSettings
    {
        public IPSCMJsonSerializerSettings()
        {
            this.ContractResolver = new IPSCMContractResolver();
        }
    }
}