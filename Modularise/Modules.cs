using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Project1;
using Common;
using System.Collections.Specialized;
using System.Configuration;

namespace Modularise
{
    public class Modules
    {
        #region vars
        private ConsoleLog log;
        private NameValueCollection appSettings = ConfigurationManager.AppSettings;

        private List<EntityMap> _mappedEntities = new List<EntityMap>();
        private List<Bucket> _buckets = new List<Bucket>();
        private List<UnitMap> _mappedUnits = new List<UnitMap>();
        private List<Entity> _entities = new List<Entity>();
        private List<Interface> _interfaces = new List<Interface>();
        private List<string> _types = new List<string>();
        #endregion

        public Modules()
        {
        }

        public void ModulariseEntities(ConsoleLog _log)
        {
            log = _log;
            ProcessUnitMap(log);
            BuildExternalInterfaces(log);
        }

        #region utilities
        private void ClearTables()
        {
            Utility bla = new Utility();

            bla.ClearTable("Admin.Bucket");
            bla.ClearTable("Admin.Interface");
            bla.ClearTable("Admin.InternalInterface");
            bla.ClearTable("Admin.EntityResidence");
            bla.ClearTable("Admin.InterfaceReporting");
            bla.ClearTable("Admin.BucketReporting");
            bla.ClearTable("Admin.BucketConnection");
        }
        #endregion

        #region buckets
        private void ProcessUnitMap(ConsoleLog log)
        {
            ClearTables();

            log.Log("Assigning entities to buckets - start");

            ReadExcel();
            CreateManualBuckets();
            UpdateMappedEntities();
            UpdateUnmappedEntities();
            PopulateBuckets();

            log.Log("Assigning entities to buckets - complete");
        }

        #region Excel
        private void ReadExcel()
        {
            ReadExcelRulesBuckets();
            ReadExcelTransactions();
        }

        private void ReadExcelRulesBuckets()
{
    string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetFromConfig("RuleBucketMapFilePath"));
            
            log.Log(string.Format("Opening {0} Excel file", file));
    try
    {
            if (File.Exists(file))
            {
                Application xlApp = new Application();
                Workbook xlWorkbook = xlApp.Workbooks.Open(file);
                CreateBucketsFromMap(xlWorkbook);
                CreateRuleUnitMappedEntities(xlWorkbook);
                xlWorkbook.Close();
                xlApp.Quit();
            }
    }
    catch (Exception e)
    {
        log.Log(string.Format("Error processing {0} Excel file: {1}", file, e.Message));
    }
}

