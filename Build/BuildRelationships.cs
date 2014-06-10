using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Import;
using System.Threading;
using System.Threading.Tasks;

namespace Build
{
    public class BuildRelationships
    {
        #region vars
        private ConsoleLog _settings = new ConsoleLog();
        private List<Entity> _entities = new List<Entity>();
        public List<EntityRelationship> _entityRelations = new List<EntityRelationship>();
        public List<Link> _links = new List<Link>();
        public List<Link> _ruleLinks = new List<Link>();
        public List<Link> _tableLinks = new List<Link>();
        public List<Link> _reportLinks = new List<Link>();
        public List<Link> _screenLinks = new List<Link>();
        public List<Link> _triggerLinks = new List<Link>();
        public List<Link> _transactionLinks = new List<Link>();

        public List<Entity> ents = new List<Entity>();
        public List<RuleDefinition> _ruleDefinitions = new List<RuleDefinition>();
        public List<TableDefinition> _tableDefinitions = new List<TableDefinition>();
        public List<TriggerDefinition> _triggers = new List<TriggerDefinition>();
        private ProcessExtracts p = new ProcessExtracts();
        #endregion

        #region build relations
        public void BuildRelations()
        {
            
            BuildRuleRuleCalls();
        }

        private void BuildRuleRuleCalls()
        {
            _settings.Log("************** BUILD ******************");

            ClearTables();

            //write processing output to same line
            Console.Write("Preparing global data...");
            HousingSAModel _context = new HousingSAModel();
            _entities = _context.Entities.ToList();
            _ruleDefinitions = _context.RuleDefinitions.ToList();
            _tableDefinitions = _context.TableDefinitions.ToList();
            _triggers = _context.TriggerDefinitions.ToList();

            var uniqueRules = ListOfNames("RULE");
            var uniqueTables = ListOfNames("TABLE");
            var uniqueReports = ListOfNames("REPORT");
            var uniqueScreens = ListOfNames("SCREEN");
            var uniqueTriggers = ListOfNames("TRIGGER");
            var uniqueTransactions = ListOfNames("TRAN");
            Console.WriteLine("done");

            //Get calls in rules to rules, screens, tables, reports, job cards, NEED TO ADD TRIGGERS
            GetRuleEntityCalls(_ruleDefinitions, uniqueRules, uniqueTables, uniqueReports, uniqueScreens, uniqueTriggers, uniqueTransactions);

            CreateMasterList();
            ConvertEntityCallsToInt();
            PopulateEntityRelationships();

            _settings.Log("************ BUILD END ****************");
            _settings.EndLog();
        }

        private void GetRuleEntityCalls(List<RuleDefinition> _rules, List<string> uniqueRules, List<string> uniqueTables, List<string> uniqueReports, List<string> uniqueScreens, List<string> uniqueTriggers, List<string> uniqueTransactions)
    {

        string[] words = BuildInput();
        List<Task> _tasks = new List<Task>();
        bool reports = false;
        bool rules = false;
        bool screens = false;
        bool tables = false;
        bool triggers = false;


        foreach (string s in words)
        {

            if (s.Trim().Equals("REPORTS"))
            {
                reports = true;
            }
            if (s.Trim().Equals("RULES"))
            {
                rules = true;
            }
            if (s.Trim().Equals("TABLES"))
            {
                tables = true;
            }
            if (s.Trim().Equals("SCREENS"))
            {
                screens = true;
            }
            if (s.Trim().Equals("TRIGGERS"))
            {
                triggers = true;
            }
            if (s.Trim().Equals("ALL"))
            {
                reports = true;
                rules = true;
                screens = true;
                tables = true;
                triggers = true;
            }

        }

        var f = Task.Factory;

        if (reports)
        {
            var calledReports = f.StartNew(() => BuildCalledReports(_rules, uniqueReports));
            _tasks.Add(calledReports);
        }
        if (screens)
        {
            var calledScreens = f.StartNew(() => BuildCalledScreens(_rules, uniqueScreens));
            _tasks.Add(calledScreens);
        }
        if (triggers)
        {
            var calledTriggers = f.StartNew(() => BuildCalledTriggers(_rules));
            _tasks.Add(calledTriggers);
        }
        if (tables)
        {
            var calledTables = f.StartNew(() => BuildCalledTables(_rules, uniqueTables));
            _tasks.Add(calledTables);
        }
        if (rules)
        {
            var calledTransactions = f.StartNew(() => GetTransactionRuleCalls(uniqueTransactions, uniqueRules));
            _tasks.Add(calledTransactions);
            var calledRules = f.StartNew(() => BuildCalledRules(_rules, uniqueRules));
            _tasks.Add(calledRules);
        }

        //for the method (primary thread) to wait for all worker threads before finishing
        foreach (Task task in _tasks)
        {
            Task.WaitAll(task);
        }

    }



