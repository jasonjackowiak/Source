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
        private ConsoleLog log = new ConsoleLog();
        private List<Entity> _entities = new List<Entity>();
        public List<EntityRelationship> _entityRelations = new List<EntityRelationship>();
        public List<Link> _links = new List<Link>();
        public List<Link> _procedureLinks = new List<Link>();
        public List<Link> _tableLinks = new List<Link>();
        public List<Link> _triggerLinks = new List<Link>();

        public List<Entity> ents = new List<Entity>();
        public List<ProcedureDefinition> _procedureDefinitions = new List<ProcedureDefinition>();
        public List<TableDefinition> _tableDefinitions = new List<TableDefinition>();
        public List<TriggerDefinition> _triggers = new List<TriggerDefinition>();
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
            HousingSAModel _context = new HousingSAModel();
            _entities = _context.Entities.ToList();
            _procedureDefinitions = _context.ProcedureDefinitions.ToList();
            _tableDefinitions = _context.TableDefinitions.ToList();
            _triggers = _context.TriggerDefinitions.ToList();

            var uniqueProcedures = ListOfNames("PROCEDURE");
            var uniqueTables = ListOfNames("TABLE");
            var uniqueTriggers = ListOfNames("TRIGGER");
            Console.WriteLine("done");

            //Get calls in rules to rules, screens, tables, reports, job cards, NEED TO ADD TRIGGERS
            GetProcedureEntityCalls(_procedureDefinitions, uniqueProcedures, uniqueTables, uniqueTriggers, input);

            CreateMasterList();
            ConvertEntityCallsToInt();
            PopulateEntityRelationships();

            log.Log("************ BUILD END ****************");
            log.EndLog();
        }

        private void GetProcedureEntityCalls(List<ProcedureDefinition> _procedures, List<string> uniqueProcedures, List<string> uniqueTables, List<string> uniqueTriggers, string[] input)
    {

        string[] words = input;
        List<Task> _tasks = new List<Task>();
        bool procedures = false;
        bool tables = false;
        bool triggers = false;


        foreach (string s in words)
        {
            if (s.Trim().Equals("PROCEDURES"))
            {
                procedures = true;
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
                procedures = true;
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
            var calledTables = f.StartNew(() => BuildCalledTables(_procedures, uniqueTables));
            _tasks.Add(calledTables);
        }
        if (procedures)
        {
            var calledRules = f.StartNew(() => BuildCalledProcedures(_procedures, uniqueProcedures));
            _tasks.Add(calledRules);
        }

        //for the method (primary thread) to wait for all worker threads before finishing
        foreach (Task task in _tasks)
        {
            Task.WaitAll(task);
        }

    }

    //    private void BuildCalledTriggers(List<ProcedureDefinition> _procedures)
    //{
    //    log.Log("Finding trigger references - start");
    //    foreach (ProcedureDefinition procedures in _procedures)
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

        private void BuildCalledTables(List<ProcedureDefinition> _procedures, List<string> uniqueTables)
    {
        log.Log("Finding table references in procedures - start");
        foreach (ProcedureDefinition procedure in _procedures)
        {
            try
            {
                foreach (String tableName in uniqueTables)
                {
                    TableInProcedureMatch(tableName, procedure, _tableLinks);
                    //write processing output to same line
                    Console.Write("Searching for {0} in {1}            \r", tableName, procedure.Name);
                }
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error reading procedure definitions for table references: {0}", e.Message));
            }

        }
        //log.Log("Finding table references in procedures - start");
        //foreach (TableDefinitions1 in _tables)
        //{
        //    try
        //    {
        //        foreach (String tableName in uniqueTables)
        //        {
        //            TableInProcedureMatch(tableName, procedure, _tableLinks);
        //            //write processing output to same line
        //            Console.Write("Searching for {0} in {1}            \r", tableName, procedure.Name);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(string.Format("Error reading procedure definitions for table references: {0}", e.Message));
        //    }
        //}
    }

        private void BuildCalledProcedures(List<ProcedureDefinition> _procedures, List<string> uniqueProcedures)
    {

        bool addNotApplicable = true;
        log.Log("Finding rule references - start");
        foreach (ProcedureDefinition procedure in _procedures)
        {
            
            try
            {

                foreach (String procedureName in uniqueProcedures)
                {
                    if (!procedure.Name.Equals(procedureName))
                    {
                        ProcedureInProcedureMatch(procedureName, procedure, _procedureLinks, addNotApplicable);
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

        #endregion

        #region token recognition

    public bool TableInProcedureMatch(string calledTable, ProcedureDefinition procedure, List<Link> _links)
    {
        string match1 = " " + calledTable + ";";
        string match2 = " " + calledTable + "(";
        string match3 = " " + calledTable + " ";
        string match4 = calledTable + ".";
        string match5 = "'" + calledTable + "'";

        if (procedure.Body.Contains(match1) || procedure.Body.Contains(match2) || procedure.Body.Contains(match3) || procedure.Body.Contains(match4) || procedure.Body.Contains(match5))
        {
            Link record = new Link(procedure.Name.ToString(), calledTable);

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

    public bool ProcedureInProcedureMatch(string calledProcedure, ProcedureDefinition procedure, List<Link> _links, bool addNotApplicable)
    {
        //line contains
        string match1 = " " + calledProcedure + ";";
        string match2 = " " + calledProcedure + "(";
        string match3 = "(" + calledProcedure + ";";
        string match4 = "(" + calledProcedure + "(";
        string match7 = "(" + calledProcedure + ")";
        string match10 = "= " + calledProcedure + "(";
        string match24 = "= " + calledProcedure + ";";
        string match11 = "^= " + calledProcedure + "(";
        string match25 = "^= " + calledProcedure + ";";
        string match15 = "> " + calledProcedure + "(";
        string match27 = "> " + calledProcedure + ";";
        string match16 = "=> " + calledProcedure + "(";
        string match28 = "=> " + calledProcedure + ";";
        string match19 = "< " + calledProcedure + "(";
        string match29 = "< " + calledProcedure + ";";
        string match20 = "<= " + calledProcedure + "(";
        string match30 = "<= " + calledProcedure + ";";
        string match21 = "(" + calledProcedure + ",";
        string match22 = ", " + calledProcedure + ", ";
        string match23 = ", " + calledProcedure + ")";

        //line starts with
        string match5 = calledProcedure + "(";
        string match6 = calledProcedure + ";";
        string match8 = calledProcedure + " =";
        string match9 = calledProcedure + " ^=";
        string match13 = calledProcedure + " >";
        string match14 = calledProcedure + " =>";
        string match17 = calledProcedure + " <";
        string match18 = calledProcedure + " <=";
        string match12 = "^" + calledProcedure + "(";
        string match26 = "^" + calledProcedure + ";";


            //donot add a rule if it matches the following string patterns, as they are signals.
            string signal1 = "SIGNAL " + calledProcedure + ";";
            string signal2 = "ON " + calledProcedure + ";";
            string signal3 = "UNTIL " + calledProcedure + ";";

            string line = procedure.Body;

            if (!line.Contains(signal1) && !line.Contains(signal2) && !line.Contains(signal3))
            {
                if (line.Contains(match1) || line.Contains(match2) || line.Contains(match3) || line.Contains(match4) || line.Contains(match7) || line.Contains(match10) || line.Contains(match11) || line.Contains(match15) || line.Contains(match16) || line.Contains(match19) || line.Contains(match20) || line.Contains(match21) || line.Contains(match22) || line.Contains(match23) || line.Contains(match24) || line.Contains(match25) || line.Contains(match27) || line.Contains(match28) || line.Contains(match29) || line.Contains(match30) || line.StartsWith(match13) || line.StartsWith(match14) || line.StartsWith(match5) || line.StartsWith(match6) || line.StartsWith(match8) || line.StartsWith(match9) || line.StartsWith(match12) || line.StartsWith(match17) || line.StartsWith(match18) || line.StartsWith(match26))
                {
                    Link record = new Link(procedure.Name.ToString(), calledProcedure);

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
            HousingSAModel _context = new HousingSAModel();
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
        foreach (Link l in _procedureLinks)
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

    private void ConvertEntityCallsToInt()
        {

            try
            {
                HousingSAModel _context = new HousingSAModel();
                log.Log("Converting entity link names into ID's & populating table - start");
                //Parallel.ForEach(_links, link =>
                foreach (Link link in _links)
                {
                    int a = (from qq in _entities
                             where qq.Name.Equals(link.CallingEnt)
                             select qq.Id).FirstOrDefault();
                    int b = (from qq in _entities
                             where qq.Name.Equals(link.CalledEnt)
                             select qq.Id).FirstOrDefault();
                    EntityRelationship rel = new EntityRelationship();
                    rel.CallingEntityId = a;
                    rel.CalledEntityId = b;
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

    private void PopulateEntityRelationships()
    {
        HousingSAModel _context = new HousingSAModel();
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

        bla.ClearTable("EntityRelationship");
        bla.ClearTable("Bucket");
        bla.ClearTable("Interface");
        bla.ClearTable("InternalInterface");
        bla.ClearTable("EntityResidence");
        bla.ClearTable("InterfaceReporting");
        bla.ClearTable("BucketReporting");
    }
    #endregion 

    }

  }
