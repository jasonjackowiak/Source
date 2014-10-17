//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UI.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Project
    {
        public Project()
        {
            this.Snapshots = new HashSet<Snapshot>();
        }
    
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
    
        public virtual ICollection<Snapshot> Snapshots { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
