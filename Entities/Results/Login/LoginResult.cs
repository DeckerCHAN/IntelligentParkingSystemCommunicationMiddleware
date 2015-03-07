#region

using IPSCM.Entities.Results.Login;

#endregion

namespace IPSCM.Entities.Results
{
    public class LoginResult : Result
    {
        public LoginInfo Info { get; set; }
    }
}