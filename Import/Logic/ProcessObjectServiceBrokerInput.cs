﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Project1;
using Common;

namespace Import
{
    public class ProcessObjectServiceBrokerInput : ProcessInput
    {
        #region vars
        private ConsoleLog log = new ConsoleLog();
        private ConfigurationHandler config = ConfigurationHandler.GetConfigurationHandlerInstance;

        private List<TableDefinition> _tables = new List<TableDefinition>();
        private List<RuleDefinition> _rules = new List<RuleDefinition>();
        private List<TriggerDefinition> _triggers = new List<TriggerDefinition>();
        private List<TransactionDefinition> _transactions = new List<TransactionDefinition>();
        private List<Entity> _entity = new List<Entity>();

        private Dictionary<string, int> ruleNames = new Dictionary<string, int>(); //Unique list of rules
        private int entityCount = 0;
        private List<Task> _tasks = new List<Task>();
        #endregion

        #region run extract
        public void ProcessInputs(ConsoleLog log)
        {
            log.Log("************** IMPORT *****************");

            ClearTables();

            var f = Task.Factory;
            var extractTables = f.StartNew(() => ExtractTables());
            var extractRules = f.StartNew(() => ExtractRules());

            Task.WaitAll(extractTables);
            PopulateTables();

            Task.WaitAll(extractRules);
            PopulateRules();
            ExtractTriggers();
            PopulateTriggers();
            CreateEntities();
            PopulateEntities();

            log.Log("************ IMPORT END ***************");
        }
        #endregion

        #region Triggers
        private bool ExtractTriggers()
        {
            string triggerFile = "";
            string line;
            int lineNo = 0;
            int triggerCount = 0;

            log.Log("Extract Triggers to local variables - start");

            if (File.Exists(triggerFile))
            {
                StreamReader triggersFile = new StreamReader(triggerFile);
                try
                {
                    while ((line = triggersFile.ReadLine()) != null) // loop through all extract lines
                    {
                        lineNo++;

                        if (lineNo > Common.Constants.triggerLinesToSkip)
                        {
                            triggerCount++;

                            TriggerDefinition trigger = new TriggerDefinition();
                            trigger.TableName = line.Substring(Common.Constants.posTriggerTableName, Common.Constants.posTriggerRuleName - Common.Constants.posTriggerTableName).Trim();
                            trigger.RuleName = line.Substring(Common.Constants.posTriggerRuleName, Common.Constants.lenTriggerRuleName).Trim();
                            trigger.Access = line.Substring(Common.Constants.posTriggerAccess).Trim();

                            foreach (TableDefinition table in _tables)
                            {
                                if (table.Name.Equals(trigger.TableName.ToString()))
                                {
                                    trigger.TableSourceId = table.Id;
                                    trigger.Unit = table.Unit;
                                }
                            }
                            foreach (RuleDefinition rule in _rules)
                            {
                                if (rule.Name.Equals(trigger.RuleName.ToString()))
                                {
                                    trigger.RuleSourceId = rule.Id;
                                    break;
                                }
                            }

                            //write processing output to same line
                            log.Log(string.Format("Reading trigger for  {0}             \r", trigger.TableName));

                            _triggers.Add(trigger);
                        }
                    }

                    triggersFile.Close();
                    log.Log("Extract Triggers to local variables - complete");
                }
                catch (Exception e)
                {
                    triggersFile.Close();
                    log.Log(string.Format("Incorrect format of the Triggers file. Error:{0}", e.Message));
                    return false;
                }
            }
            else
            {
                log.Log("Triggers Data file is missing");
                return false;
            }
            return true;
        }

        private void PopulateTriggers()
        {
            FAASModel _context = new FAASModel();

            log.Log("Persist Triggers to DB - start");
            try
            {
                foreach (TriggerDefinition item in _triggers)
                {
                    _context.TriggerDefinitions.Add(item);
                    _context.SaveChanges();
                }
                log.Log("Persist Triggers to DB - complete");
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Triggers Definitions to DB: {0}: ", e.Message));
            }
        }
        #endregion

        #region Tables

