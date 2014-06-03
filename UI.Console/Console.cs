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

        private Settings _settings = new Settings();
        private DateTime startTime = new DateTime();

        public void StartConsole()
        {
            System.Console.WriteLine(string.Format("Welcome to the Fujitsu/Housing SA Mainframe Application Analysis System (MAAS)"));
            System.Console.WriteLine(string.Format("Please select your language:"));
            System.Console.WriteLine(string.Format("ObjectStar (O)"));
            System.Console.WriteLine(string.Format("SQL (S)"));
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

        private static void ImportInput(string language)
        {
            System.Console.WriteLine(string.Format("Please Select your phase:"));
            System.Console.WriteLine(string.Format("Import (I)"));
            System.Console.WriteLine(string.Format("Build (B)"));
            System.Console.WriteLine(string.Format("Analyse (A)"));
            System.Console.WriteLine(string.Format("Modularise (M)"));

            //User Inputs here
            string line = System.Console.ReadLine();
            line = line.Trim().ToUpper();

            switch (line)
            {
                case "I":
                    ProcessOSBInputs extract = new ProcessOSBInputs();
                    extract.Extract();
                    break;
                case "B":
                    System.Console.WriteLine("Please enter a valid phase.");
                    System.Console.WriteLine("Build is under construction.");
                    ImportInput(language);
                    break;
                case "A":
                    System.Console.WriteLine("Please enter a valid phase.");
                    System.Console.WriteLine("Analyse is under construction.");
                    ImportInput(language);
                    break;
                case "M":
                    System.Console.WriteLine("Please enter a valid phase.");
                    System.Console.WriteLine("Modularise is under construction.");
                    ImportInput(language);
                    break;
                default:
                    System.Console.WriteLine("Please enter a valid phase.");
                    ImportInput(language);
                    break;
            }
        }

        private string[] BuildInput()
        {
            System.Console.Write("Please enter the entities you wish to build, seperated by a comma(,). Valid entities are: ");
            System.Console.WriteLine("rules(includes transactions), tables, triggers");
            System.Console.Write("Enter entity(s): ");
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
                    _settings.Log(string.Format("Invalid entity entered: {0}", s));
                    //clear array and run method again
                    Array.Clear(words, 0, words.Count());
                    BuildInput();
                }
            }
            return words;
        }

    }
}
