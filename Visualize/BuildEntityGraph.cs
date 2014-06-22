using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.IO;
using System.Configuration;
using Project1;
using Common;
using System.Collections.Specialized;

namespace Visualisation
{
    class BuildEntityGraph
    {
        #region vars

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        //reference lists
        private List<Entity> _entities = new List<Entity>();
        private List<EntityRelationship> _entityRels = new List<EntityRelationship>();
        
        //active lists (these are per graph)
        private List<Bucket> _entityBuckets = new List<Bucket>();
        private List<Node> _entityBucketNodes = new List<Node>();
        private List<Node> _entityNodes = new List<Node>();
        private List<Link> _entityLinks = new List<Link>();
        private List<Link> _entityTreeLinks = new List<Link>();
        private List<Link> _entityNonTreeLinks = new List<Link>();
        private List<string> _entityLabels = new List<string>();
        private List<Link> _linksToCheck = new List<Link>();
        int entityNodeCount;
        int entityLinkCount;
        #endregion

        #region entity graph

        public BuildEntityGraph()
        {
        }

        public void StartGraph(ConsoleLog log)
        {
            //instatiate and populate global variables
            entityNodeCount = 0;
            entityLinkCount = 0;
            FAASModel _context = new FAASModel();
            _entities = _context.Entities.ToList();
            _entityRels = _context.EntityRelationships.ToList();
            _entityBuckets = _context.Buckets.ToList();

            //call main methods
            log.Log("Building entity level graph - start");
            AddArtificialNodesLinks();
            AddInternalEntityLinks();
            //call clean to add all current homeless nodes to their respective buckets - fix bug where they were being allocated to external buckets
            CleanLinks(log);
            AddExternalEntityLinks(log);
            CleanLinks(log);
            log.Log("Building entity level graph - complete");
            PrintGraph(log);
            }

        #region entity graph builder
        private void AddInternalEntityLinks()
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
                //add check to see: does reverse (target, source) relationship exist?
                var loopbackExist = (from m in _entityRels
                                     where m.CalledEntityId == a.SourceEntityId
                                     && m.CallingEntityId == a.TargetEntityId
                                     select m);

