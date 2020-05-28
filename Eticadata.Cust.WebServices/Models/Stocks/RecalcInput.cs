using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eticadata.Cust.WebServices.Models.Stocks
{
    public class RecalcInput
    {

        public DateTime? CloseDate { get; set; }
        public DateTime? ProcessoUntil { get; set; }
        public string[] Products { get; set; }
        public string[] Documents { get; set; }

    }
}