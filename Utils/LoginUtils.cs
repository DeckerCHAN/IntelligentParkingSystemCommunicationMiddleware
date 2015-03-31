using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPSCM.Utils
{
    public static class LoginUtils
    {
        public static Tuple<String, String> ReadPerservedAccount(FileInfo binaryFile)
        {
            try
            {
                using (var fileStream = new FileStream(binaryFile.FullName, FileMode.Open))
                {

                    var strReader = new StreamReader(fileStream);
                    var str = strReader.ReadToEnd();

                    var base64Array = Convert.FromBase64String(str);

                    var reval = Encoding.UTF8.GetString(base64Array);
                    var userName = reval.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                    var password = reval.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1];
                    return new Tuple<string, string>(userName, password);



                }
            }
            catch (Exception)
            {
                return new Tuple<string, string>(String.Empty, String.Empty);
            }

            throw new NotImplementedException();
        }

        public static void PerserveUserNameAndPasswordToFile(FileInfo binaryFile, String userName, String password)
        {
            var mix = String.Format("{0}{1}{2}", userName, Environment.NewLine, password);
            var base64Str = Convert.ToBase64String(Encoding.UTF8.GetBytes(mix));
            var binary = Encoding.UTF8.GetBytes(base64Str);
            using (var writer = new BinaryWriter(new FileStream(binaryFile.FullName, FileMode.Create)))
            {
                writer.Write(binary);
                writer.Flush();
            }
        }
    }
}
