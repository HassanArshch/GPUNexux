using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GpuStore.Data;
using GpuStore.Models;

namespace GpuStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly GpuContext _context;

        public HomeController(GpuContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            bool nvidia = true, bool amd = true, bool intel = true,
            List<int>? vram = null, float? minPrice = null, float? maxPrice = null,
            int? maxTdp = null, bool? raytracing = null, bool? onSaleOnly = null,
            string sortBy = "price_asc")
        {
            var heroDeals = await _context.GPUs
                .Where(g => g.IsOnSale && g.InStock)
                .OrderByDescending(g => g.Price - (g.SalePrice ?? g.Price))
                .Take(4)
                .ToListAsync();

            var query = _context.GPUs.AsQueryable();

            // Brand filters
            var brands = new List<string>();
            if (nvidia) brands.Add("NVIDIA");
            if (amd) brands.Add("AMD");
            if (intel) brands.Add("Intel");
            if (brands.Count > 0)
                query = query.Where(g => brands.Contains(g.Brand));
            else
                query = query.Where(g => false);

            // VRAM filter
            if (vram != null && vram.Count > 0)
                query = query.Where(g => vram.Contains(g.VramGB));

            // Price range
            if (minPrice.HasValue) query = query.Where(g => g.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(g => g.Price <= maxPrice.Value);

            // TDP filter
            if (maxTdp.HasValue) query = query.Where(g => g.TDP <= maxTdp.Value);

            // Ray tracing
            if (raytracing.HasValue) query = query.Where(g => g.RayTracingSupport == raytracing.Value);

            // On sale only
            if (onSaleOnly == true) query = query.Where(g => g.IsOnSale);

            // Sorting
            query = sortBy switch
            {
                "price_desc" => query.OrderByDescending(g => g.Price),
                "name_asc" => query.OrderBy(g => g.Name),
                "boost_desc" => query.OrderByDescending(g => g.BoostClock),
                "vram_desc" => query.OrderByDescending(g => g.VramGB),
                "tdp_asc" => query.OrderBy(g => g.TDP),
                _ => query.OrderBy(g => g.Price),
            };

            var gpus = await query.ToListAsync();

            var vm = new GpuFilterViewModel
            {
                FilterNvidia = nvidia,
                FilterAmd = amd,
                FilterIntel = intel,
                SelectedVram = vram ?? new List<int>(),
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MaxTDP = maxTdp,
                RayTracing = raytracing,
                OnSaleOnly = onSaleOnly,
                SortBy = sortBy,
                Gpus = gpus,
                HeroDeals = heroDeals,
            };

            return View(vm);
        }

        // GET: Home/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var gpu = await _context.GPUs.FirstOrDefaultAsync(g => g.Id == id);

            if (gpu == null)
            {
                return NotFound();
            }

            return View(gpu);
        }
    }
}