        private bool ExtractTables()
        {
            string tableFile = config.TableFile;
            string line;
            int lineNo = 0;
            int tableCount = 0;

            log.Log("Extract Table Definitions to local variables - start");

            if (File.Exists(tableFile))
            {
                StreamReader tablesFile = new StreamReader(tableFile);
                try
                {
                    while ((line = tablesFile.ReadLine()) != null) // loop through all table extract lines
                    {
                        lineNo++;
                        if (lineNo > Common.Constants.tableLinesToSkip)
                        {
                            tableCount++;
                            TableDefinition tableDefinition = new TableDefinition();
                            tableDefinition.Name = line.Substring(Common.Constants.posTableName, Common.Constants.lenTableName).Trim();
                            tableDefinition.FieldName = line.Substring(Common.Constants.posFieldName, Common.Constants.lenFieldName).Trim();
                            tableDefinition.FieldType = line.Substring(Common.Constants.posFieldType, Common.Constants.lenFieldType).Trim();
                            tableDefinition.FieldSyntax = line.Substring(Common.Constants.posFieldSyntax, Common.Constants.lenFieldSyntax).Trim();
                            tableDefinition.Unit = line.Substring(Common.Constants.posTableUnit, Common.Constants.lenTableUnit).Trim();
                            tableDefinition.TableType = line.Substring(Common.Constants.posTableType, Common.Constants.lenTableType).Trim();
                            tableDefinition.Id = tableCount;

                            try
                            {
                                tableDefinition.FieldLength = Convert.ToInt32(line.Substring(Common.Constants.posFieldLength, Common.Constants.lenFieldLength).Trim());
                                tableDefinition.FieldDecimal = Convert.ToInt32(line.Substring(Common.Constants.posFieldDecimal, Common.Constants.lenFieldDecimal).Trim());
                                tableDefinition.FieldNumber = Convert.ToInt32(line.Substring(Common.Constants.posFieldNumber, Common.Constants.lenFieldNumber).Trim());
                            }
                            catch (FormatException fe)
                            {
                                log.Log("Input string is not a sequence of digits.");
                                log.Log(fe.Message);
                                return false;
                            }
                            catch (OverflowException oe)
                            {
                                log.Log("The number cannot fit in an Int32.");
                                log.Log(oe.Message);
                                return false;
                            }

                            //write processing output to same line
                            log.Log(string.Format("Reading table {0}             \r", tableDefinition.Name));

                            tableDefinition.KeyType = line.Substring(Common.Constants.posKeyType, Common.Constants.lenKeyType).Trim();
                            _tables.Add(tableDefinition);
                        }

                    }

                    tablesFile.Close();
                    log.Log("Extract Table Definitions to local variables - complete");
                }
                catch (Exception e)
                {
                    tablesFile.Close();
                    log.Log(string.Format("Incorrect format of the Table Definitions file. Error:{0}", e.Message));
                    return false;
                }
            }
            else
            {
                log.Log("Table Definitions file is missing");
                return false;
            }
            return true;
        }

        private void PopulateTables()
        {
            FAASModel _context = new FAASModel();


            log.Log("Persist Table Definitions to DB - start");
            try
            {
                foreach (TableDefinition item in _tables)
                //Parallel.ForEach(_tables, item =>
                {
                    _context.TableDefinitions.Add(item);

                }
                //   );
                _context.SaveChanges();
                log.Log("Persist Table Definitions to DB - complete");
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Table Definitions to DB: {0}: ", e.Message));
            }
        }

        #endregion

        #region Rules

