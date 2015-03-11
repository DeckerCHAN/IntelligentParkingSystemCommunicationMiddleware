#region

using System;
using IPSCM.Core.Storage;
using IPSCM.Entities;
using IPSCM.Entities.Results;
using IPSCM.Entities.Results.Parking;

#endregion

namespace IPSCM.Tests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DataPool dp = new DataPool();
            dp.Initialize();
          //  dp.PreCarParked("京666666", DateTime.Now, new ParkingResult() { ErrorMessage = null, Info = new ParkingInfo() { Money = 24, PhoneNumber = "15620910626", RecordId = 15, UserId = 24 }, ResultCode = ResultCode.Success });
            Console.ReadKey();
        }
    }
}