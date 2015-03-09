#region

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using IPSCM.Configuration;
using IPSCM.Entities.Results.Leaving;
using IPSCM.Entities.Results.Parking;
using IPSCM.Persistence;
using IPSCM.Utils;

#endregion

namespace IPSCM.Core.Storage
{
    public class DataPool
    {
        public String ConnectString { get; set; }
        private Config Config { get; set; }
        public DataPool()
        {
            this.Config = FileConfig.FindConfig("Storage.cfg");
            this.ConnectString = this.Config.GetString("DBConnectString");
        }

        public void Initialize()
        {
            var initScript = File.ReadAllText(Config.GetString("DBInitializeScript"));

            var scriptSegment = SqlUtils.SplitSqlFile(initScript);
            foreach (var segment in scriptSegment)
            {
                SqlHelper.ExecuteNonQuery(this.ConnectString, CommandType.Text, segment);
            }
            Logging.Log.Info(String.Format("{0} DB initial script executed.", scriptSegment.Length));
        }

        public void CarParked(String plateNumber, DateTime parkTime, ParkingResult result)
        {
            this.DbExecuteNonQuery(String.Format("INSERT INTO IPSCM.dbo.ParkRecord values ('{0}',N'{1}','{2}',NULL)", result.Info.RecordId, plateNumber, parkTime.ToString("yyyy-MM-dd HH:mm:ss")));
            if (result.Info.UserId == null || result.Info.PhoneNumber == null || result.Info.Money == null) return;
            this.DbExecuteNonQuery(String.Format
                (
                @"if exists( select * from IPSCM.dbo.Users where UserId = '{0}')
                begin
                update IPSCM.dbo.Users set Money='{1}',PhoneNumber='{2}'
                end
                else
                begin
                insert into IPSCM.dbo.Users (UserId,PlateNumber,Money,PhoneNumber) values ({0},{3},{1},{2})
                end"
                , result.Info.UserId, result.Info.Money, result.Info.PhoneNumber,result.Info.PhoneNumber));
        }

        public void CarLeaved(String plateNumber, DateTime leaveTime, LeavingResult result)
        {

            throw new NotImplementedException();
        }

        private void DbExecuteNonQuery(String sql)
        {
            SqlHelper.ExecuteNonQuery(this.ConnectString, CommandType.Text, sql);
        }

    }
}