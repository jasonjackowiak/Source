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
    
    public partial class Record
    {
        public int Id { get; set; }
        public int SnapshotId { get; set; }
        public int LanguageId { get; set; }
        public System.DateTime DateTimeStamp { get; set; }
        public string InputFileName { get; set; }
    
        public virtual LanguageReference LanguageReference { get; set; }
        public virtual Snapshot Snapshot { get; set; }
    }
}
