//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lifetrons.Erp.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuoteStatu
    {
        public QuoteStatu()
        {
            this.Quotes = new HashSet<Quote>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ShrtDesc { get; set; }
        public bool Authorized { get; set; }
        public bool Active { get; set; }
        public Nullable<int> Serial { get; set; }
    
        public virtual ICollection<Quote> Quotes { get; set; }
    }
}