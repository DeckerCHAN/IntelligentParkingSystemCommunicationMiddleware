#region

using System;

#endregion

namespace IPSCM.Utils
{
    public static class SqlUtils
    {
        private static readonly String[] SqlFileSplitor = {"GO\r\n", "GO ", "GO\t"};

        public static String[] SplitSqlFile(String fileContent)
        {
            return fileContent.Split(SqlFileSplitor, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}