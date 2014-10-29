using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UI.Web.Models.ViewModel
{

    public class SnapshotViewModel
    {
        public IEnumerable<Snapshot> Snapshots { get; set; }
        public IEnumerable<Record> SnapshotRecords { get; set; }
    }

}