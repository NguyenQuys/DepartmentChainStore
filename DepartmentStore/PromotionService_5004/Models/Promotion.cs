using System.ComponentModel.DataAnnotations;

namespace PromotionService_5004.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        public byte Percentage { get; set; }

        public byte ApplyFor { get; set; }

        public int MinPrice { get; set; }

        public int MaxPrice { get; set; }

        public int InitQuantity { get; set; }

        public int RemainingQuantity { get; set; }

        public DateTime TimeUpdate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public byte IdPromotionType { get; set; }
        public PromotionType PromotionType { get; set; }
    }
}
