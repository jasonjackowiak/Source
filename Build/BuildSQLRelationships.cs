using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Project1;
using Common;

namespace Build
{
    public class BuildSQLRelationships
    {
        #region vars
        // Generic lists and lists of links
        private List<Entity> _entities = new List<Entity>();
        public List<EntityRelationship> _entityRelations = new List<EntityRelationship>();
        public List<Link> _links = new List<Link>();
        public List<Link> __functionLinks = new List<Link>();
        public List<Link> _tableLinks = new List<Link>();
        public List<Link> _triggerLinks = new List<Link>();
        public List<Link> _tableForeignConstraintLinks = new List<Link>();

        // specific entity lists
        public List<Entity> ents = new List<Entity>();
        public List<PackageDefinition> _packageDefinitions = new List<PackageDefinition>();
        public List<TableDefinition> _tableDefinitions = new List<TableDefinition>();
        public List<TriggerDefinition> _triggers = new List<TriggerDefinition>();
        public List<TableForeignConstraint> _tableForeignConstraints = new List<TableForeignConstraint>();
        public List<FunctionDefinition> _functionDefinitions = new List<FunctionDefinition>();
        #endregion

        #region build relations
        public void BuildRelations(ConsoleLog log, string[] input)
        {
            BuildProcedureProcedureCalls(log, input);
        }

        private void BuildProcedureProcedureCalls(ConsoleLog log, string[] input)
        {
            log.Log("************** BUILD ******************");

            ClearTables();

            //write processing output to same line
            Console.Write("Preparing global data...");

            //Prepare data lists from DB
            FAASModel _context = new FAASModel();
            _entities = _context.Entities.ToList();
            _functionDefinitions = _context.FunctionDefinitions.ToList();
            _tableDefinitions = _context.TableDefinitions.ToList();
            _triggers = _context.TriggerDefinitions.ToList();
            _tableForeignConstraints = _context.TableForeignConstraints.ToList();

            //Create unique lists for each entity type
            var uniqueFunctions = ListOfNames("FUNCTION");
            var uniqueTables = ListOfNames("TABLE");
            var uniqueTriggers = ListOfNames("TRIGGER");
            Console.WriteLine("done");

            //Get calls in rules to rules, screens, tables, reports, job cards, NEED TO ADD TRIGGERS
            GetPackageBasedReferences(_functionDefinitions, uniqueFunctions, uniqueTables, uniqueTriggers, input, log);
            GetTableBasedReferences(input, log);

            CreateMasterList();
            ConvertEntityCallsToInt(log);
            PopulateEntityRelationships(log);

            log.Log("************ BUILD END ****************");
        }

        private void GetTableBasedReferences(string[] input, ConsoleLog log)
        {
            string[] words = input;
            List<Task> _tasks = new List<Task>();
            bool tables = false;
            bool triggers = false;

            foreach (string s in words)
            {
                if (s.Trim().Equals("TABLES"))
                {
                    tables = true;
                }
                if (s.Trim().Equals("TRIGGERS"))
                {
                    triggers = true;
                }
                if (s.Trim().Equals("ALL"))
                {
                    tables = true;
                    triggers = true;
                }
            }

            var f = Task.Factory;

            if (tables)
            {
                var calledTables = f.StartNew(() => BuildTableForeignConstraints(_tableForeignConstraints, log));
                _tasks.Add(calledTables);
            }

            //for the method (primary thread) to wait for all worker threads before finishing
            foreach (Task task in _tasks)
            {
                Task.WaitAll(task);
            }
        }