        private void BuildCalledTriggers(List<RuleDefinition> _rules)
    {
        _settings.Log("Finding trigger references - start");
        foreach (RuleDefinition rule in _rules)
        {
            try
            {
                foreach (TriggerDefinition trigger in _triggers)
                {
                    TriggerInRuleMatch(trigger, rule, _triggerLinks);
                }
            }
            catch (Exception e)
            {
                _settings.Log(string.Format("Error reading rule definitions for trigger references: {0}", e.Message));
            }
        }
        _settings.Log("Finding trigger references - start");
    }

        private void BuildCalledScreens(List<RuleDefinition> _rules, List<string> uniqueScreens)
    {
        _settings.Log("Finding screen references - start");
        foreach (RuleDefinition rule in _rules)
        {
            try
            {
                foreach (String screenName in uniqueScreens)
                {
                    ScreenInRuleMatch(screenName, rule, _screenLinks);
                    //write processing output to same line
                    Console.Write("Searching for {0} in {1}            \r", screenName, rule.Name);
                }
            }
            catch (Exception e)
            {
                _settings.Log(string.Format("Error reading rule definitions for screen references: {0}", e.Message));
            }
        }
        _settings.Log("Finding screen references - start");
    }

        private void BuildCalledTables(List<RuleDefinition> _rules, List<string> uniqueTables)
    {
        _settings.Log("Finding table references - start");
        foreach (RuleDefinition rule in _rules)
        {
            try
            {
                foreach (String tableName in uniqueTables)
                {
                    TableInRuleMatch(tableName, rule, _tableLinks);
                    //write processing output to same line
                    Console.Write("Searching for {0} in {1}            \r", tableName, rule.Name);
                }
            }
            catch (Exception e)
            {
                _settings.Log(string.Format("Error reading rule definitions for table references: {0}", e.Message));
            }
        }
        _settings.Log("Finding table references - start");
    }

        private void BuildCalledRules(List<RuleDefinition> _rules, List<string> uniqueRules)
    {

        bool addNotApplicable = true;
        _settings.Log("Finding rule references - start");
        foreach (RuleDefinition rule in _rules)
        {
            
            try
            {

                foreach (String ruleName in uniqueRules)
                {
                    if (!rule.Name.Equals(ruleName))
                    {
                        RuleInRuleMatch(ruleName, rule, _ruleLinks, addNotApplicable);
                    }
                }
            }
            catch (Exception e)
            {
                _settings.Log(string.Format("Error reading rule definitions for rule references: {0}", e.Message));
            }
        }
        _settings.Log("Finding rule references - complete");
    }

        private void BuildCalledReports(List<RuleDefinition> _rules, List<string> uniqueReports)
    {

        _settings.Log("Finding report references - start");

        foreach (RuleDefinition rule in _rules)
        {
            
            try
            {
                foreach (String reportName in uniqueReports)
                {
                    ReportInRuleMatch(reportName, rule, _reportLinks);
                    //write processing output to same line
                    Console.Write("Searching for {0} in {1}            \r", reportName, rule.Name);
                }
                
            }
            catch (Exception e)
            {
                _settings.Log(string.Format("Error reading report definitions for report references: {0}", e.Message));
            }
        }
        _settings.Log("Finding report references - complete");
    }

