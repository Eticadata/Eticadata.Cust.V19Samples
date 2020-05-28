using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eticadata.Cust.WebServices.Models.Stocks
{
    public class Stock
    {
        public string SectionCode { get; set; }
        public string DocTypeAbbrev { get; set; }
        public DateTime Date { get; set; }
        public List<StockLines> Lines { get; set; } = new List<StockLines>();
    }

    public class StockLines
    {
        public int LineNumber { get; set; }
        public string Warehouse { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public double UnitCost { get; set; }
    }
}