using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Configuration;


namespace Import
{
    public class Console
    {
        private ConfigurationHandler config = ConfigurationHandler.GetConfigurationHandlerInstance;
        public ConsoleLog _log = new ConsoleLog();

        public void StartConsole()
        {
            //Test accessing data from config

            string clientName = config.ClientName;
            string clientLanguage = config.ClientLanguage1;
            string clientDatabase = config.ClientDatabase1;

            _log.Log("Welcome to the Fujitsu Application Analysis System (FAAS)");
            _log.Log(string.Format("Import Phase for {0}", clientName));
            _log.Log(string.Format("The following languages are being imported: {0}", clientLanguage));
            _log.Log(string.Format("The following databases are being imported: {0}", clientDatabase));
            
            //User Inputs here
            string line = System.Console.ReadLine();
            _log.Log("Press any key to continue...");

            ImportLanguageInput(clientLanguage);
            //ImportDatabaseInput(clientDatabase);

            _log.Log("Import Phase complete");
        }

        private void ImportLanguageInput(string language)
        {

            switch (language)
            {
                case "ObjectServiceBroker":
                    //Make this ProcessObjectServiceBrokerLanguageInputs
                    ProcessObjectServiceBrokerInputs importLanguage = new ProcessObjectServiceBrokerInputs();
                    importLanguage.ProcessInputs(_log);
                    break;
                default:
                    _log.Log("No Language specified");
                    break;
            }

        }

        private void ImportDatabaseInput(string database)
        {
            switch (database)
            {
                case "ObjectServiceBroker":
                    //Make this ProcessObjectServiceBrokerDatabaseInputs
                    ProcessObjectServiceBrokerInputs importLanguage = new ProcessObjectServiceBrokerInputs();
                    importLanguage.ProcessInputs(_log);
                    break;
                default:
                    _log.Log("No Language specified");
                    break;
            }
        }

    }
}
