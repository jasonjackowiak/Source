using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Import;

namespace UI.Console
{
    class Console
    {

        public ConsoleLog _log = new ConsoleLog();

        public void StartConsole()
        {
            _log.BeginLog();

            _log.Log("Welcome to the Fujitsu/Housing SA Mainframe Application Analysis System (MAAS)");
            _log.Log("Please select your language:");
            _log.Log("ObjectStar (O)");
            _log.Log("SQL (S)");
            //User Inputs here
            string line = System.Console.ReadLine();
            line = line.Trim().ToUpper();

            switch (line)
            {
                case "O":
                    ImportInput("OSB");
                    break;
                case "S":
                    ImportInput("SQL");
                    break;
                default:
                    StartConsole();
                    break;
            }
        }

        private void ImportInput(string language)
        {
            _log.Log("Please Select your phase:");
            _log.Log("Import (I)");
            _log.Log("Build (B)");
            _log.Log("Analyse (A)");
            _log.Log("Modularise (M)");

            //User Inputs here
            string line = System.Console.ReadLine();
            line = line.Trim().ToUpper();

            switch (line)
            {
                case "I":
                    //ProcessOSBInputs import = new ProcessOSBInputs();
                    ProcessSQLInputs import = new ProcessSQLInputs();
                    import.ProcessInputs(_log);
                    break;
                case "B":
                    _log.Log("Please enter a valid phase.");
                    _log.Log("Build is under construction.");
                    ImportInput(language);
                    break;
                case "A":
                    _log.Log("Please enter a valid phase.");
                    _log.Log("Analyse is under construction.");
                    ImportInput(language);
                    break;
                case "M":
                    _log.Log("Please enter a valid phase.");
                    _log.Log("Modularise is under construction.");
                    ImportInput(language);
                    break;
                default:
                    _log.Log("Please enter a valid phase.");
                    ImportInput(language);
                    break;
            }
        }

        private string[] OSBBuildInput()
        {
            _log.Log("Please enter the entities you wish to build, seperated by a comma(,). Valid entities are: ");
            _log.Log("rules(includes transactions), tables, triggers");
            _log.Log("Enter entity(s): ");
            string line = System.Console.ReadLine();
            line = line.ToUpper();
            string[] words = line.Split(',');

            foreach (string s in words)
            {
                if (s.Trim().Equals("RULES"))
                {
                }
                else if (s.Trim().Equals("TABLES"))
                {
                }
                else if (s.Trim().Equals("TRIGGERS"))
                {
                }
                else if (s.Trim().Equals("ALL"))
                {
                }
                else
                {
                    _log.Log(string.Format("Invalid entity entered: {0}", s));
                    //clear array and run method again
                    Array.Clear(words, 0, words.Count());
                    OSBBuildInput();
                }
            }
            return words;
        }

    }
}
