using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPSCM.Utils
{
    public static class SqlUtils
    {
        private static String[] SqlFileSplitor = new []{"GO\r\n", "GO ", "GO\t"};
        public static String[] SplitSqlFile(String fileContent)
        {
            return fileContent.Split(SqlFileSplitor , StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
