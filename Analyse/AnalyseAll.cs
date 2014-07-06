using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Modularise;
using Common;
using Project1;

namespace Analyse
{
    public class AnalyseAll
    {
        #region vars
        //global
        //private ConsoleLog log = new ConsoleLog();

        private List<Entity> _entities = new List<Entity>();

        //internal interfaces
        private List<InternalInterface> _internalInterfaces = new List<InternalInterface>();

        //entity residence
        private List<EntityResidence> _entStr = new List<EntityResidence>();
        private List<Interface> _externalInterfaces = new List<Interface>();

        //interface reporting
        private List<EntityRelationship> _entityRelationships = new List<EntityRelationship>();
        private List<InterfaceReporting> _interfaceReports = new List<InterfaceReporting>();

        //bucket reporting
        private List<Bucket> _buckets = new List<Bucket>();
        private List<BucketReporting> _bucketReports = new List<BucketReporting>();

        //bucket connections
        private List<BucketConnection> _bucketConnections = new List<BucketConnection>();

        #endregion

        #region run analysis
        public AnalyseAll()
        {
        }

        public void StartAnalysis(ConsoleLog log)
        {

            log.Log("Analysis");
            log.Log("*************** ANALYSE ******************");
            ClearTables();

            BuildInternalInterfaces(log);
            PopulateInterfaces(log);

            BeginReferentialWeighting(log);
            PopulateEntityResidence(log);

            BuildInterfaceReporting(log);
            PopulateInterfaceReporting(log);

            BuildBucketReporting(log);
            PopulateBucketReporting(log);

            BuildBucketConnections(log);
            PopulateBucketConnections(log);

            log.Log("************* ANALYSE END ****************");
            log.EndLog();
        }
        #endregion