        private void GetPackageBasedReferences(List<FunctionDefinition> _procedures, List<string> uniqueFunctions, List<string> uniqueTables, List<string> uniqueTriggers, string[] input, ConsoleLog log)
    {

        string[] words = input;
        List<Task> _tasks = new List<Task>();
        bool functions = false;
        bool tables = false;
        bool triggers = false;

        foreach (string s in words)
        {
            if (s.Trim().Equals("PROCEDURES"))
            {
                functions = true;
            }
            if (s.Trim().Equals("TABLES"))
            {
                tables = true;
            }
            if (s.Trim().Equals("TRIGGERS"))
            {
                triggers = true;
            }
            if (s.Trim().Equals("ALL"))
            {
                functions = true;
                tables = true;
                triggers = true;
            }
        }

        var f = Task.Factory;

        //if (triggers)
        //{
        //    var calledTriggers = f.StartNew(() => BuildCalledTriggers(_procedures));
        //    _tasks.Add(calledTriggers);
        //}
        if (tables)
        {
            var calledTables = f.StartNew(() => BuildCalledTables(_functionDefinitions, uniqueTables, log));
            _tasks.Add(calledTables);
        }
        if (functions)
        {
            var calledProcedures = f.StartNew(() => BuildCalledProcedures(_functionDefinitions, uniqueFunctions, log));
            _tasks.Add(calledProcedures);
        }

        //for the method (primary thread) to wait for all worker threads before finishing
        foreach (Task task in _tasks)
        {
            Task.WaitAll(task);
        }

    }

    //    private void BuildCalledTriggers(List<PackageDefinition> _procedures)
    //{
    //    log.Log("Finding trigger references - start");
    //    foreach (PackageDefinition procedures in _procedures)
    //    {
    //        try
    //        {
    //            foreach (TriggerDefinition trigger in _triggers)
    //            {
    //                TriggerInProcedureMatch(trigger, procedures, _triggerLinks);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            log.Log(string.Format("Error reading rule definitions for trigger references: {0}", e.Message));
    //        }
    //    }
    //    log.Log("Finding trigger references - start");
    //}

        private void BuildCalledTables(List<FunctionDefinition> _functionDefinitions, List<string> uniqueTables, ConsoleLog log)
    {
        log.Log("Finding table references in procedures - start");
        foreach (FunctionDefinition function in _functionDefinitions)
        {
            try
            {
                foreach (String tableName in uniqueTables)
                {
                    TableInProcedureMatch(tableName, function, _tableLinks);
                    //write processing output to same line
                    Console.Write("Searching for {0} in {1}            \r", tableName, function.Name);
                }
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error reading procedure definitions for table references: {0}", e.Message));
            }
        }
    }

