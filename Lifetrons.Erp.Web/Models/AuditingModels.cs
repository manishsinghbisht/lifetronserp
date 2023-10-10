using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace Lifetrons.Erp.Models
{
    public class Audit
    {
        //A new SessionId that will be used to link an entire users "Session" of Audit Logs
        //together to help identifer patterns involving erratic behavior
        public string SessionId { get; set; }
        public Guid AuditId { get; set; }
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        
        public string UrlAccessed { get; set; }
        public DateTime TimeAccessed { get; set; }
        //A new Data property that is going to store JSON string objects that will later be able to
        //be deserialized into objects if necessary to view details about a Request
        public string Data { get; set; }

        public Audit()
        {
        }
    }

    public class AuditingContext : DbContext
    {
        public AuditingContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Audit> AuditRecords { get; set; }
    }
    
}