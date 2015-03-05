using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPSCM.Utils
{
    public static class StreamUtils
    {
        public static void WriteToStreamWithUF8(Stream stream, String content)
        {
            Byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            stream.Write(contentBytes, 0, contentBytes.Length);
        }
    }
}
