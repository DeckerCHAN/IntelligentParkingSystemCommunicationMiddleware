using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GUI;

namespace Core
{
    class Engine:ApplicationContext
    {
        private static Engine _instance;

        public static Engine GetEngine()
        {
            return _instance ?? (_instance = new Engine());
        }

        #region fields

        #endregion
        
        private Engine()
        {
            this.MainForm=new MainForm();
        }
    }
}
