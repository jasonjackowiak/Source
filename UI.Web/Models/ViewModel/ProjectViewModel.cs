using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UI.Web.Models.ViewModel
{

    public class ProjectViewModel
    {
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<Snapshot> Snapshots { get; set; }
    }

}