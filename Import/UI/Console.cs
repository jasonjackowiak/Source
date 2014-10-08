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
        public ConsoleLog log = new ConsoleLog();

        public void StartConsole()
        {
            //Test accessing data from config

            string clientName = config.ClientName;
            string clientLanguage = config.ClientLanguage1;
            string clientDatabase = config.ClientDatabase1;

            log.Log("Welcome to the Fujitsu Application Analysis System (FAAS)");

            //string line = CustomerInput();


            log.Log(string.Format("Import Phase for {0}", clientName));
            log.Log(string.Format("The following languages are being imported: {0}", clientLanguage));
            log.Log(string.Format("The following databases are being imported: {0}", clientDatabase));
            
            //User Inputs here
            string line = System.Console.ReadLine();
            log.Log("Press any key to continue...");

            ImportLanguageInput(clientLanguage);
            //ImportDatabaseInput(clientDatabase);

            log.Log("Import Phase complete");
            log.Log("Press any key to continue...");
        }

        private string CustomerInput()
        {
            log.Log("Do you want to (A)dd a new Customer or select an (E)xisting?");
            string line = System.Console.ReadLine();
            line = line.Trim().ToUpper();
            switch (line)
            {
                case "A":
                    //ProcessRegistrationInput = new ProcessRegistrationInput(log);
                    break;
                case "E":
                    break;
                default:
                    log.Log("Invalid selection, try again.");
                    CustomerInput();
                    break;
            }
            return line;
        }

        private void ImportLanguageInput(string language)
        {

            switch (language)
            {
                case "Object Service Broker":
                    //Make this ProcessObjectServiceBrokerLanguageInputs
                    ProcessObjectServiceBrokerInput importLanguage = new ProcessObjectServiceBrokerInput();
                    importLanguage.ProcessInputs(log);
                    break;
                default:
                    log.Log("No Language specified");
                    break;
            }

        }

        private void ImportDatabaseInput(string database)
        {
            switch (database)
            {
                case "ObjectServiceBroker":
                    //Make this ProcessObjectServiceBrokerDatabaseInputs
                    ProcessObjectServiceBrokerInput importLanguage = new ProcessObjectServiceBrokerInput();
                    importLanguage.ProcessInputs(log);
                    break;
                default:
                    log.Log("No Language specified");
                    break;
            }
        }

    }
}
