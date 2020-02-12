using System;
using System.Collections.Generic;

namespace Eticadata.Cust.WebServices.Models.RepairOrders
{
    public class myRepairOrder
    {
        public string FiscalYear { get; set; }
        public string SectionCode { get; set; }
        public string DocTypeAbbrev { get; set; }
        public int EntityCode { get; set; }
        public DateTime Date { get; set; }
        public string Vehicle { get; set; }

        public List<MaterialsLine> LinesMaterials { get; set; } = new List<MaterialsLine>();
        public List<InternalServicesLine> LinesInternalServices { get; set; } = new List<InternalServicesLine>();
        public List<ExternalServicesLine> LinesExternalServices { get; set; } = new List<ExternalServicesLine>();
    }

    public class MaterialsLine
    {
        public int LineNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public int VATTax { get; set; }
        public double UnitPriceExcludedVAT { get; set; }        
    }

    public class InternalServicesLine
    {
        public int LineNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public int VATTax { get; set; }
        public double UnitPriceExcludedVAT { get; set; }
    }

    public class ExternalServicesLine
    {
        public int LineNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public int VATTax { get; set; }
        public double UnitPriceExcludedVAT { get; set; }
    }
}