        private bool ExtractRules()
        {
            string line;
            string ruleSeparator = GetRuleSeparator();

            bool newRule = false; // indicates the start of processing new rule
            int posRule = 0; // position of the extracted rule
            string ruleName = String.Empty; // extracted rule name
            int lineNo = 0;
            int ruleCount = 0;
            string ruleFile = config.RuleFile;
            string unit = "";
            int unitpos = 0;
            int lineCount = 0;

            log.Log("Extract Rule Definitions to local variables - start");

            if (File.Exists(ruleFile))
            {
                StreamReader rulesFile = new StreamReader(ruleFile);
                try
                {
                    while ((line = rulesFile.ReadLine()) != null) // loop through all rules extract lines
                    {
                        lineNo++;
                        if (lineNo > Common.Constants.rulesLinesToSkip)
                        {
                            line = line.Substring(Common.Constants.charsToSkip);

                            if (newRule)
                            {
                                if (line.Contains("("))
                                {
                                    posRule = line.IndexOf("(");
                                    int x = line.LastIndexOf(';');
                                    unitpos = x + 2;
                                }
                                else if (line.Contains(";"))
                                {
                                    posRule = line.LastIndexOf(';');
                                    unitpos = posRule + 2;
                                    //unit = line.Substring(unitpos,Common.Constants.lenRuleUnit).Trim();
                                }
                                ruleName = line.Substring(0, posRule);
                                //write processing output to same line
                                log.Log(string.Format("Reading rule {0}             \r", ruleName));
                                ruleNames.Add(ruleName, ruleCount);
                                ruleCount++;
                                newRule = false;
                                //unit = line.Substring(unitpos,Common.Constants.lenRuleUnit).Trim();
                            }

                            if (line.Substring(0, Common.Constants.lenRuleSeperator).Trim().Equals(ruleSeparator))
                            {
                                newRule = true;
                                unit = line.Substring(Common.Constants.posRuleUnit).Trim();
                            }
                            else
                            {
                                lineCount++;
                                RuleDefinition ruleDefinition = new RuleDefinition();
                                ruleDefinition.Name = ruleName;
                                ruleDefinition.CodeLine = lineNo;
                                ruleDefinition.Body = line;
                                ruleDefinition.Unit = unit;
                                ruleDefinition.Id = lineCount;
                                _rules.Add(ruleDefinition);
                            }
                        }

                    }

                    rulesFile.Close();
                    log.Log("Extract Rule Definitions to local variables - complete");
                }
                catch (Exception e)
                {
                    rulesFile.Close();
                    log.Log(string.Format("Incorrect format of the Rule file. Error:{0}", e.Message));
                    return false;
                }
            }
            else
            {
                log.Log("Rule file is missing");
                return false;
            }
            return true;
        }

        private string GetRuleSeparator()
        {
            // this is a separator between rules - note, can change!
            string ruleSeparator = null;
            if (string.IsNullOrEmpty(ruleSeparator))
                ruleSeparator = "###";
            return ruleSeparator;
        }

        private void PopulateRules()
        {
            FAASModel _context = new FAASModel();
            log.Log("Persist Rule Definitions to DB - start");
            try
            {
                foreach (RuleDefinition item in _rules)
                {
                    _context.RuleDefinitions.Add(item);
                }
                _context.SaveChanges();
                log.Log("Persist Rule Definitions to DB - complete");
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Rule Definitions to DB: {0}", e.Message));
            }
        }

        #endregion

        #region Entities

