using System.ComponentModel.DataAnnotations;

namespace BranchService_5003.Models
{
    public class ImportProductHistory
    {
        public int Id { get; set; }
        public int IdBranch { get; set; }
        public Branch Branch { get; set; }
        public int IdProduct { get; set; }
        public int IdBatch { get; set; }
        public short Quantity { get; set; }
        [MaxLength(20)]
        public string Consignee { get; set; }
        public DateTime ImportTime { get; set; }
    }
}
