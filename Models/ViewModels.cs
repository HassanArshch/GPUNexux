namespace GpuStore.Models
{
    public class GpuFilterViewModel
    {
        // Brand filters
        public bool FilterNvidia { get; set; } = true;
        public bool FilterAmd { get; set; } = true;
        public bool FilterIntel { get; set; } = true;

        // VRAM filter
        public List<int> SelectedVram { get; set; } = new();

        // Price range
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }

        // TDP filter
        public int? MaxTDP { get; set; }

        // Performance tier
        public string? Tier { get; set; } // Budget, Mid-Range, High-End, Flagship

        // Features
        public bool? RayTracing { get; set; }
        public bool? OnSaleOnly { get; set; }
        public bool? InStockOnly { get; set; }

        // Sort
        public string SortBy { get; set; } = "price_asc";

        // Results
        public List<GPU> Gpus { get; set; } = new();
        public List<GPU> HeroDeals { get; set; } = new();

        // Available filter options (populated from DB)
        public List<int> AvailableVramOptions { get; set; } = new() { 4, 6, 8, 12, 16, 20, 24, 32, 48 };
    }

    public class AdminGpuViewModel
    {
        public List<GPU> Gpus { get; set; } = new();
        public GPU? EditGpu { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
