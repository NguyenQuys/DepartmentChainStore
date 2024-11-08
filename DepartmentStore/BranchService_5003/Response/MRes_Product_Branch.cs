namespace BranchService_5003.Response
{
    public class MRes_Product_Branch
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int IdProductCategory { get; set; }
        public int Price { get; set; }
        public bool IsHide { get; set; }
        public string BatchNumber { get; set; }
        public int Quantity { get; set; }
        public string MainImage { get; set; }
    }
}
