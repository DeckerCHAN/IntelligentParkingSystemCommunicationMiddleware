#region

using System.Linq;
using Newtonsoft.Json.Serialization;

#endregion

namespace IPSCM.Entities
{
    internal class IPSCMContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            string result =
                string.Concat(propertyName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
            return result.ToLower();
        }
    }
}