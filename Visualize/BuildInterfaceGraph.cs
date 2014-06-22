using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Common;
using Project1;
using System.Collections.Specialized;

namespace Visualisation
{
    class BuildInterfaceGraph
    {
        #region vars
        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        private List<Entity> _entities = new List<Entity>();
        private List<EntityRelationship> _entityRels = new List<EntityRelationship>();

        //active lists (these are per graph)
        private List<Bucket> _interfaceBuckets = new List<Bucket>();
        private List<Node> _interfaceNodes = new List<Node>();
        private List<Link> _interfaceLinks = new List<Link>();
        private List<Link> _interfaceTreeLinks = new List<Link>();
        private List<Link> _interfaceNonTreeLinks = new List<Link>();
        private List<string> _interfaceLabels = new List<string>();
        int nodeCount;
        int linkCount;
        #endregion

        #region interface graph
        public BuildInterfaceGraph()
        {
        }

        public void StartGraph(ConsoleLog log)
        {
            //instatiate and populate global variables
            nodeCount = 0;
            linkCount = 0;
            FAASModel _context = new FAASModel();
            _entities = _context.Entities.ToList();
            _entityRels = _context.EntityRelationships.ToList();
            _interfaceBuckets = _context.Buckets.ToList();

            //Call main methods
            log.Log("Building interface level graph - start");
            AddArtificialNodesLinks();
            AddInterfaceLinks();
            log.Log("Building interface level graph - complete");
            PrintGraph(log);
            }

        #region interface graph builder
        private void AddInterfaceLinks()
        {
            FAASModel _context = new FAASModel();

            var f = (from r in _context.EntityRelationships
                     select new
                     {
                         RelationshipId = r.Id,
                         TargetEntityId = r.CalledEntityId,
                         SourceEntityId = r.CallingEntityId,
                         TargetUnit = (from t in _context.Entities
                                       where t.Id == r.CalledEntityId
                                       select new { t.NormalisedUnit }).FirstOrDefault(),
                         SourceUnit = (from t in _context.Entities
                                       where t.Id == r.CallingEntityId
                                       select new { t.NormalisedUnit }).FirstOrDefault()
                     });

            foreach (var a in f)
            {
                if (a.SourceUnit.NormalisedUnit != a.TargetUnit.NormalisedUnit)
                {
                    var ent1 = (from m in _entities
                                where m.Id == a.SourceEntityId
                                select m).FirstOrDefault();
                    Entity x = (Entity)ent1;
                    

                    var ent2 = (from m in _entities
                                where m.Id == a.TargetEntityId
                                select m).FirstOrDefault();
                    Entity xx = (Entity)ent2;

                    if (!x.NormalisedUnit.Equals(xx.NormalisedUnit))
                    {
                        Node n = new Node();
                        n.name = x.Name;
                        n.unit = x.NormalisedUnit;
                        n.type = x.Type;
                        //testing - Add only target entities (interface). All source links will come from bucket
                        Node bn = GetBucketNode(n);
                        Node nn = AddNode(xx);

                        //add check to see: does reverse (target, source) relationship exist?
                        var loopbackExist = (from m in _entityRels
                                             where m.CalledEntityId == a.SourceEntityId
                                             && m.CallingEntityId == a.TargetEntityId
                                             select m);

                        //ignore loopback calls (serves no purpose on graph, and will malform data
                        if (!loopbackExist.Any())
                        {
                            AddLink(bn, nn);
                        }
                    }
                }
            }
        }

        private void AddArtificialNodesLinks()
        {
            //system
            Entity system = new Entity();
            system.Name = "SYSTEM";
            system.Type = "SYSTEM";
            system.NormalisedUnit = "GENERIC";
            Node s = AddNode(system);

            //buckets
            foreach (Bucket b in _interfaceBuckets)
            {
                    Entity bucket = new Entity();
                    bucket.Name = b.Name;
                    bucket.NormalisedUnit = b.Unit;
                    bucket.Type = "BUCKET";
                    Node n = AddNode(bucket);
                    AddLink(s, n);
            }
        }
        #endregion

