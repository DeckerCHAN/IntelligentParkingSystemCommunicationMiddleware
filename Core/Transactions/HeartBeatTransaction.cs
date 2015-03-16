#region

using System;
using System.Threading;
using IPSCM.Entities.Results.HeartBeat;
using IPSCM.Logging;

#endregion

namespace IPSCM.Core.Transactions
{
    public sealed class HeartBeatTransaction : Transaction
    {
        public HeartBeatTransaction(HeartBeatResult result)
        {
            this.Result = result;
            this.ProcessThread = new Thread(() =>
            {
                try
                {
                    if (result.Info != null)
                    {
                        if (result.Info.Users != null)
                        {
                            foreach (var user in result.Info.Users)
                            {
                                Engine.GetEngine().Storage.InsertOrUpdateUser(user);
                            }
                        }
                        if (result.Info.Surpluses != null)
                        {
                            foreach (var surplus in result.Info.Surpluses)
                            {
                                Engine.GetEngine().Storage.UpdateSurplus(surplus);
                            }
                        }
                        if (result.Info.Tickets != null)
                        {
                            foreach (var ticket in result.Info.Tickets)
                            {
                                Engine.GetEngine().Storage.InsertOrUpdateTicket(ticket);
                            }
                        }
                        Log.Info(
                            String.Format("Heart beated. Updated or inserted {0} users, {1} surpluses, {2} tickets.",
                                result.Info.Users != null ? result.Info.Users.Count : 0,
                                result.Info.Surpluses != null ? result.Info.Surpluses.Count : 0,
                                result.Info.Tickets != null ? result.Info.Tickets.Count : 0));
                    }
                    this.Status = TransactionStatus.Exhausted;
                }
                catch (Exception ex)
                {
                    this.Status = TransactionStatus.Errored;
                    Log.Error(String.Format("Heart beat encountered a bad error!", ex));
                }
            });
        }

        public Thread ProcessThread { get; private set; }
        public HeartBeatResult Result { get; private set; }

        public override void Execute()
        {
            this.ProcessThread.Start();
            base.Execute();
        }

        public override void Interrupt()
        {
            this.ProcessThread.Interrupt();
            base.Interrupt();
        }
    }
}