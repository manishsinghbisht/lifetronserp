using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Web.Models
{
    public class JobStatusModel
    {
        public Decimal JobNo { get; set; }
        public IEnumerable<JobProductIssue> JobProductIssues { get; set; }
        public IEnumerable<JobProductReceipt> JobProductReceipts { get; set; }
    }

}