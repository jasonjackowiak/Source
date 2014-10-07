using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project1;

namespace Import.Logic
{
    class ProcessRegistrationInput : ProcessInput
    {

        private ConsoleLog log = new ConsoleLog();

        //Create client definition from EF object here (or even from console part)

        public void ProcessInput(ConsoleLog importLog)
        {
            log = importLog;

            ConfigurationHandler config = ConfigurationHandler.GetConfigurationHandlerInstance;

            AddCustomer(config, log);



        }

        private static void AddCustomer(ConfigurationHandler config, ConsoleLog log)
        {
            Customer customer = new Customer();
            customer.Name = config.ClientName;
        }

        private static void AddProject(ConfigurationHandler config, ConsoleLog log)
        {
            Project project = new Project();
            project.Name = "Test Project";
        }


        private static Customer GetCustomer(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();
            Customer customer = new Customer();

            log.Log("Please select a customer:");
            //User Inputs here
            foreach (Customer c in _context.Customers)
            {
                log.Log(string.Format("{0} - {1}", c.Id, c.Name));
            }

            string line = System.Console.ReadLine();
            line = line.Trim().ToUpper();
            int input;
            if (int.TryParse(line, out input))
            {

                try
                {
                    var result = (from x in _context.Customers
                                  where x.Id == input
                                  select x).FirstOrDefault();

                    customer = result;
                }
                catch (Exception e)
                {
                    log.Log(e.Message);
                }
            }
            return customer;
        }


        //Assign client details to client object, then persist to database


    }
}
