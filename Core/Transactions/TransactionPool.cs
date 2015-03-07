using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IPSCM.Logging;

namespace IPSCM.Core.Transactions
{
    public class TransactionPool : IDisposable
    {
        public List<Transaction> Transactions { get; private set; }
        public readonly Thread WipeThread;

        public TransactionPool()
        {
            this.Transactions = new List<Transaction>();
            this.WipeThread = new Thread(this.WipeOut);
        }

        public void AddBeforeExecute(Transaction transaction)
        {
            this.Transactions.Add(transaction);
            transaction.Execute();
        }

        private void WipeOut()
        {
            Log.Info("Transaction pool wipe thread started!");
            while (true)
            {
                try
                {
                    Thread.Sleep(60000);
                    Log.Info("Transaction pool wiping...");
                    var wipeCount = this.Transactions.Count(x => x.Status == TransactionStatus.Errored || x.Status == TransactionStatus.Exhausted);
                    this.Transactions.RemoveAll(x => x.Status == TransactionStatus.Errored || x.Status == TransactionStatus.Exhausted);
                    Log.Info(String.Format("Transaction pool wiped {0} transactions!", wipeCount));
                }
                catch (ThreadInterruptedException)
                {
                    Log.Info("Transaction pool wipe thread stopped!");
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error("Transaction pool wipe out error!", ex);
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
