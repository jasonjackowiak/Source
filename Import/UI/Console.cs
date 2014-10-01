using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Configuration;


namespace Import
{
    class Console
    {

        public ConsoleLog _log = new ConsoleLog();

        public void StartConsole()
        {
            //_log.BeginLog();
            Utility utility = new Utility();
            string test = utility.ReadSetting("Tables");
            

            _log.Log("Welcome to the Fujitsu Application Analysis System (FAAS)");
            _log.Log(string.Format("Import Phase for {0}", "CLIENT PARAM"));
            _log.Log("The following languages are being imported:");
            _log.Log("Put stuff here");
            //User Inputs here
        }

        private void ImportInput(string language)
        {
            //ProcessOSBInputs import = new ProcessOSBInputs();
            ProcessSQLInputs import = new ProcessSQLInputs();
            import.ProcessInputs(_log);
            ImportInput(language);
        }

    }
}
