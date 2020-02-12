namespace Eticadata.Cust.WebServices.Models.Customers
{
    public class myCustomer
    {
        public myCustomer(){}

        public bool GenerateNewCode { get; set; } = false;
        public int Code { get; set; } = 0;
        public string Name { get; set; } = "";
        public string AddressLine1 { get; set; } = "";
        public string AddressLine2 { get; set; } = "";
        public string Locality { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Email { get; set; } = "";
        public string SubZone { get; set; } = "";
        public string PaymentTerm { get; set; } = "";
    }
}