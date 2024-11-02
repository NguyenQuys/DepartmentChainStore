namespace BranchService_5003.Response
{
    public class MRes_ImportProductHistory
    {
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
        public short Quantity { get; set; }
        public DateTime DateImport { get; set; }
        public string Consignee { get; set; }
    }
}
