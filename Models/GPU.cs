using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GpuStore.Models
{
    public class GPU
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty; // NVIDIA, AMD, Intel

        [Required, StringLength(100)]
        public string Architecture { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ChipName { get; set; } = string.Empty; // e.g. GA102, Navi 31

        public int VramGB { get; set; }

        [Required]
        public string VramType { get; set; } = string.Empty; // GDDR6, GDDR6X, HBM2e

        public int MemoryBusWidth { get; set; } // 128, 192, 256, 384 bit

        public int CoreCount { get; set; } // Shader processors

        public int BaseClock { get; set; } // MHz

        public int BoostClock { get; set; } // MHz

        public int TDP { get; set; } // Watts

        public int PCIeGeneration { get; set; } // 4 or 5

        public int PCIeLanes { get; set; } // x8 or x16

        [StringLength(50)]
        public string? DisplayOutputs { get; set; } // e.g. "3x DP 2.1, 1x HDMI 2.1"

        public bool RayTracingSupport { get; set; }

        public bool DLSSSupport { get; set; } // or FSR for AMD

        [Column(TypeName = "decimal(10,2)")]
        public float Price { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public float? SalePrice { get; set; }

        public bool IsOnSale { get; set; }

        public bool InStock { get; set; } = true;

        public string? ImageUrl { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Computed
        [NotMapped]
        public float DisplayPrice => IsOnSale && SalePrice.HasValue ? SalePrice.Value : Price;

        [NotMapped]
        public int? DiscountPercent => IsOnSale && SalePrice.HasValue
            ? (int)Math.Round((1 - SalePrice.Value / Price) * 100)
            : null;
    }
}
