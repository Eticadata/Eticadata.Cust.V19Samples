using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eticadata.Cust.WebServices.Models.DepositReceipt
{
    public class InputDepositReceipt
    {
        public string TargetAccount { set; get; }
        public string SourceAccount { set; get; }
        public string Currency { set; get; }
        public string SectionCode { set; get; }
        public string FiscalPeriod { set; get; }
        public string Notes { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
    }
}