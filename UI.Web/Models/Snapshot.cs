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
    
    public partial class Snapshot
    {
        public Snapshot()
        {
            this.Records = new HashSet<Record>();
        }
    
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Status { get; set; }
        public System.DateTime DateTimeStamp { get; set; }
    
        public virtual ICollection<Record> Records { get; set; }
        public virtual Project Project { get; set; }
    }
}