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
  public class ProcessSQLInputs
  {
    #region vars
      private ConsoleLog log;
      NameValueCollection appSettings = ConfigurationManager.AppSettings;

      private List<TableDefinitions1> _tables = new List<TableDefinitions1>();
      private List<TableForeignConstraint> _tableForeignConstraints = new List<TableForeignConstraint>();
	  private List<ProcedureDefinition> _procedures = new List<ProcedureDefinition>();
      private List<TriggerDefinition> _triggers = new List<TriggerDefinition>();
      private List<Entity> _entity = new List<Entity>();
      
      private Dictionary<string, int> ruleNames = new Dictionary<string, int>(); //Unique list of rules
      private int entityCount = 0;
      private List<Task> _tasks = new List<Task>();
      #endregion

    #region run extract
      public void ProcessInputs(ConsoleLog importLog)
    {
      log = importLog;

      log.Log("************** IMPORT *****************");

      ClearTables(log);

        var f = Task.Factory;
        var extractTables = f.StartNew(() => ReadTables());
        var extractTableForeignConstraints = f.StartNew(() => ExtractTableForeignConstraints());
        //var extractRules = f.StartNew(() => ExtractRules());

        Task.WaitAll(extractTableForeignConstraints);
        PopulateTables();
        PopulateTableForeignConstraints();

        //Task.WaitAll(extractRules);
        //PopulateRules();
		//ExtractTriggers();
		//PopulateTriggers();
        CreateEntities();
        PopulateEntities();

        log.Log("************ IMPORT END ***************");
        log.EndLog();

            }
      #endregion

	//#region Triggers
	//  private bool ExtractTriggers (Workbook xlWorkbook)
	//{
	//  string reportFile = log.GetFromConfig("Triggers");
	//  string line;
	//  int lineNo = 0;
	//  int triggerCount = 0;
      
	//  log.Log("Extract Triggers to local variables - start");


	//      //THIS IS NEW PROCESSING TO USE XLS INSTEAD OF TXT FORMAT
	//        _Worksheet bucketSheet = xlWorkbook.Sheets[1];
	//        Range xlRange = bucketSheet.UsedRange;

	//        int rowCount = xlRange.Rows.Count;
	//        int colCount = xlRange.Columns.Count;
	//        TriggerDefinition trigger = new TriggerDefinition();

	//        log.Log("Importing Triggers - start");

	//      //iterate through all columns/rows in xls
	//        for (int i = 2; i != rowCount + 1; i++)
	//        {
	//            int count = 1;
	//            string name = bucketSheet.Cells[i, count].Value2.ToString();
	//            count++;
	//            string unit = bucketSheet.Cells[i, count].Value2.ToString();
	//            //write processing output to same line
	//            Console.Write("Reading trigger for  {0}             \r", trigger.TableName);

	//            CreateBucket(unit, name, "Application");
	//        }

	//        log.Log("Creating normalised bucket list - complete");
	//    }


	//  //if (File.Exists(reportFile))
	//  //{
	//  //  StreamReader triggersFile = new StreamReader(reportFile);
	//  //  try
	//  //  {
	//  //    while ((line = triggersFile.ReadLine()) != null) // loop through all extract lines
	//  //    {
	//  //      lineNo++;

	//  //      if (lineNo > Constants.triggerLinesToSkip)
	//  //      {
	//  //          triggerCount++;
              
	//  //        TriggerDefinition trigger = new TriggerDefinition();
	//  //        trigger.TableName = line.Substring(Constants.posTriggerTableName, Constants.posTriggerRuleName - Constants.posTriggerTableName).Trim();
	//  //        trigger.RuleName = line.Substring(Constants.posTriggerRuleName, Constants.lenTriggerRuleName).Trim();
	//  //        trigger.Access = line.Substring(Constants.posTriggerAccess).Trim();

	//  //        foreach (TableDefinition table in _tables)
	//  //        {
	//  //            if (table.Name.Equals(trigger.TableName.ToString()))
	//  //            {
	//  //                trigger.TableSourceId = table.Id;
	//  //                trigger.Unit = table.Unit;
	//  //            }
	//  //        }
	//  //                foreach (ProcedureDefinition rule in _rules)
	//  //                {
	//  //                    if (rule.Name.Equals(trigger.RuleName.ToString()))
	//  //                    {
	//  //                        trigger.RuleSourceId = rule.Id;
	//  //                        break;
	//  //                    }
	//  //            }

	//                  //write processing output to same line
	//                  Console.Write("Reading trigger for  {0}             \r", trigger.TableName);

	//          _triggers.Add(trigger);
	//        }
	//    }
      
	//      triggersFile.Close();
	//      log.Log("Extract Triggers to local variables - complete");
	//    }
	//    catch (Exception ex)
	//    {
	//      triggersFile.Close();
	//      log.Log(string.Format("Incorrect format of the Triggers file. Error:{0}", ex.Message));
	//      return false;
	//    }
	//  }
	//  else
	//  {
	//    log.Log("Triggers Data file is missing");
	//    return false;
	//  }
	//  return true;
	//}

	//private void PopulateTriggers ()
	//{
	//     FAASModel _context = new FAASModel();

	//    log.Log("Persist Triggers to DB - start");
	//    try
	//    {
	//        foreach (TriggerDefinition item in _triggers)
	//    {
	//        _context.AddToTriggerDefinitions(item);
	//        _context.SaveChanges();
	//    }
	//    log.Log("Persist Triggers to DB - complete");
	//    }
	//    catch (Exception e)
	//    {
	//        log.Log(string.Format("Error persisting Triggers Definitions to DB: {0}: ", e.Message));
	//    }
	//}
 
	//#endregion

    #region Tables

      private void ReadTables()
      {
          string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetFromConfig("Tables"));
          if (file.Contains(".txt"))
              ExtractTextTables();
          else if (file.Contains(".xls"))
              ReadExcelTables();
      }

	  private void ReadExcelTables()
	  {
		  string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetFromConfig("Tables"));
		  log.Log(string.Format("Opening {0} Excel file", file));
		  try
		  {
			  if (File.Exists(file))
			  {
				  Application xlApp = new Application();
				  Workbook xlWorkbook = xlApp.Workbooks.Open(file);
				  ExtractExcelTables(xlWorkbook);
				  xlWorkbook.Close();
				  xlApp.Quit();
			  }
		  }
		  catch (Exception e)
		  {
			  log.Log(string.Format("Error processing {0} Excel file: {1}", file, e.Message));
		  }
	  }

	  private void ExtractExcelTables(Workbook xlWorkbook)
	  {
		  int tableCount = 0;

		  log.Log("Extract Tables to local variables - start");

		  //THIS IS NEW PROCESSING TO USE XLS INSTEAD OF TXT FORMAT
		  _Worksheet bucketSheet = xlWorkbook.Sheets[1];
		  Range xlRange = bucketSheet.UsedRange;

		  int rowCount = xlRange.Rows.Count;
		  int colCount = xlRange.Columns.Count;
		  TableDefinitions1 table = new TableDefinitions1();

		  log.Log("Importing Tables - start");

		  for (int i = 2; i != rowCount + 1; i++)
		  {
			  //skip the first column in .xls
			  //tableCount++;
			  int count = 2;
			  string name = bucketSheet.Cells[i, count].Value2.ToString();
			  count++;
			  //write processing output to same line
			  Console.Write("Reading table {0}             \r", name);

			  table.Name = name;
			  table.Id = tableCount;
			  _tables.Add(table);
			  tableCount++;
		  }

		  log.Log("Importing Tables - complete");
	  }

      private bool ExtractTextTables()
      {
          string tableFile = GetFromConfig("Tables");
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

                          tableCount++;
                          TableDefinitions1 tableDefinition = new TableDefinitions1();
                          tableDefinition.Name = line.Trim();
                          tableDefinition.Id = tableCount;

                          //write processing output to same line
                          log.Log(string.Format("Reading table {0}             \r", tableDefinition.Name));

                          _tables.Add(tableDefinition);
                      }

                  tablesFile.Close();
                  log.Log("Extract Table Definitions to local variables - complete");
              }
              catch (Exception ex)
              {
                  tablesFile.Close();
                  log.Log(string.Format("Incorrect format of the Table Definitions file. Error:{0}", ex.Message));
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

      private bool ExtractTableForeignConstraints()
      {
          string tableConstraintFile = GetFromConfig("TableForeignConstraints");
          string line;
          int lineNo = 0;
          int tableCount = 0;

          log.Log("Extract Table Foreign Constraints to local variables - start");

          if (File.Exists(tableConstraintFile))
          {
              StreamReader tablesFile = new StreamReader(tableConstraintFile);
              try
              {
                  while ((line = tablesFile.ReadLine()) != null) // loop through all lines
                  {
                      string[] tables = line.Split('>');

                      lineNo++;
                      tableCount++;

                      TableForeignConstraint tableForeignConstraint = new TableForeignConstraint();
                      tableForeignConstraint.Name = tables[0].TrimEnd('-').Trim();
                      tableForeignConstraint.ConastraintName = tables[1].TrimEnd(';').Trim();
                      tableForeignConstraint.Id = tableCount;

                      //write processing output to same line
                      log.Log(string.Format("Reading table {0}             \r", tableForeignConstraint.Name));

                      _tableForeignConstraints.Add(tableForeignConstraint);
                  }

                  tablesFile.Close();
                  log.Log("Extract Table Definitions to local variables - complete");
              }
              catch (Exception ex)
              {
                  tablesFile.Close();
                  log.Log(string.Format("Incorrect format of the Table Definitions file. Error:{0}", ex.Message));
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

      private string GetFromConfig(string p)
      {
          return appSettings.Get(p);
      }

    private void PopulateTables ()
    {
      FAASModel _context = new FAASModel();

      log.Log("Persist Table Definitions to DB - start");
      try
      {
          foreach (TableDefinitions1 item in _tables)
          {
              _context.TableDefinitions1.Add(item);
          }
            _context.SaveChanges();
          log.Log("Persist Table Definitions to DB - complete");
      }
      catch (Exception e)
      {
          log.Log(string.Format("Error persisting Table Definitions to DB: {0}: ", e.Message));
      }
    }

    private void PopulateTableForeignConstraints()
    {
        FAASModel _context = new FAASModel();

        log.Log("Persist Table Foreign Constraints to DB - start");
        try
        {
            foreach (TableForeignConstraint item in _tableForeignConstraints)
            {
                _context.TableForeignConstraints.Add(item);
            }
            _context.SaveChanges();
            log.Log("Persist Table Foreign Constraints to DB - complete");
        }
        catch (Exception e)
        {
            log.Log(string.Format("Error persisting Table Definitions to DB: {0}: ", e.Message));
        }
    }

    #endregion

    #region Procedures

    private bool ExtractRules()
    {
        string line;
        string ruleSeparator = GetRuleSeparator();

        bool newRule = false; // indicates the start of processing new rule
        int posRule = 0; // position of the extracted rule
        string ruleName = String.Empty; // extracted rule name
        int lineNo = 0;
        int ruleCount = 0;
        string ruleFile = GetFromConfig("Rules");
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
                    if (lineNo > Constants.rulesLinesToSkip)
                    {
                        line = line.Substring(Constants.charsToSkip);
                        
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
                                    //unit = line.Substring(unitpos, Constants.lenRuleUnit).Trim();
                                }
                                ruleName = line.Substring(0, posRule);
                                //write processing output to same line
                                Console.Write("Reading rule {0}             \r", ruleName);
                                ruleNames.Add(ruleName, ruleCount);
                                ruleCount++;
                                newRule = false;
                                //unit = line.Substring(unitpos, Constants.lenRuleUnit).Trim();
                            }
                            
                            if (line.Substring(0, Constants.lenRuleSeperator).Trim().Equals(ruleSeparator))
                            {
                                newRule = true;
                                unit = line.Substring(Constants.posRuleUnit).Trim();
                            }
                            else
                            {
                                lineCount++;
                                ProcedureDefinition procedureDefinition = new ProcedureDefinition();
                                procedureDefinition.Name = ruleName;
                                procedureDefinition.CodeLine = lineNo;
                                procedureDefinition.Body = line;
                                //procedureDefinition.Unit = unit;
                                procedureDefinition.Id = lineCount;
                                _procedures.Add(procedureDefinition);
                            }
                    }

                }

                rulesFile.Close();
                log.Log("Extract Rule Definitions to local variables - complete");
            }
            catch (Exception ex)
            {
                rulesFile.Close();
                log.Log(string.Format("Incorrect format of the Rule file. Error:{0}", ex.Message));
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

    private string GetRuleSeparator ()
    {
      // this is a separator between rules - note, can change!
      string ruleSeparator = GetFromConfig("RuleSeparator");
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
            foreach (ProcedureDefinition item in _procedures)
            {
                _context.ProcedureDefinitions.Add(item);
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

	//#region Entities

    private bool CreateEntities()
    {
        string oldName = "";
        int count = 0;
        log.Log("Assign Entities - start");

        if (CreateTableEntities(oldName, count))
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

	private bool CreateTableEntities(string oldName, int count)
	{
		int tableCount = 0;
		foreach (TableDefinitions1 item in _tables)
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

	//private bool CreateTriggerEntities(string oldName, int count)
	//{
	//    int triggerCount = 0;
	//    foreach (TriggerDefinition item in _triggers)
	//    {
	//        //triggers consist of a table that fires the rule, and so are made up of the combination of the two - hence the appended name below
	//        string Name = item.TableName + ", " + item.RuleName + ", " + item.Access;
	//        count++;
	//        if (Name != oldName)
	//        {
	//            triggerCount++;
	//            entityCount++;
	//            Entity trigger = new Entity();
	//            trigger.Name = Name;
	//            oldName = Name;
	//            trigger.Type = "TRIGGER";
	//            trigger.SourceUnit = item.Unit;
	//            trigger.SourceId = item.Id;
	//            trigger.NormalisedUnit = "N/A";
	//            trigger.Id = entityCount;

	//            _entity.Add(trigger);
	//        }
	//    }
	//    log.Log(string.Format("{0} Trigger Definitions added", triggerCount));
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

	//#endregion

    #region utilities
      /// <summary>
      /// Clear all database tables except audit log.
      /// </summary>
    private void ClearTables(ConsoleLog log)
    {
        Utility bla = new Utility();

        bla.ClearTable("SQL.ProcedureDefinitions");
        bla.ClearTable("SQL.TableDefinitions");
        bla.ClearTable("SQL.TableForeignConstraints");
        bla.ClearTable("Admin.Buckets");
        bla.ClearTable("Admin.Entities");
        bla.ClearTable("Admin.EntityRelationships");
        bla.ClearTable("Admin.Interfaces");
        bla.ClearTable("Admin.InternalInterfaces");
    }
    }
    #endregion

  }

