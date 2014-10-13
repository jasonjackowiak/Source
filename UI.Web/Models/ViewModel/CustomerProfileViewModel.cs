using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UI.Web.Models.ViewModel
{
    public class CustomerViewModel
    {
        [Required]
        [Display(Name = "Customer Name")]
        public string Name { get; set; }
    }

    public class ProjectViewModel
    {
        [Required]
        [Display(Name = "Project Name")]
        public string Name { get; set; }
    }

    public class SnapshotViewModel
    {
        [Required]
        [Display(Name = "Snapshot Date")]
        public DateTime DateTimeStart { get; set; }
    }

}