        private void GetTransactionRuleCalls(List<string> uniqueTransactions, List<string> uniqueRules)
    {
        HousingSAModel _context = new HousingSAModel();
        List<TransactionDefinition> _transactionDefinitions = _context.TransactionDefinitions.ToList();

        var transKeyRules = (from t in _context.TransactionDefinitions
                            select new { t.Name, t.PFKeyRule }).Distinct();

        //Convert results to a Link and add to list of links
        _settings.Log("Finding transaction PFKey rules - start");

        foreach (var item in transKeyRules)
        {
            foreach (String ruleName in uniqueRules)
            {
                //write processing output to same line
                Console.Write("Searching for {0} in {1}            \r",ruleName, item.Name);

                if (item.PFKeyRule.Equals(ruleName))
                {
                    Link record = new Link(item.Name, item.PFKeyRule);
                     bool exists = CheckLinkExists(_transactionLinks, record);

                     if (!exists)
                     {
                         _transactionLinks.Add(record);
                     }
                }
            }
        }
        _settings.Log("Finding transaction PFKey rules - complete");

        //Get distinct list of trans > build rules
        var transBuildRules = (from t in _context.TransactionDefinitions
                             select new { t.Name, t.BuildRule }).Distinct();

        _settings.Log("Finding transaction Build rules - start");

        //Convert results to a Link and add to list of links
        foreach (var item in transBuildRules)
        {
            foreach (String ruleName in uniqueRules)
            {
                //write processing output to same line
                Console.Write("Searching for {0} in {1}            \r", ruleName, item.Name);

                if (item.BuildRule.Equals(ruleName))
                {
                    Link record = new Link(item.Name, item.BuildRule);
                    bool exists = CheckLinkExists(_transactionLinks, record);

                    if (!exists)
                    {
                        _transactionLinks.Add(record);
                    }
                }
            }
        }
        _settings.Log("Finding transaction Build rules - complete");
    }
        #endregion

        #region token recognition

