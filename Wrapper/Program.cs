using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCM.Core;
using IPSCM.UI;

namespace Wrapper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var e = Engine.GetEngine();
            e.Run();
        }
    }
}
