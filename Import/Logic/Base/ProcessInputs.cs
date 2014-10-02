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
    public class ProcessInputs
    {
        private ConsoleLog log;
        NameValueCollection appSettings = ConfigurationManager.AppSettings;
        private List<Entity> _entity = new List<Entity>();
        private List<Task> _tasks = new List<Task>();

        public void BeginProcessInputs(ConsoleLog importLog)
        {
            log = importLog;
            ClearTables(log);

            //Create list of eneities and persist to model
            PopulateEntities();
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


}