        private bool CreateEntities()
        {
            string oldName = "";
            int count = 0;
            log.Log("Assign Entities - start");

            if (CreateRuleEntities(oldName, count) && CreateTableEntities(oldName, count) && CreateTransactionEntities(oldName, count) && CreateTriggerEntities(oldName, count))
            {
                log.Log("Assign Entities - complete");
                log.Log(string.Format("{0} Entities found", entityCount));
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CreateRuleEntities(string oldName, int count)
        {
            int ruleCount = 0;
            foreach (RuleDefinition item in _rules)
            {
                count++;
                if (item.Name != oldName)
                {
                    ruleCount++;
                    entityCount++;
                    Entity rule = new Entity();
                    rule.Name = item.Name;
                    oldName = item.Name;
                    rule.Type = "RULE";
                    rule.SourceUnit = item.Unit;
                    rule.SourceId = item.Id;
                    rule.NormalisedUnit = "N/A";
                    rule.Id = entityCount;
                    _entity.Add(rule);
                }
            }
            log.Log(string.Format("{0} Rule Definitions added", ruleCount));
            return true;
        }

        private bool CreateTableEntities(string oldName, int count)
        {
            int tableCount = 0;
            foreach (TableDefinition item in _tables)
            {
                count++;
                if (item.Name != oldName)
                {
                    tableCount++;
                    entityCount++;
                    Entity table = new Entity();
                    table.Name = item.Name;
                    oldName = item.Name;
                    table.Type = "TABLE";
                    //table.SourceUnit = item.Unit;
                    table.SourceId = item.Id;
                    table.NormalisedUnit = "N/A";
                    table.Id = entityCount;

                    _entity.Add(table);
                }
            }
            log.Log(string.Format("{0} Table Definitions added", tableCount));
            return true;
        }

        private bool CreateTransactionEntities(string oldName, int count)
        {
            int transCount = 0;
            foreach (TransactionDefinition item in _transactions)
            {
                count++;
                if (item.Name != oldName)
                {
                    transCount++;
                    entityCount++;
                    Entity transaction = new Entity();
                    transaction.Name = item.Name;
                    oldName = item.Name;
                    transaction.Type = "TRAN";
                    transaction.SourceUnit = item.Unit;
                    transaction.SourceId = item.Id;
                    transaction.NormalisedUnit = "N/A";
                    transaction.Id = entityCount;

                    _entity.Add(transaction);
                }
            }
            log.Log(string.Format("{0} Transaction Definitions added", transCount));
            return true;
        }

        private bool CreateTriggerEntities(string oldName, int count)
        {
            int triggerCount = 0;
            foreach (TriggerDefinition item in _triggers)
            {
                //triggers consist of a table that fires the rule, and so are made up of the combination of the two - hence the appended name below
                string Name = item.TableName + ", " + item.RuleName + ", " + item.Access;
                count++;
                if (Name != oldName)
                {
                    triggerCount++;
                    entityCount++;
                    Entity trigger = new Entity();
                    trigger.Name = Name;
                    oldName = Name;
                    trigger.Type = "TRIGGER";
                    trigger.SourceUnit = item.Unit;
                    trigger.SourceId = item.Id;
                    trigger.NormalisedUnit = "N/A";
                    trigger.Id = entityCount;

                    _entity.Add(trigger);
                }
            }
            log.Log(string.Format("{0} Trigger Definitions added", triggerCount));
            return true;
        }

        //private bool CreateJobCardEntities(string oldName, int count)
        //{
        //    int jobCardCount = 0;
        //    foreach (JobCardDefinition item in _jobCards)
        //    {
        //        count++;
        //        if (item.Name != oldName)
        //        {
        //            jobCardCount++;
        //            entityCount++;
        //            Entity jobCard = new Entity();
        //            jobCard.Name = item.Name;
        //            oldName = item.Name;
        //            jobCard.Type = "JOBCARD";
        //            jobCard.SourceUnit = item.Unit;
        //            jobCard.SourceId = item.Id;
        //            jobCard.NormalisedUnit = "N/A";
        //            jobCard.Id = entityCount;

        //            _entity.Add(jobCard);
        //        }
        //    }
        //    log.Log(string.Format("{0} Job Card Definitions added", jobCardCount));
        //    return true;
        //}

        private void PopulateEntities()
        {
            FAASModel _context = new FAASModel();

            log.Log("Extract Entities to DB - start");
            try
            {
                foreach (Entity item in _entity)
                {
                    _context.Entities.Add(item);
                }
                _context.SaveChanges();
                log.Log("Extract Entities to DB - complete");
            }
            catch (Exception e)
            {
                log.Log(string.Format("Erorr extracting Entities to DB: {0}", e.Message));
            }

        }

        #endregion

        #region utilities
        /// <summary>
        /// Clear all database tables except audit log.
        /// </summary>
        private void ClearTables()
        {
            System.Console.Write("Clear AuditLog table (Y/N)?");
            string line = System.Console.ReadLine();
            line = line.ToUpper();
            if (line.Trim().Equals("Y"))
            {
                ClearTable("AuditLog");
            }
            else if (line.Trim().Equals("N"))
            {
                System.Console.WriteLine("AuditLog table NOT cleared");
            }
            else
            {
                System.Console.WriteLine(string.Format("Invalid option entered: {0}", line));
                ClearTables();
            }

            ClearTable("OSB.RuleDefinitions");
            ClearTable("OSB.TableDefinitions");
            ClearTable("OSB.TransactionDefinitions");
            ClearTable("OSB.TriggerDefinitions");
            ClearTable("Admin.Buckets");
            ClearTable("Admin.Entities");
            ClearTable("Admin.EntityRelationships");
            ClearTable("Admin.Interfaces");
            ClearTable("Admin.InternalInterfaces");
        }

        public void ClearTable(string table)
        {
            FAASModel _context = new FAASModel();

            try
            {
                _context.Database.ExecuteSqlCommand(string.Format("truncate table {0}", table));
                _context.SaveChanges();

                log.Log(string.Format("{0} table cleared", table));
            }

            catch (Exception e)
            {
                log.Log(string.Format("Error clearing {0} table in DB: {1}", table, e.Message));

            }
        }
    }
        #endregion

}

