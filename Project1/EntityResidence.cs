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
    
    public partial class EntityResidence
    {
        public int EntityId { get; set; }
        public int InternalWeight { get; set; }
        public int ExternalWeight { get; set; }
        public int ExternalSources { get; set; }
    
        public virtual Entity Entity { get; set; }
    }
}