    public bool ReportInRuleMatch(string reportName, RuleDefinition rule, List<Link> _links)
    {
        string match1 = "'" + reportName + "'";
        if (rule.Body.Contains(match1))
        {
            Link record = new Link(rule.Name, reportName);
            bool exists = CheckLinkExists(_links, record);

            if (!exists)
            {
                _links.Add(record);
                return true;
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

    public bool ScreenInRuleMatch(string screenName, RuleDefinition rule, List<Link> _links)
    {
        string match1 = "'" + screenName + "'";
        if (rule.Body.Contains(match1))
        {
            Link record = new Link(rule.Name, screenName);
            bool exists = CheckLinkExists(_links, record);

            if (!exists)
            {
                _links.Add(record);
                return true;
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

    public bool TableInRuleMatch(string calledTable, RuleDefinition rule, List<Link> _links)
    {
        string match1 = " " + calledTable + ";";
        string match2 = " " + calledTable + "(";
        string match3 = " " + calledTable + " ";
        string match4 = calledTable + ".";
        string match5 = "'" + calledTable + "'";

        if (rule.Body.Contains(match1) || rule.Body.Contains(match2) || rule.Body.Contains(match3) || rule.Body.Contains(match4) || rule.Body.Contains(match5))
        {
            Link record = new Link(rule.Name.ToString(), calledTable);

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

    public bool RuleInRuleMatch(string calledRule, RuleDefinition rule, List<Link> _links, bool addNotApplicable)
    {
        //line contains
        string match1 = " " + calledRule + ";";
        string match2 = " " + calledRule + "(";
        string match3 = "(" + calledRule + ";";
        string match4 = "(" + calledRule + "(";
        string match7 = "(" + calledRule + ")";
        string match10 = "= " + calledRule + "(";
        string match24 = "= " + calledRule + ";";
        string match11 = "^= " + calledRule + "(";
        string match25 = "^= " + calledRule + ";";
        string match15 = "> " + calledRule + "(";
        string match27 = "> " + calledRule + ";";
        string match16 = "=> " + calledRule + "(";
        string match28 = "=> " + calledRule + ";";
        string match19 = "< " + calledRule + "(";
        string match29 = "< " + calledRule + ";";
        string match20 = "<= " + calledRule + "(";
        string match30 = "<= " + calledRule + ";";
        string match21 = "(" + calledRule + ",";
        string match22 = ", " + calledRule + ", ";
        string match23 = ", " + calledRule + ")";

        //line starts with
        string match5 = calledRule + "(";
        string match6 = calledRule + ";";
        string match8 = calledRule + " =";
        string match9 = calledRule + " ^=";
        string match13 = calledRule + " >";
        string match14 = calledRule + " =>";
        string match17 = calledRule + " <";
        string match18 = calledRule + " <=";
        string match12 = "^" + calledRule + "(";
        string match26 = "^" + calledRule + ";";


            //donot add a rule if it matches the following string patterns, as they are signals.
            string signal1 = "SIGNAL " + calledRule + ";";
            string signal2 = "ON " + calledRule + ";";
            string signal3 = "UNTIL " + calledRule + ";";

            string line = rule.Body;

            if (!line.Contains(signal1) && !line.Contains(signal2) && !line.Contains(signal3))
            {
                if (line.Contains(match1) || line.Contains(match2) || line.Contains(match3) || line.Contains(match4) || line.Contains(match7) || line.Contains(match10) || line.Contains(match11) || line.Contains(match15) || line.Contains(match16) || line.Contains(match19) || line.Contains(match20) || line.Contains(match21) || line.Contains(match22) || line.Contains(match23) || line.Contains(match24) || line.Contains(match25) || line.Contains(match27) || line.Contains(match28) || line.Contains(match29) || line.Contains(match30) || line.StartsWith(match13) || line.StartsWith(match14) || line.StartsWith(match5) || line.StartsWith(match6) || line.StartsWith(match8) || line.StartsWith(match9) || line.StartsWith(match12) || line.StartsWith(match17) || line.StartsWith(match18) || line.StartsWith(match26))
                {
                    Link record = new Link(rule.Name.ToString(), calledRule);

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

    public void TriggerInRuleMatch(TriggerDefinition trigger, RuleDefinition rule, List<Link> _links)
    {

        List<string> matches = new List<string>();

        //Build up list of stirng pattern matches based on trigger access code
        matches = TriggerCodeConversion(trigger, matches);

        foreach (string s in matches)
        {
            if (rule.Body.StartsWith(s))
            {
                string triggerName = trigger.TableName.ToString() + ", " + trigger.RuleName.ToString() + ", " + trigger.Access.ToString();
                Link record = new Link(rule.Name, triggerName);
                bool exists = CheckLinkExists(_links, record);

                if (!exists)
                {
                    _links.Add(record);
                }
            }
        }
    }

    public List<string> TriggerCodeConversion(TriggerDefinition trigger, List<string> matches)
    {

        string tableToken = " " + trigger.TableName + ";";

        switch (trigger.Access)
        {
            case "I":
                matches.Add("INSERT" + tableToken);
                break;
            case "D":
                matches.Add("DELETE" + tableToken);
                break;
            case "R":
                matches.Add("REPLACE" + tableToken);
                break;
            case "W":
                matches.Add("INSERT" + tableToken);
                matches.Add("DELETE" + tableToken);
                matches.Add("REPLACE" + tableToken);
                break;
            case "G":
                matches.Add("GET" + tableToken);
                matches.Add("FORALL" + tableToken);
                break;
            default:
                matches.Add(" ");
                Console.WriteLine("Invalid triggerCode");
                break;

                //TriggerCodeConversion(trigger) + " " + trigger.TableName + ";"
        }
        return matches;
    }

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
        foreach (Link l in _reportLinks)
        {
            _links.Add(l);
        }
        foreach (Link l in _ruleLinks)
        {
            _links.Add(l);
        }
        foreach (Link l in _screenLinks)
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
        foreach (Link l in _transactionLinks)
        {
            _links.Add(l);
        }
    }

    private void ConvertEntityCallsToInt()
        {

            try
            {
                HousingSAModel _context = new HousingSAModel();
                _settings.Log("Converting entity link names into ID's & populating table - start");
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
                _settings.Log("Converting entity link names into ID's & populating table - complete");
            }
            catch (Exception e)
            {
                _settings.Log(string.Format("Error converting link names to link ID's: {0}", e.Message));
                _settings.Log(string.Format("ERROR: {0}", e.Message));
            }
        }

    private void PopulateEntityRelationships()
    {
        HousingSAModel _context = new HousingSAModel();
        _settings.Log("Persist Entity Relationship to DB - start");
        try
        {
            foreach (EntityRelationship item in _entityRelations)
            {
                _context.EntityRelationships.Add(item);
            }
            _context.SaveChanges();
            _settings.Log("Persist Entity Relationship to DB - complete");
            _settings.Log(string.Format("{0} entity relationships created", _entityRelations.Count()));
        }
        catch (Exception e)
        {
            _settings.Log(string.Format("Error persisting Entity Relationship to DB: {0}", e.Message));
        }
    }

    private void ClearTables()
    {
        p.ClearTable("EntityRelationship");
        p.ClearTable("Bucket");
        p.ClearTable("Interface");
        p.ClearTable("InternalInterface");
        p.ClearTable("EntityResidence");
        p.ClearTable("InterfaceReporting");
        p.ClearTable("BucketReporting");
    }
    #endregion 

    }

  }
