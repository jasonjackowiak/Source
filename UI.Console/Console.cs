using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Import;
using Build;
using Common;
using Modularise;
using Analyse;
using Visualisation;

namespace UI.Console
{
    class Console
    {

        public ConsoleLog _log = new ConsoleLog();

        public void StartConsole()
        {
            _log.BeginLog();

            _log.Log("Welcome to the Fujitsu Application Analysis System (FAAS)");
            _log.Log("Please select your language:");
            _log.Log("ObjectStar (O)");
            _log.Log("SQL (S)");
            _log.Log("To quit, type 'Exit'");
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
                case "EXIT":
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
            _log.Log("Visualization (V)");
            _log.Log("To quit, type 'Exit'");

            //User Inputs here
            string line = System.Console.ReadLine();
            line = line.Trim().ToUpper();

            switch (line)
            {
                case "I":
                    //ProcessOSBInputs import = new ProcessOSBInputs();
                    ProcessSQLInputs import = new ProcessSQLInputs();
                    import.ProcessInputs(_log);
                    ImportInput(language);
                    break;
                case "B":
                    string[] bInput = SQLBuildInput();
                    BuildSQLRelationships build = new BuildSQLRelationships();
                    build.BuildRelations(_log, bInput);
                    ImportInput(language);
                    break;
                case "A":
                    AnalyseAll analyse = new AnalyseAll();
                    analyse.StartAnalysis(_log);
                    ImportInput(language);
                    break;
                case "M":
                    Modules modularise = new Modules();
                    modularise.ModulariseEntities(_log);
                    ImportInput(language);
                    break;
                case "V":
                    string[] vInput = VisualizationInput();
                    BuildGraph buildGraphs = new BuildGraph();
                    buildGraphs.StartGraph(_log, vInput);
                    ImportInput(language);
                    break;
                case "EXIT":
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

        private string[] SQLBuildInput()
        {
            _log.Log("Please enter the entities you wish to build, seperated by a comma(,). Valid entities are: ");
            _log.Log("procedures, tables, triggers");
            _log.Log("Enter entity(s): ");
            string line = System.Console.ReadLine();
            line = line.ToUpper();
            string[] words = line.Split(',');

            foreach (string s in words)
            {
                if (s.Trim().Equals("PROCEDURES"))
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

        private string[] VisualizationInput()
        {
            _log.Log("Please enter the type of graph you wish to build, seperated by a comma(,). Valid types are: ");
            _log.Log("entity, interface, all");
            _log.Log("Enter type(s): ");
            string line = System.Console.ReadLine();
            line = line.ToUpper();
            string[] words = line.Split(',');

            foreach (string s in words)
            {

                if (s.Trim().Equals("ENTITY"))
                {
                }
                else if (s.Trim().Equals("INTERFACE"))
                {
                }
                else if (s.Trim().Equals("ALL"))
                {
                }
                else
                {
                    _log.Log(string.Format("Invalid type entered: {0}", s));
                    //clear array and run method again
                    Array.Clear(words, 0, words.Count());
                    VisualizationInput();
                }
            }
            return words;
        }
    }
}
