using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class Utility
    {

        public void Dummy()
        {

        }

        public void ClearTable(string table)
        {
            HousingSAModel _context = new HousingSAModel();

            try
            {
                _context.Database.ExecuteSqlCommand(string.Format("truncate table {0}", table));
                _context.SaveChanges();
                //log.Log(string.Format("{0} table cleared", table));
            }

            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error clearing {0} table in DB: {1}", table, e.Message));
            }
        }

    }
}
