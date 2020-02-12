namespace Eticadata.Cust.WebServices.Helpers
{
    public class DocumentResult
    {
        public string ErrorDescription { get; set; }
        public DocumentKey DocumentKey { get; set; }
        public byte[] reportBytes { get; set; }
    }
}