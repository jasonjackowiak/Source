using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Import.Configuration
{
    public class InputSection : ConfigurationSection
    {
        public InputSection()
        {
        }

        //input section definition
        [ConfigurationProperty("LanguageFiles")]
        public LanguageInputFileElement LanguageInput
        {
            get { return (LanguageInputFileElement)base["LanguageFiles"]; }
        }

        [ConfigurationProperty("DatabaseFiles")]
        public DatabaseInputFileElement DatabaseInput
        {
            get { return (DatabaseInputFileElement)base["DatabaseFiles"]; }
        }

        [ConfigurationProperty("ClientData")]
        public ClientDataElement ClientData
        {
            get { return (ClientDataElement)base["ClientData"]; }
        }

        //input element definitions
        public class DatabaseInputFileElement : ConfigurationElement
        {
            [ConfigurationProperty("TableFile")]
            public FileElement Table
            {
                get { return (FileElement)base["TableFile"]; }
            }

            [ConfigurationProperty("TriggerFile")]
            public FileElement Trigger
            {
                get { return (FileElement)base["TriggerFile"]; }
            }

            [ConfigurationProperty("TableForeignConstraintFile")]
            public FileElement TableForeignConstraint
            {
                get { return (FileElement)base["TableForeignConstraintFile"]; }
            }

            [ConfigurationProperty("TransactionFile")]
            public FileElement Transaction
            {
                get { return (FileElement)base["TransactionFile"]; }
            }

            [ConfigurationProperty("PackageFile")]
            public FileElement Package
            {
                get { return (FileElement)base["PackageFile"]; }
            }

        }

        public class LanguageInputFileElement : ConfigurationElement
        {
            [ConfigurationProperty("RuleFile")]
            public FileElement Rule
            {
                get { return (FileElement)base["RuleFile"]; }
            }

            [ConfigurationProperty("JobCardLanguageFile")]
            public FileElement JobCardLanguage
            {
                get { return (FileElement)base["JobCardLanguageFile"]; }
            }

            [ConfigurationProperty("CobolFile")]
            public FileElement Cobol
            {
                get { return (FileElement)base["CobolFile"]; }
            }

        }

        public class ClientDataElement : ConfigurationElement
        {
            [ConfigurationProperty("Name")]
            public DataElement Name
            {
                get { return (DataElement)base["Name"];}
            }

            [ConfigurationProperty("Language1")]
            public DataElement Language1
            {
                get { return (DataElement)base["Language1"]; }
            }

            [ConfigurationProperty("Database1")]
            public DataElement Database1
            {
                get { return (DataElement)base["Database1"]; }
            }

            [ConfigurationProperty("Language2")]
            public DataElement Language2
            {
                get { return (DataElement)base["Language2"]; }
            }

            [ConfigurationProperty("Database2")]
            public DataElement Database2
            {
                get { return (DataElement)base["Database2"]; }
            }
        }

        //Base elements
        public class FileElement : ConfigurationElement
        {
            [ConfigurationProperty("file")]
            public string File
            {
                get { return (string)base["file"]; }
                set { base["file"] = value; }
            }
        }

        public class DataElement : ConfigurationElement
        {
            [ConfigurationProperty("data")]
            public string Data
            {
                get { return (string)base["data"]; }
                set { base["data"] = value; }
            }
        }

    }
}
