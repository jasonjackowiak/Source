//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project1
{
    using System;
    using System.Collections.Generic;
    
    public partial class PhaseReference
    {
        public PhaseReference()
        {
            this.Snapshots = new HashSet<Snapshot>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Snapshot> Snapshots { get; set; }
    }
}
