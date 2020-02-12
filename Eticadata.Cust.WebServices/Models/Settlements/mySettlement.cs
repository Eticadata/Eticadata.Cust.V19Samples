namespace Eticadata.Cust.WebServices.Models.Settlements
{
    public class mySettlement
    {
        public string DocTypeAbbrev { get; set; }
        public string SectionCode { get; set; }
        public myPendingDocument PendingDocument { get; set; }
        public double ExtraCharge { get; set; }
    }

    public class myPendingDocument
    {
        public string FiscalYear { get; set; }
        public string SectionCode { get; set; }
        public string DocTypeAbbrev { get; set; }
        public int Number { get; set; }
    }
}