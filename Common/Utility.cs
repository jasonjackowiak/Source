using Project1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Utility
    {

        public void ClearTable(string table)
        {
            FAASModel _context = new FAASModel();
            ConsoleLog log = new ConsoleLog();

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
}