        #region Links
        private Link AddLink(Node source, Node target)
        {
                ////write processing output to same line
                //Console.Write("Building Link from {0} to {1}            \r", source.name, target.name);

            Link link = new Link();
            link.Source = source.id;
            link.Target = target.id;

            //Add system to bucket links
            if (source.id.Equals(0))
            {
                link.Id = linkCount;
                _interfaceLinks.Add(link);
                _interfaceTreeLinks.Add(link);
                linkCount++;
            }

            //add other entity links
            else
            {
                //if target does not already exist
                var targetExists = (from n in _interfaceTreeLinks
                                    where n.Target.Equals(link.Target)
                                    select n);

                //Add to list as tree 
                if (!targetExists.Any())
                {
                    AddInterfaceEntityLink(target, link);
                }

                //if source does not already exist as a target
                var sourceExists = (from n in _interfaceTreeLinks
                                    where n.Target.Equals(link.Source)
                                    select n);

                //Add to list as tree 
                if (!sourceExists.Any())
                {
                    AddInterfaceEntityLink(source, link);
                }

               //If not tree link, add non-tree link
                else
                {
                    var linkExists = (from n in _interfaceLinks
                                      where n.Source.Equals(link.Source)
                                      && n.Target.Equals(link.Target)
                                      select n);
                    if (!linkExists.Any())
                    {
                        link.Id = linkCount;
                        _interfaceLinks.Add(link);
                        _interfaceNonTreeLinks.Add(link);
                        linkCount++;
                    }
                }
            }
            return link;
        }

        private void AddInterfaceEntityLink(Node node, Link link)
        {
            //Add entity to entity link as non-tree
            link.Id = linkCount;
            _interfaceLinks.Add(link);
            _interfaceNonTreeLinks.Add(link);
            linkCount++;

            //write processing output to same line
            Console.Write("Building Link from {0} to {1}            \r", GetBucketNode(node).name, node.name);

            //Add bucket to entity link for node
            Link unitLink = new Link();
            unitLink.Id = linkCount;
            unitLink.Source = GetBucketNode(node).id;
            unitLink.Target = node.id;
            _interfaceTreeLinks.Add(unitLink);
            _interfaceLinks.Add(unitLink);
            linkCount++;

        }
        #endregion

        #region Nodes
        private Node AddNode(Entity node)
        {
            Node newNode = new Node();
            newNode.name = node.Name;
            newNode.unit = node.NormalisedUnit;
            newNode.type = node.Type;

            var nodeExists = (from n in _interfaceNodes
                              where n.name.Equals(newNode.name)
                              && n.type.Equals(newNode.type)
                              select n);

            //Add to list only if does not already exist
            if (!nodeExists.Any())
            {
                newNode.id = nodeCount;
                _interfaceNodes.Add(newNode);
                nodeCount++;
                return newNode;
            }
            else if (nodeExists.Any())
            {
                var getNode = (from n in _interfaceNodes
                               where n.name.Equals(newNode.name)
                               && n.type.Equals(newNode.type)
                               select n).FirstOrDefault();
                Node existingNode = (Node)getNode;
                //_interfaceNodes.Add(existingNode);
                return existingNode;
            }
            else
            {
                //satisfy return paths - this should never be reached
                return newNode;
            }
        }

        public Node GetBucketNode(Node node)
        {
            Node thing = new Node();

            var bucket = (from b in _interfaceNodes
                          where b.unit.Equals(node.unit)
                          && b.type.ToString().Equals("BUCKET")
                          select b).FirstOrDefault();

                return (Node)bucket;
        }
        #endregion

        #endregion

        #region utilities
        private string FormatLabel(string utext)
        {
            string ftext = utext;
            ftext = ftext.Replace(" ", "");
            ftext = ftext.Replace("-", "");
            ftext = ftext.Replace("#", "123");
            ftext = ftext.Replace("$", "456");
            ftext = ftext.Replace("@", "789");

            return ftext;
        }

