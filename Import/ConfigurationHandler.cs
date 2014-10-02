using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Import.Configuration;
using System.Configuration;

namespace Import
{
    public class ConfigurationHandler
    {
        private static InputSection inputSection;

        //Constructor
        private ConfigurationHandler() { }

        //Copied this from Dave's example, don't exactly understand it :p
        private class ConfigurationHandlerSingletonCreator
        {

            static ConfigurationHandlerSingletonCreator()
            {
                inputSection = (InputSection)ConfigurationManager.GetSection("Input");
            }

            //Pre-populate object
            internal static readonly ConfigurationHandler uniqueInstance = new ConfigurationHandler();
        }

        public static ConfigurationHandler GetConfigurationHandlerInstance
        {
            get { return ConfigurationHandlerSingletonCreator.uniqueInstance; }
        }

        //Languages
        public string RuleFile
        {
            get { return inputSection.LanguageInput.Rule.File; }
        }

        public string JobCardLanguageFile
        {
            get { return inputSection.LanguageInput.JobCardLanguage.File; }
        }

        public string CobolFile
        {
            get { return inputSection.LanguageInput.Cobol.File; }
        }

        //Databases
        public string TableFile
        {
            get { return inputSection.DatabaseInput.Table.File; }
        }

        public string TriggerFile
        {
            get { return inputSection.DatabaseInput.Trigger.File; }
        }

        public string TransactionFile
        {
            get { return inputSection.DatabaseInput.Transaction.File; }
        }

        public string TableForeignConstraintFile
        {
            get { return inputSection.DatabaseInput.TableForeignConstraint.File; }
        }

        public string PackageFile
        {
            get { return inputSection.DatabaseInput.Package.File; }
        }

        //Client Data
        public string ClientName
        {
            get { return inputSection.ClientData.Name.Data; }
        }

        public string ClientLanguage1
        {
            get { return inputSection.ClientData.Language1.Data; }
        }

        public string ClientDatabase1
        {
            get { return inputSection.ClientData.Database1.Data; }
        }

        public string ClientLanguage2
        {
            get { return inputSection.ClientData.Language2.Data; }
        }

        public string ClientDatabase2
        {
            get { return inputSection.ClientData.Database2.Data; }
        }

    }
}
