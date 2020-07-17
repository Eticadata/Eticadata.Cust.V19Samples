
using Eticadata.Cust.WebServices.Helpers;
using System;

namespace Eticadata.Cust.WebServices.Models.OrdersSatisfaction
{
    public class MyOrderSatisfaction
    {
        public string DocTypeAbbrevSale { get; set; }
        public string FiscalYearSale { get; set; }
        public DateTime DateSale { get; set; }
        public DateTime ExpirationDateSale { get; set; }
        public DocumentKey KeyOrder { get; set; }
    }
}