        private void ReadExcelTransactions()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetFromConfig("TransactionMapFilePath"));
            log.Log(string.Format("Opening {0} Excel file", file));
            try
            {
                if (File.Exists(file))
                    {
                         Application xlApp = new Application();
                         Workbook xlWorkbook = xlApp.Workbooks.Open(file);
                         //CreateTranUnitMappedEntities(xlWorkbook);
                         xlWorkbook.Close();
                         xlApp.Quit();
                    }
            }
            catch (Exception e)
                {
                    log.Log(string.Format("Error processing {0} Excel file: {1}", file, e.Message));
                }
        }

        private void CreateBucketsFromMap(Workbook xlWorkbook)
        {
            _Worksheet bucketSheet = xlWorkbook.Sheets[3];
            Range xlRange = bucketSheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            log.Log("Creating normalised bucket list - start");

            for (int i = 2; i != rowCount + 1; i++)
            {
                int count = 1;
                string name = bucketSheet.Cells[i, count].Value2.ToString();
                count++;
                string unit = bucketSheet.Cells[i, count].Value2.ToString();
                //write processing output to same line
                Console.Write("Building bucket {0}             \r", name);

                CreateBucket(unit, name, "Application");
            }

            log.Log("Creating normalised bucket list - complete");
        }

        private void CreateTranUnitMappedEntities(Workbook xlWorkbook)
        {
            _Worksheet tranToUnitMappingSheet = xlWorkbook.Sheets[1];
            Range xlRange = tranToUnitMappingSheet.UsedRange;
            string type = "TRAN";
            _types.Add(type);

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            log.Log(string.Format("Normalise {0} units - start", type));

            //start at row 2, then grab details from each column
            for (int i = 2; i != rowCount + 1; i++)
            {
                int count = 3;
                string name = tranToUnitMappingSheet.Cells[i, count].Value2.ToString();
                count++;
                count++;
                string sourceUnit = tranToUnitMappingSheet.Cells[i, count].Value2.ToString();
                count++;
                string normalisedUnit = tranToUnitMappingSheet.Cells[i, count].Value2.ToString();

                //write processing output to same line
                Console.Write("Mapping transaction {0} from {1} to {2}             \r", name, sourceUnit, normalisedUnit);

                EntityMap mappedEntity = new EntityMap(name, type, sourceUnit, normalisedUnit);
                _mappedEntities.Add(mappedEntity);
            }
            log.Log(string.Format("Normalise {0} units - complete", type));
        }

        private void CreateRuleUnitMappedEntities(Workbook xlWorkbook)
        {
            _Worksheet ruleToUnitMappingSheet = xlWorkbook.Sheets[1];
            Range xlRange = ruleToUnitMappingSheet.UsedRange;
            string type = "RULE";
            _types.Add(type);

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            log.Log(string.Format("Normalise {0} units - start", type));

            for (int i = 2; i != rowCount + 1; i++)
            {
                int count = 1;
                string name = ruleToUnitMappingSheet.Cells[i, count].Value2.ToString();
                count++;
                string sourceUnit = ruleToUnitMappingSheet.Cells[i, count].Value2.ToString();
                count++;
                string normalisedUnit = ruleToUnitMappingSheet.Cells[i, count].Value2.ToString();
                EntityMap mappedEntity = new EntityMap(name, type, sourceUnit, normalisedUnit);
                //write processing output to same line
                //write processing output to same line
                Console.Write("Mapping rule {0} from {1} to {2}             \r", name, sourceUnit, normalisedUnit);
                _mappedEntities.Add(mappedEntity);
            }
            log.Log(string.Format("Normalise {0} units - complete", type));
        }
        #endregion

        private void CreateManualBuckets()
        {
            //Manual bucket creation to deal with any leftover/system entities after HSA mapping
            try
            {
                log.Log("Creating manual bucket list - start");
                CreateBucket("LONELY", "Unallocated", "Application");
                log.Log("Creating manual bucket list - complete");
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error creating manual bucket list: {0}", e.Message));
            }
        }

        private void UpdateMappedEntities()
        {
            FAASModel _context = new FAASModel();
            log.Log("Persist mapped entity units to DB - start");
            _entities = _context.Entities.ToList();

            foreach (EntityMap mappedEntity in _mappedEntities)
            {

                foreach (Entity entity in _entities)
                {
                    foreach (string type in _types)
                    {
                        if (mappedEntity.Name.Equals(entity.Name) && mappedEntity.Type.Equals(type))
                        {
                            entity.NormalisedUnit = mappedEntity.NormalisedUnit;
                        }
                    }
                }
            }
            _context.SaveChanges();
            log.Log("Persist mapped entity units to DB - complete");

            var count = (from x in _context.Entities
                         where x.NormalisedUnit != "N/A"
                         && x.NormalisedUnit != "LONELY"
                         select x).Count();

            log.Log(string.Format("{0} entities Normalised", count));
        }

        private void UpdateUnmappedEntities()
        {
            FAASModel _context = new FAASModel();
            log.Log("Persist unmapped entity units to DB - start");
            _entities = _context.Entities.ToList();

            foreach (Entity entity in _entities)
            {
                if (entity.NormalisedUnit.ToString().Equals("N/A"))
                {
                    entity.NormalisedUnit = "LONELY";
                }
            }

            //need to persist to DB
            _context.SaveChanges();
            log.Log("Persist unmapped entity units to DB - complete");

            var count = (from x in _context.Entities
                         where x.NormalisedUnit == "LONELY"
                         select x).Count();

            log.Log(string.Format("{0} entities Unallocated", count));

        }

        private void CreateBucket(string unit, string name, string type)
        {
            Bucket bucket = new Bucket();
            bucket.Name = name;
            bucket.Unit = unit;
            bucket.Type = type;
            _buckets.Add(bucket);
        }

        private void PopulateBuckets()
        {
            FAASModel _context = new FAASModel();

            log.Log("Persits Buckets to DB - start");
            try
            {
                foreach (Bucket item in _buckets)
                {
                    _context.Buckets.Add(item);

                }
                _context.SaveChanges();
                log.Log("Persist Buckets to DB - complete");
                log.Log(string.Format("{0} buckets created", _buckets.Count()));
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Buckets to DB: {0}: ", e.Message));
            }

        }
        #endregion

        #region Interfaces
        public void BuildExternalInterfaces(ConsoleLog log)
        {
            log.Log("Building Interfaces - start");

            BuildAllInterfaces();
            PopulateInterfaces();

            log.Log("Building Interfaces - complete");
            log.EndLog();
        }

        private void BuildAllInterfaces()
        {
            FAASModel _context = new FAASModel();
            log.Log("Build bucket to bucket Interfaces - start");

            //get target id & unit for each relationship
            var f = (from r in _context.EntityRelationships
                     select new
                     {
                         RelationshipId = r.Id,
                         TargetEntityId = r.CalledEntityId,
                         SourceEntityId = r.CallingEntityId,
                         TargetUnit = (from t in _context.Entities
                                where t.Id == r.CalledEntityId
                                select new 
                                { t.NormalisedUnit }).FirstOrDefault(),
                        SourceUnit = (from t in _context.Entities
                                where t.Id == r.CallingEntityId
                                select new 
                                { t.NormalisedUnit }).FirstOrDefault()
                    });

            foreach (var a in f)
            {
                if (a.SourceUnit.NormalisedUnit != a.TargetUnit.NormalisedUnit)
                {
                    bool exists = CheckInterface(a.TargetEntityId, a.TargetUnit.NormalisedUnit, a.SourceEntityId);
                    if (!exists)
                    {
                        //create new interface
                        Interface newInterface = new Interface();
                        newInterface.TargetEntityId = a.TargetEntityId;
                        newInterface.TargetUnit = a.TargetUnit.NormalisedUnit.ToString();
                        newInterface.EntityRelationshipIds = a.RelationshipId.ToString();
                        //write processing output to same line
                        Console.Write("Creating interface for {0}          \r", newInterface.TargetUnit);
                        _interfaces.Add(newInterface);
                    }
                }
            }
            log.Log("Build bucket to bucket Interfaces - complete");
        }

        /// <summary>
        /// Check for existing interface - if exists add current relationship and return true
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="targetUnit"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        private bool CheckInterface(int targetId, object targetUnit, int sourceId)
        {
            FAASModel _context = new FAASModel();
            bool exists = false;

            foreach (Interface item in _interfaces)
            {
                if (item.TargetEntityId.Equals(targetId) && item.TargetUnit.Equals(targetUnit))
                {
                    //Add relationship to interface - hoping this doesnt malform the list of interface objects...
                    item.EntityRelationshipIds = item.EntityRelationshipIds + "," + sourceId.ToString();
                    exists = true;
                }
            }
            return exists;
        }

        private void PopulateInterfaces()
        {
            FAASModel _context = new FAASModel();

            log.Log("Persits Interfaces to DB - start");
            try
            {
                foreach (Interface item in _interfaces)
                {
                    _context.Interfaces.Add(item);
                }
                _context.SaveChanges();
                log.Log("Persist Interfaces to DB - complete");
                log.Log(string.Format("{0} interfaces created", _interfaces.Count()));
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Intefaces to DB: {0}: ", e.Message));
            }

        }

        private string GetFromConfig(string p)
        {
            return appSettings.Get(p);
        }
        #endregion
    }    

}
