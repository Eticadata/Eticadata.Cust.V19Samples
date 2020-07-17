using System.Collections.Generic;

namespace Eticadata.Cust.WebServices.Models.Treasury
{
    public class InputWithdrawalTransfer
    {
        public string SectionCode { set; get; }
        public string AccountCode { set; get; }
        public string PayTransfType { set; get; }
        public string Currency { set; get; }

        public List<WithdrawalTransferLines> Lines { set; get; }
    }

    public class WithdrawalTransferLines
    {
        public string PayTransfType { set; get; }
        public string AccountCode { set; get; }
        public string Currency { set; get; }
        public double ValueDeb { set; get; }
    }
}