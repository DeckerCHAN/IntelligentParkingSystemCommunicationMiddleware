using IPSCM.Entities.Results;

namespace IPSCM.Protocol.EventArgs
{
    public class LoginEvenArgs : System.EventArgs
    {
        public LoginResult Entity { get; private set; }

        public LoginEvenArgs(LoginResult entity)
        {
            this.Entity = entity;
        }
    }
}