        #region internal interfaces
        private void BuildInternalInterfaces(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();
            log.Log("Build internal bucket Interfaces - start");

            try
            {
                //get target id & unit for each relationship
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
                    if (a.SourceUnit.NormalisedUnit.Equals(a.TargetUnit.NormalisedUnit))
                    {
                        bool exists = CheckInterface(a.RelationshipId, a.TargetEntityId, a.TargetUnit.NormalisedUnit, a.SourceEntityId, a.SourceUnit.NormalisedUnit);
                        if (!exists)
                        {
                            log.Log(String.Format("Creating Interface for {0}            \r", a.TargetUnit.NormalisedUnit));
                            //create new interface
                            InternalInterface newInterface = new InternalInterface();
                            newInterface.TargetEntityId = a.TargetEntityId;
                            newInterface.TargetUnit = a.TargetUnit.NormalisedUnit.ToString();
                            newInterface.EntityRelationshipIds = a.RelationshipId.ToString();
                            _internalInterfaces.Add(newInterface);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(String.Format("Error creating interface: {0}", e));
            }
            log.Log("Build internal bucket Interfaces - complete");
        }

        /// <summary>
        /// Check for existing interface (for target id) - if exists add current relationship and return true
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="targetUnit"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        private bool CheckInterface(int relId, int targetId, string targetUnit, int sourceId, string sourceUnit)
        {
            FAASModel _context = new FAASModel();
            bool exists = false;

            foreach (InternalInterface item in _internalInterfaces)
            {
                if (item.TargetEntityId.Equals(targetId) && item.TargetUnit.Equals(targetUnit) && item.TargetUnit.Equals(sourceUnit))
                {
                    //Add relationship to interface - hoping this doesnt malform the list of interface objects...
                    item.EntityRelationshipIds = item.EntityRelationshipIds + ", " + relId.ToString();
                    exists = true;
                }
            }
            return exists;
        }

        private void PopulateInterfaces(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();

            log.Log("Persist internal Interfaces to DB - start");
            try
            {
                foreach (InternalInterface item in _internalInterfaces)
                {
                    _context.InternalInterfaces.Add(item);
                }
                _context.SaveChanges();
                log.Log("Persist internal Interfaces to DB - complete");
                log.Log(string.Format("{0} internal Interfaces created", _internalInterfaces.Count()));
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting internal Intefaces to DB: {0}: ", e.Message));
            }

            

        }
        #endregion

        #region entity residence

        private void BeginReferentialWeighting(ConsoleLog log)
        {
            log.Log("Assess entity residence - start");

            FAASModel _context = new FAASModel();
            _entities = _context.Entities.ToList();
            _internalInterfaces = _context.InternalInterfaces.ToList();
            _externalInterfaces = _context.Interfaces.ToList();

            foreach (Entity entity in _entities)
            {
                //write processing output to same line
                Console.Write("Assessing {0}            \r", entity.Name);

                EntityResidence entStr = new EntityResidence();
                entStr.EntityId = entity.Id;
                entStr.InternalWeight = GetStrength(entity, "internal");
                entStr.ExternalWeight = GetStrength(entity, "external");
                entStr.ExternalSources = InterfaceSources(entity, "external").Count;
                _entStr.Add(entStr);
            }
            log.Log("Assess entity residence - start");
        }

        /// <summary>
        /// Returns number of source entities that call the supplied entity (based on internal or external interfaces)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int GetStrength(Entity entity, string type)
        {
            FAASModel _context = new FAASModel();
            List<int> nums = new List<int>();
            int value = 0;

            if (type.Equals("external"))
            {
                foreach (Interface extInterface in _externalInterfaces)
                {
                    if (extInterface.TargetEntityId.Equals(entity.Id))
                    {
                        nums = RetreiveIds(extInterface.EntityRelationshipIds.ToString());
                    }

                }
            }

            if (type.Equals("internal"))
            {
                foreach (InternalInterface intInterface in _internalInterfaces)
                {
                    if (intInterface.TargetEntityId.Equals(entity.Id))
                    {
                        nums = RetreiveIds(intInterface.EntityRelationshipIds.ToString());
                    }

                }
            }

            value = nums.Count;
            return value;
        }

        private List<int> RetreiveIds(string Ids)
        {

            //convert strings to ints
            List<int> SourceIds = new List<int>();

            string[] temp = Ids.Split(',');
            foreach (string s in temp)
            {
                int id = 0;
                int.TryParse(s.Trim(), out id);
                SourceIds.Add(id);
            }

            return SourceIds;
        }

        /// <summary>
        /// Get the distinct units that interface with the current entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private HashSet<string> InterfaceSources(Entity entity, string type)
        {
            FAASModel _context = new FAASModel();
            List<string> _units = new List<string>();
            List<int> _relationIds = new List<int>();
            List<int> _sourceIds = new List<int>();
            int value = 0;

            //get list of relIds for the interface
            if (type.Equals("external"))
            {
                foreach (Interface extInterface in _externalInterfaces)
                {
                    if (extInterface.TargetEntityId.Equals(entity.Id))
                    {
                        _relationIds = RetreiveIds(extInterface.EntityRelationshipIds.ToString());
                    }

                }
            }

            //get list of relIds for the interface
            else if (type.Equals("internal"))
            {
                foreach (InternalInterface intInterface in _internalInterfaces)
                {
                    if (intInterface.TargetEntityId.Equals(entity.Id))
                    {
                        _relationIds = RetreiveIds(intInterface.EntityRelationshipIds.ToString());
                    }

                }
            }
            //get the source (caller) id for each relationship
            //get unique source Unit ids
                foreach (int i in _relationIds)
                {
                    int d = (from q in _context.EntityRelationships
                             where q.Id == i
                             select q.CallingEntityId).SingleOrDefault();

                    foreach (Entity e in _entities)
                    {
                        if (d.Equals(e.Id))
                        {
                            _units.Add(e.NormalisedUnit);
                        }
                    }
                }

                var uniqueUnits = new HashSet<string>(_units);
                value = uniqueUnits.Count;

                return uniqueUnits;
        }

        private void PopulateEntityResidence(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();

            log.Log("Persist entity ownership strength to DB - start");
            try
            {
                foreach (EntityResidence item in _entStr)
                {
                    _context.EntityResidences.Add(item);
                }
                _context.SaveChanges();
                log.Log("Persist entity ownership strength to DB - complete");
                log.Log(string.Format("{0} entities assessed", _entities.Count));
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting entity ownership strength to DB: {0}: ", e.Message));
            }

        }

        #endregion

        #region utilities
        private void ClearTables()
        {
            Utility bla = new Utility();

            bla.ClearTable("Admin.InternalInterfaces");
            bla.ClearTable("Admin.EntityResidence");
            bla.ClearTable("Admin.InterfaceReporting");
            bla.ClearTable("Admin.BucketReporting");
            bla.ClearTable("Admin.BucketConnection");
        }
        #endregion 

        #region interface reporting
        private void BuildInterfaceReporting(ConsoleLog log)
        {
            log.Log("Build Interface Reports - start");
            FAASModel _context = new FAASModel();
            _entities = _context.Entities.ToList();

            foreach (Interface i in _externalInterfaces)
            {
                _interfaceReports.Add(CreateExternalInterfaceReport(i, "external"));
            }
            foreach (InternalInterface i in _internalInterfaces)
            {
                _interfaceReports.Add(CreateInternalInterfaceReport(i, "internal"));
            }
            log.Log("Build Interface Reports - complete");

        }

        private void PopulateInterfaceReporting(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();

            log.Log("Persist Interface Reports to DB - start");
            try
            {
                foreach (InterfaceReporting item in _interfaceReports)
                {
                    _context.InterfaceReportings.Add(item);
                }
                _context.SaveChanges();
                log.Log("Persist Interface Reports to DB - complete");
                log.Log(string.Format("{0} Interfaces assessed", _interfaceReports.Count));
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Interface Reports to DB: {0}: ", e.Message));
            }

        }

        private InterfaceReporting CreateExternalInterfaceReport(Interface i, string type)
        {
            //write processing output to same line
            Console.Write("Assessing interface for {0}            \r", i.TargetUnit);

            //id, entity, bucket/unit, type
            InterfaceReporting interfaceReport = new InterfaceReporting();
            interfaceReport.InterfaceId = i.Id;
            interfaceReport.TargetEntityId = i.TargetEntityId;
            interfaceReport.TargetUnit = i.TargetUnit;
            interfaceReport.Type = type;

            //determine total number of source entities
            //NEED ADD TO DB
            var entity = (from e in _entities
                          where e.Id == i.TargetEntityId
                          select e).FirstOrDefault();

            interfaceReport.TotalEntitySources = GetStrength(entity, type);

            //determine total number of source buckets
            interfaceReport.TotalBucketsSources = InterfaceSources(entity, type).Count();

            //get list/array of unique units
            interfaceReport.SourceBuckets = SourcesToString(InterfaceSources(entity, type));

            return interfaceReport;
        }

        private InterfaceReporting CreateInternalInterfaceReport(InternalInterface i, string type)
        {
            //write processing output to same line
            Console.Write("Assessing interface for {0}            \r", i.TargetUnit);

            //id, entity, bucket/unit, type
            InterfaceReporting interfaceReport = new InterfaceReporting();
            interfaceReport.InterfaceId = i.Id;
            interfaceReport.TargetEntityId = i.TargetEntityId;
            interfaceReport.TargetUnit = i.TargetUnit;
            interfaceReport.Type = type;

            //determine total number of source entities
            var entity = (from e in _entities
                          where e.Id == i.TargetEntityId
                          select e).FirstOrDefault();

            interfaceReport.TotalEntitySources = GetStrength(entity, type);

            //determine total number of source buckets
            interfaceReport.TotalBucketsSources = InterfaceSources(entity, type).Count();

            //get list/array of unique units
            interfaceReport.SourceBuckets = SourcesToString(InterfaceSources(entity, type));

            return interfaceReport;
        }

        private string SourcesToString(HashSet<string> units)
        {
            string line = "";
            int count = 0;

            foreach (string s in units)
            {
                if (s != null)
                {
                    if (count == 0)
                    {
                        line = line + s;
                        count++;
                    }
                    else
                    {
                        line = line + "," + s;
                    }
                }
            }

            return line;
        }
        #endregion

        #region bucket reporting
        private void BuildBucketReporting(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();
            _buckets = _context.Buckets.ToList();

            foreach (Bucket bucket in _buckets)
            {
                _bucketReports.Add(CreateBucketReport(bucket));
            }
        }

        private BucketReporting CreateBucketReport(Bucket b)
        {
            //write processing output to same line
            Console.Write("Assessing bucket {0}            \r", b.Name);

            BucketReporting bucketReport = new BucketReporting();
            bucketReport.BucketId = b.Id;
            bucketReport.Name = b.Name;
            bucketReport.Unit = b.Unit;

            List<InterfaceReporting> _interfacesPerBucket = new List<InterfaceReporting>();
            List<InterfaceReporting> _bucketsPerInterface = new List<InterfaceReporting>();
            List<Entity> _bucketEntities = new List<Entity>();

            var bucketEntities = (from e in _entities
                               where e.NormalisedUnit == b.Unit
                               select e).ToList().Count;
            bucketReport.Entities = bucketEntities;

            //number of internal interfaces
            var internalInterfaces = (from i in _interfaceReports
                                      where i.TargetUnit == b.Unit
                                      && i.Type == "internal"
                                      select i).Count();
            bucketReport.NumberInternalInterfaces = internalInterfaces;

            //number of external interfaces
            var externalInterfaces = (from i in _interfaceReports
                                      where i.TargetUnit == b.Unit
                                      && i.Type == "external"
                                      select i).Count();
            bucketReport.NumberExternalInterfaces = externalInterfaces;

                var dependingSources = DependingBucketSources(b);
                bucketReport.NumberCallingBuckets = DependingBucketSources(b).Count;
                bucketReport.CallingBuckets = BucketSourcesToString(dependingSources);

                var dependantSources = DependantBucketSources(b);
                bucketReport.NumberCalleduckets = DependantBucketSources(b).Count;
                bucketReport.CalledBuckets = BucketSourcesToString(dependantSources);
            
            return bucketReport;
        }

        private void PopulateBucketReporting(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();

            log.Log("Persist Bucket Reports to DB - start");
            try
            {
                foreach (BucketReporting item in _bucketReports)
                {
                    _context.BucketReportings.Add(item);
                }
                _context.SaveChanges();
                log.Log("Persist Bucket Reports to DB - complete");
                log.Log(string.Format("{0} Buckets assessed", _bucketReports.Count));
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Bucket Reports to DB: {0}: ", e.Message));
            }

        }

        /// <summary>
        /// Return units that current bucket uses
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<string> DependantBucketSources(Bucket bucket)
        {
            List<string> _units = new List<string>();

            //get list of relIds for the interface
            foreach (InterfaceReporting interfaceReport in _interfaceReports)
            {
                    if  (interfaceReport.SourceBuckets.Contains(bucket.Unit))
                    {
                          var exists = (from x in _units
                                      where x.Equals(interfaceReport.TargetUnit)
                                      select x);
                        if (!exists.Any())
                        {
                            _units.Add(interfaceReport.TargetUnit);
                        }
                    }
            }
            return _units;
        }


        /// <summary>
        /// Return units that use current bucket
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<string> DependingBucketSources(Bucket bucket)
        {
            List<string> _units = new List<string>();

            foreach (InterfaceReporting interfaceReport in _interfaceReports)
            {
                if (interfaceReport.TargetUnit.Equals(bucket.Unit))
                {
                    string[] sourceUnits = interfaceReport.SourceBuckets.ToString().Trim().Split(',');
                    //foreach source unit
                    foreach (string s in sourceUnits)
                    {
                        //does it already exist in the list?
                        var exists = (from x in _units
                                   where x.Equals(s)
                                   select x);
                        if (!exists.Any())
                        {
                            _units.Add(s);
                        }
                    }
                }
            }

         return _units;
        }

        private string BucketSourcesToString(List<string> _units)
        {
            string line = "";
            int count = 0;

            foreach (string s in _units)
            {
                if (s != null)
                {
                    if (count == 0)
                    {
                        line = line + s;
                        count++;
                    }
                    else
                    {
                        line = line + ", " + s;
                    }
                }
            }

            return line;
        }
        #endregion

        #region bucket connections
        private void BuildBucketConnections(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();
            _buckets = _context.Buckets.ToList();

            foreach (Bucket bucket in _buckets)
            {
                CreateBucketConnection(bucket);
            }
        }

        private void CreateBucketConnection(Bucket b)
        {
            foreach (Bucket otherb in _buckets)
            {
                if (otherb != b)
                {
                    BucketConnection bucketConnection = new BucketConnection();
                    bucketConnection.CallingBucket = b.Unit;
                    bucketConnection.CalledBucket = otherb.Unit;
                    bucketConnection.NumberCalledInterfaces = BucketCallingConnections(b, otherb).Count;
                    //write processing output to same line
                    Console.Write("Assessing connections between {0} and {1}            \r", bucketConnection.CallingBucket, bucketConnection.CalledBucket);
                    _bucketConnections.Add(bucketConnection);
                }
            }
        }

        private void PopulateBucketConnections(ConsoleLog log)
        {
            FAASModel _context = new FAASModel();

            log.Log("Persist Bucket Connections to DB - start");
            try
            {
                foreach (BucketConnection item in _bucketConnections)
                {
                    _context.BucketConnections.Add(item);
                }
                _context.SaveChanges();
                log.Log("Persist Bucket Connections to DB - complete");
                log.Log(string.Format("{0} bucket to bucket connections assessed", _bucketConnections.Count));
            }
            catch (Exception e)
            {
                log.Log(string.Format("Error persisting Bucket Connections to DB: {0}: ", e.Message));
            }

        }

        /// <summary>
        /// Return number of units representing unit called by unit connection
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<string> BucketCallingConnections(Bucket callingbucket, Bucket calledbucket)
        {
            List<string> _units = new List<string>();

            foreach (InterfaceReporting interfaceReport in _interfaceReports)
            {
                if (interfaceReport.TargetUnit.Equals(calledbucket.Unit))
                {
                    string[] sourceUnits = interfaceReport.SourceBuckets.ToString().Trim().Split(',');
                    //foreach source unit
                    foreach (string s in sourceUnits)
                    {
                        //Add each occurence of the calling bucket
                        if (s.Equals(callingbucket.Unit))
                        {
                            _units.Add(s);
                        }
                    }
                }
            }

            return _units;
        }
        #endregion

    }    

}