        private void BuildCalledProcedures(List<FunctionDefinition> _functions, List<string> uniqueFunctions, ConsoleLog log)
    {

        bool addNotApplicable = true;
        log.Log("Finding rule references - start");
        foreach (FunctionDefinition function in _functions)
        {
            try
            {
                foreach (String functionName in uniqueFunctions)
                {
                    if (!function.Name.Equals(functionName))
                    {
                        ProcedureInProcedureMatch(functionName, function, __functionLinks, addNotApplicable);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error reading rule definitions for rule references: {0}", e.Message));
            }
        }
        log.Log("Finding rule references - complete");
    }

        private void BuildTableForeignConstraints(List<TableForeignConstraint> _tableForeignConstraints, ConsoleLog log)
        {
            log.Log("Finding table references in procedures - start");

                try
                {
                    foreach (TableForeignConstraint tfc in _tableForeignConstraints)
                    {
                    Link record = new Link(tfc.Name, tfc.ConastraintName);

                    bool exists = CheckLinkExists(_links, record);

                    if (!exists)
                    {
                        _links.Add(record);
                        //addNotApplicable = false;
                    }
                    
                }
                    }
                
                catch (Exception e)
                {
                    log.Log(string.Format("Error reading procedure definitions for table references: {0}", e.Message));
                }
            log.Log("Finding table references in procedures - complete");
        }
        #endregion

        #region token recognition

        public bool TableInProcedureMatch(string calledTable, FunctionDefinition function, List<Link> _links)
    {
        string match1 = " " + calledTable + ";";
        string match2 = " " + calledTable + "(";
        string match3 = " " + calledTable + " ";
        string match4 = calledTable + ".";
        string match5 = "'" + calledTable + "'";

        if (function.Body.Contains(match1) || function.Body.Contains(match2) || function.Body.Contains(match3) || function.Body.Contains(match4) || function.Body.Contains(match5))
        {
            Link record = new Link(function.Name.ToString(), calledTable);

            bool exists = CheckLinkExists(_links, record);
            if (!exists)
            {
                _links.Add(record);
                return true; // no need to loop any more
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool ProcedureInProcedureMatch(string calledFunction, FunctionDefinition function, List<Link> _links, bool addNotApplicable)
    {
        //line contains
        string match1 = " " + calledFunction + ";";
        string match2 = " " + calledFunction + "(";
        string match3 = "(" + calledFunction + ";";
        string match4 = "(" + calledFunction + "(";
        string match7 = "(" + calledFunction + ")";
        string match10 = "= " + calledFunction + "(";
        string match24 = "= " + calledFunction + ";";
        string match11 = "^= " + calledFunction + "(";
        string match25 = "^= " + calledFunction + ";";
        string match15 = "> " + calledFunction + "(";
        string match27 = "> " + calledFunction + ";";
        string match16 = "=> " + calledFunction + "(";
        string match28 = "=> " + calledFunction + ";";
        string match19 = "< " + calledFunction + "(";
        string match29 = "< " + calledFunction + ";";
        string match20 = "<= " + calledFunction + "(";
        string match30 = "<= " + calledFunction + ";";
        string match21 = "(" + calledFunction + ",";
        string match22 = ", " + calledFunction + ", ";
        string match23 = ", " + calledFunction + ")";

        //line starts with
        string match5 = calledFunction + "(";
        string match6 = calledFunction + ";";
        string match8 = calledFunction + " =";
        string match9 = calledFunction + " ^=";
        string match13 = calledFunction + " >";
        string match14 = calledFunction + " =>";
        string match17 = calledFunction + " <";
        string match18 = calledFunction + " <=";
        string match12 = "^" + calledFunction + "(";
        string match26 = "^" + calledFunction + ";";


            //donot add a rule if it matches the following string patterns, as they are signals.
            string signal1 = "SIGNAL " + calledFunction + ";";
            string signal2 = "ON " + calledFunction + ";";
            string signal3 = "UNTIL " + calledFunction + ";";

            string line = function.Body;

            if (!line.Contains(signal1) && !line.Contains(signal2) && !line.Contains(signal3))
            {
                if (line.Contains(match1) || line.Contains(match2) || line.Contains(match3) || line.Contains(match4) || line.Contains(match7) || line.Contains(match10) || line.Contains(match11) || line.Contains(match15) || line.Contains(match16) || line.Contains(match19) || line.Contains(match20) || line.Contains(match21) || line.Contains(match22) || line.Contains(match23) || line.Contains(match24) || line.Contains(match25) || line.Contains(match27) || line.Contains(match28) || line.Contains(match29) || line.Contains(match30) || line.StartsWith(match13) || line.StartsWith(match14) || line.StartsWith(match5) || line.StartsWith(match6) || line.StartsWith(match8) || line.StartsWith(match9) || line.StartsWith(match12) || line.StartsWith(match17) || line.StartsWith(match18) || line.StartsWith(match26))
                {
                    Link record = new Link(function.Name.ToString(), calledFunction);

                    bool exists = CheckLinkExists(_links, record);

                    if (!exists)
                    {
                        _links.Add(record);
                        addNotApplicable = false;
                    }
                    
                }
            }
            return addNotApplicable;
      }

    //public void TriggerInRuleMatch(TriggerDefinition trigger, RuleDefinition rule, List<Link> _links)
    //{

    //    List<string> matches = new List<string>();

    //    //Build up list of stirng pattern matches based on trigger access code
    //    matches = TriggerCodeConversion(trigger, matches);

    //    foreach (string s in matches)
    //    {
    //        if (rule.Body.StartsWith(s))
    //        {
    //            string triggerName = trigger.TableName.ToString() + ", " + trigger.RuleName.ToString() + ", " + trigger.Access.ToString();
    //            Link record = new Link(rule.Name, triggerName);
    //            bool exists = CheckLinkExists(_links, record);

    //            if (!exists)
    //            {
    //                _links.Add(record);
    //            }
    //        }
    //    }
    //}

    //public List<string> TriggerCodeConversion(TriggerDefinition trigger, List<string> matches)
    //{

    //    string tableToken = " " + trigger.TableName + ";";

    //    switch (trigger.Access)
    //    {
    //        case "I":
    //            matches.Add("INSERT" + tableToken);
    //            break;
    //        case "D":
    //            matches.Add("DELETE" + tableToken);
    //            break;
    //        case "R":
    //            matches.Add("REPLACE" + tableToken);
    //            break;
    //        case "W":
    //            matches.Add("INSERT" + tableToken);
    //            matches.Add("DELETE" + tableToken);
    //            matches.Add("REPLACE" + tableToken);
    //            break;
    //        case "G":
    //            matches.Add("GET" + tableToken);
    //            matches.Add("FORALL" + tableToken);
    //            break;
    //        default:
    //            matches.Add(" ");
    //            Console.WriteLine("Invalid triggerCode");
    //            break;

    //            //TriggerCodeConversion(trigger) + " " + trigger.TableName + ";"
    //    }
    //    return matches;
    //}

    #endregion

    #region utilities

    private static List<string> ListOfNames(string type)
        {
            FAASModel _context = new FAASModel();
            //create entity (of object type) object
            var uniqueNames = (from e in _context.Entities
                                      where e.Type.Equals(type)
                                      select e.Name).Distinct().ToList();
            return uniqueNames;
        }

    public bool CheckLinkExists(List<Link> _links, Link record)
    {
        bool exists = false;
        foreach (Link l in _links)
        {
            if (l.CallingEnt.Equals(record.CallingEnt) && l.CalledEnt.Equals(record.CalledEnt))
            {
                exists = true;
            }
        }
        return exists;
    }

    private void CreateMasterList()
    {
        foreach (Link l in __functionLinks)
        {
            _links.Add(l);
        }
        foreach (Link l in _tableLinks)
        {
            _links.Add(l);
        }
        foreach (Link l in _triggerLinks)
        {
            _links.Add(l);
        }
    }

    private void ConvertEntityCallsToInt(ConsoleLog log)
        {

            try
            {
                FAASModel _context = new FAASModel();
                log.Log("Converting entity link names into ID's & populating table - start");
                //Parallel.ForEach(_links, link =>
                foreach (Link link in _links)
                {
                    int a = (from x in _entities
                             where x.Name.Equals(link.CallingEnt)
                             select x.Id).FirstOrDefault();
                    int b = (from x in _entities
                             where x.Name.Equals(link.CalledEnt)
                             select x.Id).FirstOrDefault();

                        //Create a relationship between the two id's and add to list
                        EntityRelationship rel = new EntityRelationship();
                        rel.CallingEntityId = a;
                        rel.CalledEntityId = b;

                    //Do not add a 0-0 relationship (this is likely a failure)
                    //Currently not adding any 0 relationships (these are unreferenced tables, will need to figure out what to do for these)
                    if ((!a.Equals(0)) && (!b.Equals(0)))
                        _entityRelations.Add(rel);
                }
                log.Log("Converting entity link names into ID's & populating table - complete");
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error converting link names to link ID's: {0}", e.Message));
                log.Log(string.Format("ERROR: {0}", e.Message));
            }
        }

    private void PopulateEntityRelationships(ConsoleLog log)
    {
        FAASModel _context = new FAASModel();
        log.Log("Persist Entity Relationship to DB - start");
        try
        {
            foreach (EntityRelationship item in _entityRelations)
            {
                _context.EntityRelationships.Add(item);
            }
            _context.SaveChanges();
            log.Log("Persist Entity Relationship to DB - complete");
            log.Log(string.Format("{0} entity relationships created", _entityRelations.Count()));
        }
        catch (Exception e)
        {
            log.Log(string.Format("Error persisting Entity Relationship to DB: {0}", e.Message));
        }
    }

    private void ClearTables()
    {
        Utility bla = new Utility();

        bla.ClearTable("Admin.EntityRelationships");
        bla.ClearTable("Admin.Buckets");
        bla.ClearTable("Admin.Interfaces");
        bla.ClearTable("Admin.InternalInterfaces");
        bla.ClearTable("Admin.EntityResidence");
        bla.ClearTable("Admin.InterfaceReporting");
        bla.ClearTable("Admin.BucketReporting");
    }
    #endregion 

    }

  }
