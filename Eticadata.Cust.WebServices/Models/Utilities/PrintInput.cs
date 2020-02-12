using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eticadata.Cust.WebServices.Models.Utilities
{
    public class PrintInput
    {
        public string DocFiscalYearCode { get; set; }

        public string DocType { get; set; }

        public string DocSeccion { get; set; }

        public int DocNumber { get; set; }
    }
}