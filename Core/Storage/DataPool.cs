#region

using System;
using System.Data;
using System.IO;
using IPSCM.Configuration;
using IPSCM.Entities;
using IPSCM.Entities.Results.Leaving;
using IPSCM.Entities.Results.Parking;
using IPSCM.Logging;
using IPSCM.Persistence;
using IPSCM.Utils;

#endregion

namespace IPSCM.Core.Storage
{
    public class DataPool
    {
        public DataPool()
        {
            this.Config = FileConfig.FindConfig("Storage.cfg");
            this.ConnectString = this.Config.GetString("DBConnectString");
        }

        public String ConnectString { get; set; }
        private Config Config { get; set; }

        public void Initialize()
        {
            var initScript = File.ReadAllText(Config.GetString("DBInitializeScript"));

            var scriptSegment = SqlUtils.SplitSqlFile(initScript);
            foreach (var segment in scriptSegment)
            {
                SqlHelper.ExecuteNonQuery(this.ConnectString, CommandType.Text, segment);
            }
            Log.Info(String.Format("{0} DB initial script executed.", scriptSegment.Length));
        }

        public Guid PreCarParked(String plateNumber, DateTime parkTime)
        {
            var g = Guid.NewGuid();
            this.DbExecuteNonQuery(String.Format(
                "insert into IPSCM.dbo.ParkRecord values('{0}',NULL,N'{1}','{2}',NULL)", g.ToString("D"), plateNumber,
                parkTime.ToString("yyyy-MM-dd HH:mm:ss")));
            return g;
        }

        public void PostCarParked(Guid Id, String plateNumber, ParkingResult result)
        {
            this.DbExecuteNonQuery(String.Format("update IPSCM.dbo.ParkRecord set RecordId='{0}' where Id='{1}'",
                result.Info.RecordId, Id));

            this.DbExecuteNonQuery(String.Format
                (
                    @"if exists( select * from IPSCM.dbo.Users where UserId = '{0}')
                begin
                update IPSCM.dbo.Users set [Money]='{1}',PlateNumber=N'{3}',PhoneNumber='{2}' where UserId = '{0}'
                end
                else
                begin
                insert into IPSCM.dbo.Users ([UserId],[PlateNumber],[Money],[PhoneNumber]) values ('{0}',N'{3}','{1}','{2}')
                end"
                    , result.Info.UserId, result.Info.Money, result.Info.PhoneNumber, plateNumber));
        }

        public Boolean TryDeductBalance(String plateNumber, UInt64 deductBalance)
        {
            var remblaObj =
                this.DbExecuteScalar(String.Format("select [Money] from IPSCM.dbo.Users where PlateNumber='{0}'",
                    plateNumber));
            var currentBal = remblaObj as ulong? ?? 0;
            if (currentBal > deductBalance)
            {
                this.DbExecuteNonQuery(
                    String.Format("update IPSCM.dbo.Users set [Money]=[Money]-{0} where PlateNumber='{1}'",
                        deductBalance, plateNumber));
                return true;
            }
            return false;
        }

        public void PreCarLeave(String plateNumber, DateTime leaveTime)
        {
            this.DbExecuteNonQuery(String.Format(
                @"if exists (select * from IPSCM.dbo.ParkRecord where PlateNumber=N'{0}' and OutTime is NULL and InTime is not NULL )
            begin
            update IPSCM.dbo.ParkRecord set OutTime='{1}'
            end
            else
            begin
            insert into IPSCM.dbo.ParkRecord values ('{2}',NULL,'{0}',NULL,'{1}')
            end", plateNumber, leaveTime.ToString("yyyy-MM-dd HH:mm:ss"), Guid.NewGuid().ToString("D")));
        }

        public void PostCarLeaved(String plateNumber, LeavingResult result)
        {
        }

        public void UpdateSurplus(Surplus surplus)
        {
            this.DbExecuteNonQuery(String.Format("update IPSCM.dbo.Users set [Money]='{1}' where [UserId]='{0}'",
                surplus.UserId, surplus.Money));
        }

        public void InsertOrUpdateUser(User user)
        {
            this.DbExecuteNonQuery(String.Format(
                @"if exists (select * from IPSCM.dbo.Users where [UserId] = '{0}')
                begin
                update IPSCM.dbo.Users set [PlateNumber]=N'{1}',[Money]='{2}',[PhoneNumber]='{3}'
                end
                else
                begin
                insert into IPSCM.dbo.Users ([UserId],[PlateNumber],[Money],[PhoneNumber]) values ('{0}',N'{1}','{2}','{3}')
                end", user.UserId, user.PlateNumber, user.Money, user.PhoneNumber
                ));
        }

        public void InsertOrUpdateTicket(Ticket ticket)
        {
            this.DbExecuteNonQuery(String.Format(
                @"if exists (select * from IPSCM.dbo.Tickets where [TicketId] = '{0}')
                begin
                update IPSCM.dbo.Tickets set [Type]='{1}',[Value]='{2}',[UserId]='{3}',[StorageName]=N'{4}'
                end
                else
                begin
                insert into IPSCM.dbo.Tickets ([TicketId],[Type],[Value],[UserId],[StorageName]) values ('{0}','{1}','{2}','{3}',N'{4}')
                end",
                ticket.TicketId, ticket.Type, ticket.Value, ticket.UserId, ticket.StoreName
                ));
        }

        public Ticket GetTicketByPlateNumber(String plateNumber)
        {
            var table = this.DbExecuteDataSet(
                String.Format(
                    "select top 1 [TicketId],[Type],[Value],[UserId],[StoreName] from IPSCM.dbo.Tickets where [UserId] in (select [UserId] from IPSCM.dbo.Users where [PlateNumber] = '{0}' )",
                    plateNumber)).Tables[0];
            if (table.Rows.Count <= 0) return null;
            if (table.Rows.Count >= 1)
            {
                Log.Info("Queried more than one tickets, using first one.");
            }
            var items = table.Rows[0].ItemArray;
            var ticket = new Ticket
            {
                TicketId = UInt32.Parse(items[0].ToString()),
                Type = items[1].ToString(),
                Value = Int32.Parse(items[2].ToString()),
                UserId = UInt32.Parse(items[3].ToString()),
                StoreName = items[4].ToString()
            };
            return ticket;
        }

        private void DbExecuteNonQuery(String sql)
        {
            SqlHelper.ExecuteNonQuery(this.ConnectString, CommandType.Text, sql);
        }

        private Object DbExecuteScalar(String sql)
        {
            return SqlHelper.ExecuteScalar(this.ConnectString, CommandType.Text, sql);
        }

        private DataSet DbExecuteDataSet(String sql)
        {
            return SqlHelper.ExecuteDataset(this.ConnectString, CommandType.Text, sql);
        }
    }
}