using System.Windows.Forms;
using IPSCM.GUI;

namespace IPSCM.Core
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
