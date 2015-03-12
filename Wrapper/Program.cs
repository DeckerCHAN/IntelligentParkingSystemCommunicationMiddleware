using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCM.UI;

namespace Wrapper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            UIControl control = new UIControl();
            control.Run();
        }
    }
}
