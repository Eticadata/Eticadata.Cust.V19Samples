using System.Collections.Generic;

namespace Eticadata.Cust.WebServices.Models
{
    public class myItem
    {
        public myItem() { }

        public string Code { get; set; } = "";
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string Abbreviation { get; set; } = "";
        public int VATRateSale { get; set; } = 0;
        public int VATRatePurchase { get; set; } = 0;
        public string MeasureOfStock { get; set; } = "";
        public string MeasureOfSale { get; set; } = "";
        public string MeasureOfPurchase { get; set; } = "";

        public List<myFamilies> Families { get; set; } = new List<myFamilies>();
        public List<myPricesLines> PricesLines { get; set; } = new List<myPricesLines>();
    }


    public class myFamilies
    {
        public myFamilies() { }

        public string Code { get; set; } = "";
    }

    public class myPricesLines
    {
        public myPricesLines() { }

        public int Number { get; set; } = 0;
        public double SalePrice { get; set; } = 0;
        public string Currency { get; set; } = "EUR";

        public string VATIncluded { get; set; } = "0";
    }
}