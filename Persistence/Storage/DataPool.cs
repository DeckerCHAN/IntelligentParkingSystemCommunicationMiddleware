#region

using System;
using System.Data;
using System.IO;
using IPSCM.Configuration;
using IPSCM.Entities.FundElements;
using IPSCM.Entities.Results.Leaving;
using IPSCM.Entities.Results.Parking;
using IPSCM.Logging;
using IPSCM.Utils;

#endregion

namespace IPSCM.Persistence.Storage
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
            var initScript = File.ReadAllText(this.Config.GetString("DBInitializeScript"));

            var scriptSegment = SqlUtils.SplitSqlFile(initScript);
            foreach (var segment in scriptSegment)
            {
                SqlHelper.ExecuteNonQuery(this.ConnectString, CommandType.Text, segment);
            }
            Log.Info(String.Format("{0} DB initial script executed.", scriptSegment.Length));
        }

        public Guid PreCarPark(String plateNumber, DateTime parkTime)
        {
            var g = Guid.NewGuid();
            this.DbExecuteNonQuery(String.Format(
                "insert into IPSCM.dbo.ParkRecord([Id],[RecordId],[PlateNumber],[InTime],[OutTime]) values('{0}',NULL,N'{1}','{2}',NULL)", g.ToString("D"), plateNumber,
                parkTime.ToString("yyyy-MM-dd HH:mm:ss")));
            return g;
        }

        public void PostCarPark(Guid Id, String plateNumber, ParkingResult result)
        {
            this.DbExecuteNonQuery(String.Format("update IPSCM.dbo.ParkRecord set RecordId='{0}' where Id='{1}'",
                result.Info.RecordId, Id));
            if (result.Info.UserId == 0)
            {
                return;
            }
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

        public Boolean TryDeductBalance(String plateNumber, Decimal deductBalance)
        {
            var remblaObj =
                this.DbExecuteScalar(String.Format("select [Money] from IPSCM.dbo.Users where PlateNumber=N'{0}'",
                    plateNumber));
            var currentBal = remblaObj as Decimal? ?? 0;
            if (currentBal > deductBalance)
            {
                this.DbExecuteNonQuery(
                    String.Format("update IPSCM.dbo.Users set [Money]=[Money]-{0} where PlateNumber=N'{1}'",
                        deductBalance, plateNumber));
                return true;
            }
            return false;
        }

        public void UsedTicket(UInt32 ticketId, DateTime useTime)
        {
            try
            {
                this.DbExecuteNonQuery(String.Format("update IPSCM.dbo.Tickets set [UsedTime] = '{0}' where [TicketId]='{1}'", useTime.ToString("yyyy-MM-dd HH:mm:ss"), ticketId));
            }
            catch (Exception)
            {
                Log.Error(String.Format("Can not set ticket {0} as used.", ticketId));
            }
        }

        public UInt32 PreCarLeave(String plateNumber, DateTime leaveTime,Decimal copeMoney,Decimal actualMoney,UInt32 ticketId)
        {
            var record = this.DbExecuteScalar(String.Format(
             @"if exists (select * from IPSCM.dbo.ParkRecord where PlateNumber=N'{0}' and OutTime is NULL and InTime is not NULL )
            begin
            
            select Top 1 [RecordId] from IPSCM.dbo.ParkRecord where PlateNumber=N'{0}' and OutTime is NULL and InTime is not NULL order by [InTime] desc
            update IPSCM.dbo.ParkRecord set [OutTime]='{1}',[CopeMoney]='{3}',[ActualMoney]='{4}',[TicketId]='{5}' where PlateNumber=N'{0}' and OutTime is NULL and InTime is not NULL and [RecordId] in (select Top 1 [RecordId] from IPSCM.dbo.ParkRecord where PlateNumber=N'{0}' and OutTime is NULL and InTime is not NULL order by [InTime] desc) 
            end
            else
            begin
            insert into IPSCM.dbo.ParkRecord([Id],[RecordId],[PlateNumber],[InTime],[OutTime],[CopeMoney],[ActualMoney],[TicketId]) values ('{2}',NULL,N'{0}',NULL,'{1}','{3}','{4}','{5}')
            select '0'
            end", plateNumber, leaveTime.ToString("yyyy-MM-dd HH:mm:ss"), Guid.NewGuid().ToString("D"),copeMoney,actualMoney,ticketId));
            UInt32 result;
            if (!UInt32.TryParse(record.ToString(), out result))
            {
                result = 0;
            }
            return result;
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
                update IPSCM.dbo.Tickets set [Type]='{1}',[Value]='{2}',[UserId]='{3}',[StoreName]=N'{4}'
                end
                else
                begin
                insert into IPSCM.dbo.Tickets ([TicketId],[Type],[Value],[UserId],[StoreName]) values ('{0}','{1}','{2}','{3}',N'{4}')
                end",
                ticket.TicketId, ticket.Type, ticket.Value, ticket.UserId, ticket.StoreName
                ));
        }

        public Ticket GetTicketByPlateNumber(String plateNumber)
        {
            var table = this.DbExecuteDataSet(
                String.Format(
                    "select top 1 [TicketId],[Type],[Value],[UserId],[StoreName] from IPSCM.dbo.Tickets where [UserId] in (select [UserId] from IPSCM.dbo.Users where [PlateNumber] = N'{0}') and [UsedTime] is NULL",
                    plateNumber)).Tables[0];
            if (table.Rows.Count <= 0) return null;
            if (table.Rows.Count > 1)
            {
                Log.Info("Queried more than one tickets, using first one.");
            }
            var items = table.Rows[0].ItemArray;
            var ticket = new Ticket
            {
                TicketId = UInt32.Parse(items[0].ToString()),
                Type = items[1].ToString(),
                Value = Decimal.Parse(items[2].ToString()),
                UserId = UInt32.Parse(items[3].ToString()),
                StoreName = items[4].ToString()
            };
            return ticket;
        }

        public DataTable GetParkingHistoryOrderByTime(UInt32 start,UInt32 end)
        {
            var data =
                this.DbExecuteDataSet(
                    String.Format("select [PlateNumber],[InTime],[OutTime],[CopeMoney],[ActualMoney],[TicketId] from  ( SELECT *, ROW_NUMBER() OVER (ORDER BY [InTime]) as row FROM IPSCM.dbo.ParkRecord ) a where row>{0} and row<={1}", start, end)).Tables[0];
            return data;
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