using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Web.Models
{
    public class MasterProductionScheduleViewModel : ProdPlanDetail
    {
        
        //Production Quantity - PlannedQuantity
        public decimal QuantityUnplanned { get; set; }

        public decimal QuantityPlanned { get; set; }
        //Quantity issued to Assembly process
        public decimal QuantityInProduction { get; set; }

        public decimal OrderQuantity { get; set; }

        public decimal OperationSequence { get; set; }
        public string OperationName { get; set; }

        public DateTime OperationStartDate { get; set; }
        public DateTime OperationEndDate { get; set; }
        public decimal OperationTimeInHour { get; set; }
        public decimal OperationCapacity { get; set; }
    }
}