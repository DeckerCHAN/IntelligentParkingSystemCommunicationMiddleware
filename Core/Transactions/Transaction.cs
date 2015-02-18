namespace IPSCM.Core.Transactions
{
    public abstract class Transaction
    {
        public TransactionStatus Status { get; protected set; }
        public Transaction()
        {
            this.Status=TransactionStatus.Immature;
        }

        public virtual void Execute()
        {
            this.Status=TransactionStatus.Started;
        }

        public virtual void Interrupt()
        {
            this.Status=TransactionStatus.Exhausted;
        }
    }
}
