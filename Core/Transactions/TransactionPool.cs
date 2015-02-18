using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IPSCM.Logging;

namespace IPSCM.Core.Transactions
{
    public class TransactionPool:IDisposable
    {
        public List<Transaction> Transactions { get; private set; }
        public readonly Thread WipeThread;

        public TransactionPool()
        {
            this.Transactions = new List<Transaction>();
            this.WipeThread=new Thread(this.WipeOut);
        }

        public void AddBeforeExecute(Transaction transaction)
        {
            this.Transactions.Add(transaction);
            transaction.Execute();
        }

        private void WipeOut()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(60000);
                    this.Transactions.RemoveAll(x => x.Status == TransactionStatus.Errored);


                }
                catch (ThreadInterruptedException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error("Transaction pool wipe out error!",ex);
                }
            }
        }

        public void Dispose()
        {
            this.WipeThread.Interrupt();
            foreach (var transaction in Transactions)
            {
                transaction.Interrupt();
            }
        }
    }
}
