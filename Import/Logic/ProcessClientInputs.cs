using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Logic
{
    class ProcessClientInputs
    {
        private ConsoleLog log = new ConsoleLog();

        //Create client definition from EF object here (or even from console part)

        public void ProcessInputs(ConsoleLog importLog)
        {
            log = importLog;

            //Assign client details to client object, then persist to database
        }

    }
}