                //build internal bucket links and nodes
                if (a.SourceUnit.NormalisedUnit == a.TargetUnit.NormalisedUnit)
                {
                    string unit = a.SourceUnit.NormalisedUnit.ToString();

                    if (a.SourceEntityId != a.TargetEntityId)
                    {

                        var ent1 = (from m in _entities
                                    where m.Id == a.SourceEntityId
                                    select m).FirstOrDefault();
                        Entity x = (Entity)ent1;
                        Node source = AddNode(x);

                        var ent2 = (from m in _entities
                                    where m.Id == a.TargetEntityId
                                    select m).FirstOrDefault();
                        Entity xx = (Entity)ent2;
                        Node target = AddNode(xx);

                        //deal with links that exist also in reverse (a calls b, b calls a)
                        if (loopbackExist.Any())
                        {
                            //check if either link exists as tree
                            var link = (from m in _entityTreeLinks
                                        where m.Source == a.SourceEntityId
                                        && m.Target == a.TargetEntityId
                                        select m);
                            var loopLink = (from m in _entityTreeLinks
                                            where m.Target == a.SourceEntityId
                                            && m.Source == a.TargetEntityId
                                            select m);

                            var targetExists = (from n in _entityTreeLinks
                                                where n.Target.Equals(a.TargetEntityId)
                                                select n);
                            //if either exist as tree, add non-tree link
                            if (link.Any() || loopLink.Any())
                            {
                                //add this to a list of links to check later on - may need to add orphans to buckets
                                AddNonTreeLink(source, target);
                            }
                            else
                            {
                                _linksToCheck.Add(AddTreeLink(source, target));
                            }

                        }
                        else if (!loopbackExist.Any())
                        {
                            //does a relative call my target?
                            if (CheckForParent(source, target))
                            {
                                AddNonTreeLink(source, target);
                            }
                            else
                            {
                                AddLinkCheck(source, target);
                                bool exists = CheckParent(a.TargetEntityId, a.TargetUnit, a.SourceEntityId);
                                if (!exists)
                                {
                                    AddLink(GetBucketNode(source), source);
                                }
                            }
                        }

                    }
                }
            }
        }

        private void AddExternalEntityLinks(ConsoleLog log)
        {
            log.Log("Building entity level graph external links - start");

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
            //46364 error

            foreach (var a in f)
            {
                //add check to see: does reverse (target, source) relationship exist?
                var loopbackExist = (from m in _entityRels
                                     where m.CalledEntityId == a.SourceEntityId
                                     && m.CallingEntityId == a.TargetEntityId
                                     select m);

                //build external (bucket to bucket) links 
                if (a.SourceUnit.NormalisedUnit != a.TargetUnit.NormalisedUnit)
                {
                    var ent1 = (from m in _entities
                                where m.Id == a.SourceEntityId
                                select m).FirstOrDefault();
                    Entity x = (Entity)ent1;
                    //get node or add if doesnt exist
                    Node source = GetNode(ent1);

                    var ent2 = (from m in _entities
                                where m.Id == a.TargetEntityId
                                select m).FirstOrDefault();
                    Entity xx = (Entity)ent2;
                    //get node or add if doesnt exist
                    Node target = GetNode(ent2);


                        AddLink(source, target);
                }
            }
            log.Log("Building entity level graph external links - complete");
        }

        private bool CheckParent(int targetId, object targetUnit, int sourceId)
        {
            FAASModel _context = new FAASModel();
            bool exists = false;

            //check if current source is called anywhere (is it an orphan?)
            foreach (EntityRelationship item in _entityRels)
            {
                if (item.CalledEntityId.Equals(sourceId))
                {
                    exists = true;
                }
            }
            return exists;
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
            foreach (Bucket b in _entityBuckets)
            {
                    Entity bucket = new Entity();
                    bucket.Name = b.Name;
                    bucket.NormalisedUnit = b.Unit;
                    bucket.Type = "BUCKET";
                    Node m = AddTempBucketNode(bucket);
                    Node n = AddNode(bucket);
                    AddLink(s, n);
            }

            //trans (links are artificial)
            foreach (Entity node in _entities)
            {
                foreach (Node bucket in _entityBucketNodes)
                {
                        if (node.NormalisedUnit.Equals(bucket.unit) && node.Type.Equals("TRAN"))
                        {
                            Node tran = AddNode(node);
                            AddLink(bucket, tran);
                        }
                }
            }
        }
        #endregion

        #region links
        private Link AddLink(Node source, Node target)
        {
            //this is to fix a multi-line console display bug
            var bucket = (from b in _entityBucketNodes
                          where b.name == source.name
                          select b);
            if (!bucket.Any())
            {
                //write processing output to same line
                Console.Write("Building Link from {0} to {1}            \r", source.name, target.name);
            }

            Link link = new Link();
            link.Source = source.id;
            link.Target = target.id;

            //Add system to bucket links
            if (source.id.Equals(0))
            {
                link.Id = entityLinkCount;
                _entityLinks.Add(link);
                _entityTreeLinks.Add(link);
                entityLinkCount++;
            }

            //add other entity links
            else
            {
                    //if target does not already exist
                    var targetExists = (from n in _entityTreeLinks
                                        where n.Target.Equals(link.Target)
                                        select n);

                    //Add to list as tree 
                    if (!targetExists.Any())
                    {
                        link.Id = entityLinkCount;
                        _entityLinks.Add(link);
                        _entityTreeLinks.Add(link);
                        entityLinkCount++;
                    }
                
                //If not tree link, add non-tree link
                else
                {
                    var linkExists = (from n in _entityLinks
                                      where n.Source.Equals(link.Source)
                                      && n.Target.Equals(link.Target)
                                      select n);
                    if (!linkExists.Any())
                    {
                        link.Id = entityLinkCount; ;
                        _entityLinks.Add(link);
                        _entityNonTreeLinks.Add(link);
                        entityLinkCount++;
                    }
                }
            }
            return link;
        }

        private void AddLinkCheck(Node source, Node target)
        {

            if (CheckForParent(source, target))
            {
                AddNonTreeLink(source, target);
            }
            else
            {
                AddLink(source, target);
            }
            
        }

        private bool CheckForParent(Node source, Node target)
        {
            bool b = false;

            var paexist = (from p in _entityTreeLinks
                           where p.Target == source.id
                           select p);
            //yes
            if (paexist.Any())
            {
                //get their link
                var pa = (from p in _entityTreeLinks
                          where p.Target == source.id
                          select p).FirstOrDefault();
                //if his dad is my target NO TREE

                if (pa.Source.Equals(target.id))
                {
                    b = true;
                }
                else
                {
                    //Call again, check next parent
                    var parentSource = (from e in _entityNodes
                                        where e.id == pa.Source
                                        select e).FirstOrDefault();
                    return CheckForParent(parentSource, target);
                }
                return b;
            }
            return b;
        }

        private Link AddNonTreeLink(Node source, Node target)
        {
            //write processing output to same line
            Console.Write("Building Link from {0} to {1}            \r", source.name, target.name);

            Link link = new Link();
            link.Source = source.id;
            link.Target = target.id;
            link.Id = entityLinkCount;
            _entityLinks.Add(link);
            _entityNonTreeLinks.Add(link);
            entityLinkCount++;

            return link;
        }

        private Link AddTreeLink(Node source, Node target)
        {
            //write processing output to same line
            Console.Write("Building Link from {0} to {1}            \r", source.name, target.name);

            //add a 2nd check that inverse link does not exist as tree
            var loopLink = (from m in _entityTreeLinks
                            where m.Target == source.id
                            && m.Source == target.id
                            select m);

            Link link = new Link();
            link.Source = source.id;
            link.Target = target.id;

            if (!loopLink.Any())
            {

                link.Id = entityLinkCount;
                _entityLinks.Add(link);
                _entityTreeLinks.Add(link);
                entityLinkCount++;
            }

            return link;
        }

        private void CleanLinks(ConsoleLog log)
        {
            List<Link> _homelessLinks = new List<Link>();
            List<Node> _homelessNodes = new List<Node>();

            //for the looplinks, check all tree link sources have a source
            foreach (Link checkLink in _linksToCheck)
            {
                var x = (from l in _entityTreeLinks
                         where l.Target == checkLink.Source
                         select l);

                //if tree-link source is never called as a target, it needs a home
                if (!x.Any())
                {
                    Node source = (from n in _entityNodes
                                   where n.id == checkLink.Source
                                   select n).FirstOrDefault();

                    AddLink(GetBucketNode(source), source);
                }

                //if multiple tree-links exist remove the checking one
                var r = (from l in _entityTreeLinks
                         where l.Target == checkLink.Target
                         && l.Source != checkLink.Source
                         select l);

                if (r.Any())
                {
                    _entityTreeLinks.Remove(checkLink);
                }
            }

            foreach (Link link in _entityLinks)
            {
                GetHomelessLinks(_homelessLinks, link);
            }

            //add links to owning buckets for leftover tree-link sources
            foreach (Link hlink in _homelessLinks)
            {
                var source = (from m in _entityNodes
                              where m.id == hlink.Source
                              select m).FirstOrDefault();

                AddLink(GetBucketNode(source), source);
            }

            log.Log("Building entity level graph - complete");
        }

        private void GetHomelessLinks(List<Link> _homelessLinks, Link link)
        {
            bool exists = false;
            //check if current source is called anywhere (is it an orphan?)
            foreach (Link item in _entityTreeLinks)
            {

                //for a link source, look through all other links to see if it is called (as target)
                if (item.Target.Equals(link.Source))
                {
                    exists = true;
                }
            }
            //if link source node does NOT exist as tree-link target
            if (!exists && !link.Source.Equals(0))
            {
                _homelessLinks.Add(link);
            }
        }
        #endregion

        #region nodes
        public Node GetBucketNode(Node node)
        {

            Node thing = new Node();

            foreach (Node item in _entityNodes)
            {
                if (item.unit.Equals(node.unit) && item.type.Equals("BUCKET"))
                {
                    thing = item;
                }
            }

            return thing;

        }

        private Node AddNode(Entity node)
        {
            Node newNode = new Node();
            newNode.name = node.Name;
            newNode.unit = node.NormalisedUnit;
            newNode.type = node.Type;

            var nodeExists = (from n in _entityNodes
                              where n.name.Equals(newNode.name)
                              && n.type.Equals(newNode.type)
                              select n);

            //Add to list only if does not already exist
            if (!nodeExists.Any())
            {
                newNode.id = entityNodeCount;
                _entityNodes.Add(newNode);
                entityNodeCount++;
                return newNode;
            }
            else if (nodeExists.Any())
            {
                Node existingNode = GetNode(node);
                //_entityNodes.Add(existingNode);
                return existingNode;
            }
            else
            {
                //satisfy return paths - this should never be reached
                return newNode;
            }
        }

        private Node GetNode(Entity e)
        {
            Node node;

            var getNodeTest = (from n in _entityNodes
                               where n.name.Equals(e.Name)
                               && n.type.Equals(e.Type)
                               select n);

            if (!getNodeTest.Any())
            {
                node = AddNode(e);
                return node;
            }

            else 
            {
                var getNode = (from n in _entityNodes
                               where n.name.Equals(e.Name)
                               && n.type.Equals(e.Type)
                               select n).FirstOrDefault();
                node = (Node)getNode;
                return node;
            }

        }

        private Node AddTempBucketNode(Entity node)
        {
            Node newNode = new Node();
            newNode.id = entityNodeCount;
            newNode.name = node.Name;
            newNode.unit = node.NormalisedUnit;
            newNode.type = node.Type;

            var nodeExists = (from n in _entityBucketNodes
                              where n.name.Equals(newNode.name)
                              && n.type.Equals(newNode.type)
                              select n);

            //Add to list only if does not already exist
            if (!nodeExists.Any())
            {
                _entityBucketNodes.Add(newNode);
            }

            return newNode;
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
            log.Log("Saving Entity Links graph file - start");
            //start the graph file

            TextWriter wgraph = new StreamWriter(GetFromConfig("EntityGraphFilePath"));

            //structural data (links)
            wgraph.WriteLine(string.Format("Graph \r\n {{ \r\n ### metadata ### \r\n @name=\"HSA\"; \r\n @description=\"A Housing SA System Graph\"; \r\n @numNodes={0}; \r\n @numLinks={1}; \r\n @numPaths=0; \r\n @numPathLinks=0; \r\n \r\n ### structural data ### \r\n @links=[ \r\n", _entityNodes.Count, _entityLinks.Count));
            foreach (Link l in _entityLinks)
            {
                string line;
                if (!l.Id.Equals(_entityLinks.Count - 1))
                {
                    line = string.Format("{{ {0}; {1}; }}, ", l.Source, l.Target);
                    wgraph.WriteLine(line);
                }
                else if (l.Id.Equals(_entityLinks.Count - 1))
                {
                    line = string.Format("{{ {0}; {1}; }} ", l.Source, l.Target);
                    wgraph.WriteLine(line);
                }
            }

            //start the attribute data (tree links)
            wgraph.WriteLine(string.Format("\r\n ]; \r\n @paths=; \r\n ### attribute data ### \r\n @enumerations=; \r\n @attributeDefinitions=[ \r\n {{" + "@name=$root; \r\n @type=bool; \r\n @default=|| false ||; \r\n @nodeValues=[ {{  @id=0; @value=T; }} ]; \r\n @linkValues=; \r\n @pathValues=; \r\n }}, \r\n {{ \r\n @name=$tree_link; \r\n @type=bool; \r\n @default=|| false ||; \r\n @nodeValues=; \r\n @linkValues=["));
            foreach (Link tl in _entityTreeLinks)
            {
                int curElement = _entityTreeLinks.IndexOf(tl);

                if (!curElement.Equals(_entityTreeLinks.Count - 1))
                {
                    string line = string.Format("{{ {0}; T; }}, ", tl.Id);
                    wgraph.WriteLine(line);
                }
                else if (curElement.Equals(_entityTreeLinks.Count - 1))
                {
                    string line = string.Format("{{ {0}; T; }} ", tl.Id);
                    wgraph.WriteLine(line);
                }
            }

            // (labels)
            wgraph.WriteLine(string.Format("]; \r\n @pathValues=; \r\n }}, \r\n {{ \r\n @name=$labels; \r\n @type=string; \r\n @default=; \r\n @nodeValues=[ "));
            foreach (Node n in _entityNodes)
            {
                if (!n.id.Equals(_entityNodes.Count - 1))
                {
                    string line = string.Format("{{ {0}; \"{1}\"; }}, ", n.id, FormatLabel(n.name));
                    wgraph.WriteLine(line);
                }
                else if (n.id.Equals(_entityNodes.Count - 1))
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

