using System;
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
using System.Data.Entity.Validation;
using System.Diagnostics;
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
	  private List<PackageDefinition> _packages = new List<PackageDefinition>();
      private List<TriggerDefinitions1> _triggers = new List<TriggerDefinitions1>();
      private List<FunctionDefinition> _functions = new List<FunctionDefinition>();
      private List<Entity> _entity = new List<Entity>();
      
      //MAY NEED TO ATER THIS TO INCLUDE FUNCTIONS/SPs
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
        var extractTriggers = f.StartNew(() => ReadTriggers());
        var extractPackages = f.StartNew(() => ReadPackages());

        Task.WaitAll(extractPackages);

        PopulateTables();
        PopulateTableForeignConstraints();
        PopulatePackages();
        PopulateTriggers();

        //This must be run after Packages have been persisted to model
        // to ensure Id (link) is taken from model
        RePopulatePackageList();
        ExtractPackageFunctions();
        PopulateFunctions();

        //Create list of eneities and persist to model
        CreateEntities();
        PopulateEntities();

        log.Log("************ IMPORT END ***************");
        log.EndLog();
            }
      #endregion

	#region Triggers
      private void ReadTriggers()
      {
          string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetFromConfig("Triggers"));
          ReadExcelTriggers(file);
      }

      private void ReadExcelTriggers(string filePath)
      {

          log.Log(string.Format("Opening {0} Excel file", filePath));
          try
          {
              if (File.Exists(filePath))
              {
                  Application xlApp = new Application();
                  Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);
                  ExtractExcelTriggers(xlWorkbook);
                  xlWorkbook.Close();
                  xlApp.Quit();
              }
          }
          catch (Exception e)
          {
              log.Log(string.Format("Error processing {0} Excel file: {1}", filePath, e.Message));
          }
      }

	private bool ExtractExcelTriggers (Workbook xlWorkbook)
	{
      
	  log.Log("Extract Triggers to local variables - start");

	      //THIS IS NEW PROCESSING TO USE XLS INSTEAD OF TXT FORMAT
      _Worksheet sheet = xlWorkbook.Sheets[1];
      Range xlRange = sheet.UsedRange;

	        int rowCount = xlRange.Rows.Count;
	        int colCount = xlRange.Columns.Count;

	        log.Log("Importing Triggers - start");
          
            try {
	        
                //iterate through all columns/rows in xls
	        for (int i = 2; i != rowCount + 1; i++)
	        {
                TriggerDefinitions1 trigger = new TriggerDefinitions1();
	            int count = 3;
                trigger.Name = sheet.Cells[i, count].Value2.ToString();
	            count++;
                trigger.Type = sheet.Cells[i, count].Value2.ToString();
                count++;
                trigger.TriggeringEvent = sheet.Cells[i, count].Value2.ToString();
                count++;
                count++;
                trigger.BaseObjectType = sheet.Cells[i, count].Value2.ToString();
                count++;
                trigger.TableName = sheet.Cells[i, count].Value2.ToString();
                count++;
                count++;
                count++;
                trigger.WhenClause = sheet.Cells[i, count].Value2.ToString();
                count++;
                count++;
                count++;
                count++;
                trigger.Body = sheet.Cells[i, count].Value2.ToString();
                //trigger.Unit = bucketSheet.Cells[i, count].Value2.ToString();

	            //write processing output to same line
	            log.Log(String.Format("Reading trigger {0} \r", trigger.Name));
                _triggers.Add(trigger);
	        }

	      log.Log("Extract Triggers to local variables - complete");
	    }
	    catch (Exception e)
	    {
	      log.Log(string.Format("Error reading trigger file: {0}", e));
	      return false;
	    }

	  return true;
	}

	private void PopulateTriggers ()
	{
	     FAASModel _context = new FAASModel();

	    log.Log("Persist Triggers to DB - start");
        try
        {
	        foreach (TriggerDefinitions1 item in _triggers)
	    {
	        _context.TriggerDefinitions1.Add(item);
	        _context.SaveChanges();
	    }
	    log.Log("Persist Triggers to DB - complete");
        }
        catch (DbEntityValidationException e)
        {
            log.Log(string.Format("Error persisting Triggers Definitions to DB: {0}: ", e));

    foreach (var validationErrors in e.EntityValidationErrors)
    {
        foreach (var validationError in validationErrors.ValidationErrors)
        {
            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
        }
    }
}
        }
	#endregion

    #region Tables
      private void ReadTables()
      {
          string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetFromConfig("Tables"));
          if (file.Contains(".txt"))
              ExtractTextTables(file);
          else if (file.Contains(".xls"))
              ReadExcelTables(file);
      }

	  private void ReadExcelTables(string filePath)
	  {
		  string file = filePath;
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

      private bool ExtractTextTables(string filePath)
      {
          string file = filePath;
          string line;
          int lineNo = 0;
          int tableCount = 0;

          log.Log("Extract Table Definitions to local variables - start");

          if (File.Exists(file))
          {
              StreamReader tablesFile = new StreamReader(file);
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

    #region Packages

    private void ReadPackages()
    {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetFromConfig("Procedures"));
        ReadExcelPackages(file);
    }

    private void ReadExcelPackages(string filePath)
    {

        log.Log(string.Format("Opening {0} Excel file", filePath));
        try
        {
            if (File.Exists(filePath))
            {
                Application xlApp = new Application();
                Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);
                ExtractExcelPackages(xlWorkbook);
                xlWorkbook.Close();
                xlApp.Quit();
            }
        }
        catch (Exception e)
        {
            log.Log(string.Format("Error processing {0} Excel file: {1}", filePath, e.Message));
        }
    }

    private bool ExtractExcelPackages(Workbook xlWorkbook)
    {
        log.Log("Extract Packages to local variables - start");

        //THIS IS NEW PROCESSING TO USE XLS INSTEAD OF TXT FORMAT
        _Worksheet sheet = xlWorkbook.Sheets[1];
        Range xlRange = sheet.UsedRange;

        int rowCount = xlRange.Rows.Count;
        int colCount = xlRange.Columns.Count;
        string oldName = "";

        log.Log("Importing Packages - start");

        try
        {
            //iterate through all columns/rows in xls
            for (int i = 2; i != rowCount + 1; i++)
            {
                PackageDefinition package = new PackageDefinition();
                int count = 3;
                package.Name = sheet.Cells[i, count].Value2.ToString();
                count++;
                package.Type = sheet.Cells[i, count].Value2.ToString();
                count++;
                package.CodeLine = Convert.ToInt32(sheet.Cells[i, count].Value2);
                count++;
                package.Body = sheet.Cells[i, count].Value2.ToString();

                //write processing output to same line
                if (!oldName.Equals(package.Name))
                {
                    log.Log(String.Format("Reading Package {0} \r", package.Name));
                    oldName = package.Name;
                }

                _packages.Add(package);
            }

            log.Log("Extract Packages to local variables - complete");
        }
        catch (Exception e)
        {
            log.Log(string.Format("Error reading Package file: {0}", e));
            return false;
        }

        return true;
    }

    private bool RePopulatePackageList()
    {
        log.Log(String.Format("Repopulating Package Definitions from Database."));
        FAASModel _repopulateContext = new FAASModel();
        _packages.Clear();
        _packages = _repopulateContext.PackageDefinitions.ToList();
        log.Log(string.Format("Repopulation of Package Definitions from Database complete."));
        return true;
    }

    private bool ExtractPackageFunctions()
    {
        log.Log("Extract Stored Procedures and Functions to local variables from parsed packages - start");

        //Extract the SQL functions from the Oracle packages that have already been entered into a List
        //This will mostly comprise of syntax methods to recognise the start and end of functions and put them into their own list
        //Maybe also create a list of function names (here or somewhere else)
        string name = "";
        string type = "";
        string unit = "";
        bool newFunction = false;

        foreach (PackageDefinition package in _packages) // loop through all package lines
        {
            FunctionDefinition function = new FunctionDefinition();

            //Need to grab each line somewhere and ensure each is iterated through, has the same checks applied
            //Assignment of function name
            if (((name.Equals("")) | (newFunction)) && (package.Type.Equals("PACKAGE BODY"))) {
                name = FindFunctionOrProcedureName(package.Body, name, type)[0];
                type = FindFunctionOrProcedureName(package.Body, name, type)[1];
                if ((!name.Equals(null)) && (!type.Equals(null)))
                {
                    newFunction = false;
                }
            }

            if ((!newFunction) && (package.Type.Equals("PACKAGE BODY")) && (!name.Equals("")))
            {
                function.PackageId = package.Id;
                function.Name = name;
                //TO DO - use a method to determine the type
                function.Type = type;
                function.CodeLine = package.CodeLine;
                function.Body = package.Body;
                _functions.Add(function);
            }

            if (package.Body.Contains("END "))
            {
                newFunction = true;
                //Probably need to create a function to determine the unit
                //Should be able to deconstruct the name using the supplied list of applications
                //function.Unit = unit;
            }

            }

        return true;
    }

    private string[] FindFunctionOrProcedureName(string line, string name, string type)
    {
        int nameStart = 0;
        int nameEnd = 0;

        //Reset strings
        name = "";
        type = "";

        if (line.StartsWith("FUNCTION "))
        {
            if (line.Contains("("))
            {
                nameEnd = line.IndexOf("(");
            }
            else
            {
                nameEnd = line.Length;
            }

            nameStart = Constants.posFunctionName;
            int length = nameEnd - nameStart;
            name = line.Substring(nameStart, length);
            type = "Function";
        }
        else if (line.StartsWith("PROCEDURE "))
        {
            if (line.Contains("("))
            {
                nameEnd = line.IndexOf("(");
            }
            else
            {
                nameEnd = line.Length;
            }

            nameStart = Constants.posProcedureName;
            int length = nameEnd - nameStart;
            name = line.Substring(nameStart, length);
            type = "Procedure";
        }

        string[] strings = {name, type};

        return strings;
    }
      
    private void PopulatePackages()
    {
        FAASModel _context = new FAASModel();
        log.Log("Persist Package Definitions to DB - start");
        try
        {
            foreach (PackageDefinition item in _packages)
            {
                _context.PackageDefinitions.Add(item);
            }
            _context.SaveChanges();
            log.Log("Persist Package Definitions to DB - complete");
        }
        catch (Exception e)
        {
            log.Log(string.Format("Error persisting Package Definitions to DB: {0}", e.Message)); 
        }
    }

    private void PopulateFunctions()
    {
        FAASModel _context = new FAASModel();
        log.Log("Persist Function Definitions to DB - start");
        try
        {
            foreach (FunctionDefinition item in _functions)
            {
                _context.FunctionDefinitions.Add(item);
            }
            _context.SaveChanges();
            log.Log("Persist Function Definitions to DB - complete");
        }
        catch (DbEntityValidationException e)
        {
            log.Log(string.Format("Error persisting Function Definitions to DB: {0}", e.Message));
            foreach (var eve in e.EntityValidationErrors)
            {
                log.Log(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validations errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                foreach (var xxx in eve.ValidationErrors){
                    log.Log(string.Format(" - Property: \"{0}\", Error: \"{1}\"", xxx.PropertyName, xxx.ErrorMessage));
                }
            }
            
        }
    }
    #endregion

	#region Entities
    private bool CreateEntities()
    {
        string oldName = "";
        int count = 0;
        log.Log("Assign Entities - start");

        if (CreateTableEntities(oldName, count) && CreateTriggerEntities(oldName, count) && CreateProcedureEntities(oldName, count) && CreateFunctionEntities(oldName,count))
        {
            log.Log("Assign Entities - complete");
            log.Log(string.Format("{0} Entities found", entityCount));
            return true;
        }
        else
        return false;
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

    private bool CreateProcedureEntities(string oldName, int count)
    {
        int procedureCount = 0;
        foreach (PackageDefinition item in _packages)
        {
            count++;
            if (item.Name != oldName)
            {
                procedureCount++;
                entityCount++;
                Entity procedure = new Entity();
                procedure.Name = item.Name;
                oldName = item.Name;
                procedure.Type = "PACKAGE";
                //table.SourceUnit = item.Unit;
                procedure.SourceId = item.Id;
                procedure.NormalisedUnit = "N/A";
                procedure.Id = entityCount;

                _entity.Add(procedure);
            }
        }
        log.Log(string.Format("{0} Package Definitions added", procedureCount));
        return true;
    }

    private bool CreateFunctionEntities(string oldName, int count)
    {
        int functionCount = 0;
        foreach (FunctionDefinition item in _functions)
        {
            count++;
            if (item.Name != oldName)
            {
                functionCount++;
                entityCount++;
                Entity function = new Entity();
                function.Name = item.Name;
                oldName = item.Name;
                function.Type = "FUNCTION";
                //table.SourceUnit = item.Unit;
                function.SourceId = item.Id;
                function.NormalisedUnit = "N/A";
                function.Id = entityCount;

                _entity.Add(function);
            }
        }
        log.Log(string.Format("{0} Function Definitions added", functionCount));
        return true;
    }


    private bool CreateTriggerEntities(string oldName, int count)
    {
        int triggerCount = 0;
        foreach (TriggerDefinitions1 item in _triggers)
        {
            count++;
            if (item.Name != oldName)
            {
                triggerCount++;
                entityCount++;
                Entity trigger = new Entity();
                trigger.Name = item.Name;
                trigger.Type = "TRIGGER";
                //trigger.SourceUnit = item.Unit;
                trigger.SourceId = item.Id;
                trigger.NormalisedUnit = "N/A";
                trigger.Id = entityCount;

                _entity.Add(trigger);
            }
        }
        log.Log(string.Format("{0} Trigger Definitions added", triggerCount));
        return true;
    }

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
    private void ClearTables(ConsoleLog log)
    {
        Utility bla = new Utility();

        bla.ClearTable("SQL.PackageDefinitions");
        bla.ClearTable("SQL.TableDefinitions");
        bla.ClearTable("SQL.TableForeignConstraints");
        bla.ClearTable("SQL.FunctionDefinitions");
        bla.ClearTable("Admin.Buckets");
        bla.ClearTable("Admin.Entities");
        bla.ClearTable("Admin.EntityRelationships");
        bla.ClearTable("Admin.Interfaces");
        bla.ClearTable("Admin.InternalInterfaces");
    }
    }
    #endregion

  }

