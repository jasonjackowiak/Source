//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project1
{
    using System;
    using System.Collections.Generic;
    
    public partial class LanguageReference
    {
        public LanguageReference()
        {
            this.Records = new HashSet<Record>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Variant { get; set; }
        public Nullable<decimal> Version { get; set; }
    
        public virtual ICollection<Record> Records { get; set; }
    }
}
