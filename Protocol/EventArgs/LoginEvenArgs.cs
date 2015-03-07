#region

using IPSCM.Entities.Results;

#endregion

namespace IPSCM.Protocol.EventArgs
{
    public class LoginEvenArgs : System.EventArgs
    {
        public LoginEvenArgs(LoginResult entity)
        {
            this.Entity = entity;
        }

        public LoginResult Entity { get; private set; }
    }
}