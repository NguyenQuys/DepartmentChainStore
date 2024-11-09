namespace BranchService_5003.Models
{
    public class Product_Branch
    {
        public int Id { get; set; }

        public int IdBranch { get; set; }

        public Branch Branch { get; set; }

        public int IdProduct { get; set; }

        public int IdBatch { get; set; }

        public int Quantity { get; set; }
    }
}
