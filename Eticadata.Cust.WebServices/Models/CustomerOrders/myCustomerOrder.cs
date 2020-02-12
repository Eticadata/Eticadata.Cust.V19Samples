
using Eticadata.Cust.WebServices.Helpers;
using System;
using System.Collections.Generic;

namespace Eticadata.Cust.WebServices.Models.CustomerOrders
{
    public class myCustomerOrder
    {
        public myCustomerOrder() {}

        public string FiscalYear { get; set; }
        public string SectionCode { get; set; }
        public string DocTypeAbbrev { get; set; }
        public int EntityCode { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CurrencyCode { get; set; }
        public List<CustomerOrderLine> Lines { get; set; } = new List<CustomerOrderLine>();
        public bool GetReportBytes { get; set; } = false;       
    }

    public class CustomerOrderLine
    {
        public int LineNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public int VATTax { get; set; }
        public double UnitPriceExcludedVAT { get; set; }
        public double Discount1 { get; set; }
        public double Discount2 { get; set; }
        public double Discount3 { get; set; }
        public double DiscountValue { get; set; }
    }
}