        public void PrintGraph(ConsoleLog log)
        {
            log.Log("Saving HSA Interface Links graph file - start");
            //start the graph file
            TextWriter wgraph = new StreamWriter(GetFromConfig("InterfaceGraphFilePath"));

            //structural data (links)
            wgraph.WriteLine(string.Format("Graph \r\n {{ \r\n ### metadata ### \r\n @name=\"HSA\"; \r\n @description=\"A Housing SA System Graph\"; \r\n @numNodes={0}; \r\n @numLinks={1}; \r\n @numPaths=0; \r\n @numPathLinks=0; \r\n \r\n ### structural data ### \r\n @links=[ \r\n", _interfaceNodes.Count, _interfaceLinks.Count));
            foreach (Link l in _interfaceLinks)
            {
                string line;

                if (!l.Id.Equals(_interfaceLinks.Count - 1))
                {
                    line = string.Format("{{ {0}; {1}; }}, ", l.Source, l.Target);
                    wgraph.WriteLine(line);
                }
                else if (l.Id.Equals(_interfaceLinks.Count - 1))
                {
                    line = string.Format("{{ {0}; {1}; }} ", l.Source, l.Target);
                    wgraph.WriteLine(line);
                }
                
            }

            //start the attribute data (tree links)
            wgraph.WriteLine(string.Format("\r\n ]; \r\n @paths=; \r\n ### attribute data ### \r\n @enumerations=; \r\n @attributeDefinitions=[ \r\n {{" + "@name=$root; \r\n @type=bool; \r\n @default=|| false ||; \r\n @nodeValues=[ {{  @id=0; @value=T; }} ]; \r\n @linkValues=; \r\n @pathValues=; \r\n }}, \r\n {{ \r\n @name=$tree_link; \r\n @type=bool; \r\n @default=|| false ||; \r\n @nodeValues=; \r\n @linkValues=["));
            foreach (Link tl in _interfaceTreeLinks)
            {
                int curElement = _interfaceTreeLinks.IndexOf(tl);

                if (!curElement.Equals(_interfaceTreeLinks.Count - 1))
                {
                    string line = string.Format("{{ {0}; T; }}, ", tl.Id);
                    wgraph.WriteLine(line);
                }
                else if (curElement.Equals(_interfaceTreeLinks.Count - 1))
                {
                    string line = string.Format("{{ {0}; T; }} ", tl.Id);
                    wgraph.WriteLine(line);
                }
            }

            // (labels)
            wgraph.WriteLine(string.Format("]; \r\n @pathValues=; \r\n }}, \r\n {{ \r\n @name=$labels; \r\n @type=string; \r\n @default=; \r\n @nodeValues=[ "));
            foreach (Node n in _interfaceNodes)
            {
                if (!n.id.Equals(_interfaceNodes.Count - 1))
                {
                    string line = string.Format("{{ {0}; \"{1}\"; }}, ", n.id, FormatLabel(n.name));
                    wgraph.WriteLine(line);
                }
                else if (n.id.Equals(_interfaceNodes.Count - 1))
                {
                    string line = string.Format("{{ {0}; \"{1}\"; }} ", n.id, FormatLabel(n.name));
                    wgraph.WriteLine(line);
                }
            }

            //Closing line
            wgraph.WriteLine(string.Format("];\r\n @linkValues=; \r\n @pathValues=; \r\n }} \r\n ]; \r\n @qualifiers=[ \r\n {{ \r\n @type=$spanning_tree; \r\n @name=$sample_spanning_tree; \r\n @description=; \r\n @attributes=[ \r\n {{ @attribute=0; @alias=$root; }}, \r\n {{ @attribute=1; @alias=$tree_link; }} \r\n ]; \r\n }} \r\n ]; \r\n ### visualization hints ### \r\n @filters=; \r\n @selectors=; \r\n @displays=; \r\n @presentations=; \r\n ### interface hints ### \r\n @presentationMenus=; \r\n @displayMenus=; \r\n @selectorMenus=; \r\n @filterMenus=; \r\n @attributeMenus=; \r\n }}"));
            wgraph.Close();
            log.Log("Printing graph file - complete");
        }
        #endregion

        private string GetFromConfig(string p)
        {
            return appSettings.Get(p);
        }
    }
}

