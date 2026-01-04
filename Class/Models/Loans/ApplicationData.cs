namespace LoanApplicationUI
{
    public class ApplicationData
    {
        public string Id { get; set; }          // application_number
        public string Customer { get; set; }
        public string LoanType { get; set; }
        public string Amount { get; set; }      // "₱12,345.00"
        public string AppliedDate { get; set; }
        public string Status { get; set; }
    }
}