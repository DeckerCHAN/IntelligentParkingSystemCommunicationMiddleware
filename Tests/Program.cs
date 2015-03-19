using System;
using System.Data;
using IPSCM.Persistence.Storage;
using IPSCM.UI;


namespace Tests
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            DataPool dp=new DataPool();
            var res = dp.GetParkingHistoryOrderByTime(0,1);
            foreach (DataRow row in res.Rows)
            {
                foreach (DataColumn column in res.Columns)
                {
                    Console.Write(row[column]+"||");
                }
                Console.WriteLine();
            }
            //Console.ReadKey();
        }
    }
}
