using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.IO;
using System.Threading.Tasks;

namespace Visualisation
{
    public class BuildGraph
    {
        #region vars

        #endregion

        #region run graph

        public BuildGraph()
        {
        }

        public void StartGraph(ConsoleLog log, string[] input)
        {
            log.Log("************ VISUALISATION *************");
            RunGraphs(log, input);
            log.Log("******* VISUALISATION COMPLETE *********");
            log.EndLog();
        }

        private void RunGraphs(ConsoleLog log, string[] input)
       {
           List<Task> _tasks = new List<Task>();
           bool entities = false;
           bool interfaces = false;

           foreach (string s in input)
           {

               if (s.Equals("ENTITY"))
               {
                   entities = true;
               }
               if (s.Equals("INTERFACE"))
               {
                   interfaces = true;
               }
               if (s.Equals("ALL"))
               {
                   interfaces = true;
                   entities = true;
               }
           }

           var f = Task.Factory;

           if (entities)
           {
               //log.Log("Build Entity graph - start");
               BuildEntityGraph entityGraph = new BuildEntityGraph();
               var buildEntityGraph = f.StartNew(() => entityGraph.StartGraph(log));
               _tasks.Add(buildEntityGraph);
           }
           if (interfaces)
           {
               //log.Log("Build Interface graph - start");
               BuildInterfaceGraph interfaceGraph = new BuildInterfaceGraph();
               var buildInterfaceGraph = f.StartNew(() => interfaceGraph.StartGraph(log));
               _tasks.Add(buildInterfaceGraph);
           }

           foreach (Task task in _tasks)
           {
               Task.WaitAll(task);
           }
           log.Log(string.Format("Graph building complete - {0} graphs built", _tasks.Count));
       }
        #endregion
    }



}

