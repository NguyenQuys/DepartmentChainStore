﻿using System.ComponentModel.DataAnnotations.Schema;

namespace PromotionService_5004.Models
{
    public class PromotionType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Disable identity (auto-increment)
        public byte Id { get; set; }

        public string Type { get; set; }

        public ICollection<Promotion> Promotions { get; set; }
    }
}
