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
    
    public partial class InterfaceReporting
    {
        public int Id { get; set; }
        public int InterfaceId { get; set; }
        public int TargetEntityId { get; set; }
        public string TargetUnit { get; set; }
        public string Type { get; set; }
        public int TotalEntitySources { get; set; }
        public int TotalBucketsSources { get; set; }
        public string SourceBuckets { get; set; }
    
        public virtual Entity Entity { get; set; }
        public virtual Interface Interface { get; set; }
    }
}
