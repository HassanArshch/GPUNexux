using GpuStore.Data;
using GpuStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GpuStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly GpuContext _context;

        public AdminController(GpuContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? message, bool success = true)
        {
            var vm = new AdminGpuViewModel
            {
                Gpus = await _context.GPUs.OrderBy(g => g.Brand).ThenBy(g => g.Name).ToListAsync(),
                Message = message,
                IsSuccess = success
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new GPU());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GPU gpu)
        {
            if (!ModelState.IsValid)
                return View(gpu);

            gpu.CreatedAt = DateTime.UtcNow;
            if (!gpu.IsOnSale) gpu.SalePrice = null;

            _context.GPUs.Add(gpu);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { message = $"GPU '{gpu.Name}' added successfully!", success = true });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var gpu = await _context.GPUs.FindAsync(id);
            if (gpu == null) return NotFound();
            return View(gpu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GPU gpu)
        {
            if (id != gpu.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(gpu);

            if (!gpu.IsOnSale) gpu.SalePrice = null;

            _context.Entry(gpu).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { message = $"GPU '{gpu.Name}' updated successfully!", success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var gpu = await _context.GPUs.FindAsync(id);
            if (gpu == null) return NotFound();

            _context.GPUs.Remove(gpu);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { message = $"GPU '{gpu.Name}' removed.", success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSale(int id, float? salePrice)
        {
            var gpu = await _context.GPUs.FindAsync(id);
            if (gpu == null) return NotFound();

            gpu.IsOnSale = !gpu.IsOnSale;
            if (gpu.IsOnSale && salePrice.HasValue)
                gpu.SalePrice = salePrice;
            else if (!gpu.IsOnSale)
                gpu.SalePrice = null;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { message = $"Sale status updated for '{gpu.Name}'.", success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStock(int id)
        {
            var gpu = await _context.GPUs.FindAsync(id);
            if (gpu == null) return NotFound();

            gpu.InStock = !gpu.InStock;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { message = $"Stock status updated for '{gpu.Name}'.", success = true });
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderBy(o => o.Status == "Pending" ? 0 : 1)
                .ThenByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }
    }
}
