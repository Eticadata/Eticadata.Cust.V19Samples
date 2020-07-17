using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eticadata.Cust.WebServices.Models.Accounting
{
    public class AccountingMov
    {
        public int Journal { get; set; }
        public DateTime Date { get; set; }

        public List<AccountingMovLines> Lines { get; set; }
    }

    public class AccountingMovLines
    {
        public int Number { get; set; }
        public string Account { get; set; }

        public string Description { get; set; }

        public double Value { get; set; }

        public char Signal { get; set; }
    }
}